using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using csv_parser;
using System.IO;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;

namespace kdz_manager
{
    public partial class MainForm : Form
    {
        // sample string not used anymore
        string source = @"timestamp,TestString,SetComment,PropertyString,IntField,IntProperty
2012-05-01,test1,""Hi there, I said!"",Bob,57,0
2011-04-01,test2,""What's up, buttercup?"",Ralph,1,-999
1975-06-03,test3,""Bye and bye, dragonfly!"",Jimmy's The Bomb,12,13";

        string _filter_code_template = @"
            using System;
            using System.IO;
            using System.Linq;
            using System.Windows.Forms;
            namespace kdz_manager {

            static class kdzManagerCompiledUserFilter {
                static DataTable datatable = null;
                static Type datarowtype = null;

                public DataTable RunFilter(Type data_row_type) {
                    var query = {{0}}   // TODO: conditionall add final semicolon
                    DataTable new_dt = query.CopyToDataTable<datarowtype>();
                    return new_dt;
                }
            }
            }";

        // Will put information from csv into here
        List<RegistryOfficeDataRow> _data_rows = null;
        BindingSource _dt_binding_source = new BindingSource();

        public MainForm()
        {
            InitializeComponent();

            InitRecentDir();
            InitOpenRecentList();
        }


        /// <summary>
        /// Set sensible value for directory in OpenFile dialog.
        /// </summary>
        private void InitRecentDir()
        {
            if (Properties.Settings.Default.RecentDirectory == null)
            {
                // if the recent direcotry is screwed up or not set
                Properties.Settings.Default.RecentDirectory 
                    = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            }
        }

        /// <summary>
        /// Initially build the Recent-Files menu
        /// </summary>
        private void InitOpenRecentList()
        {
            if (Properties.Settings.Default.RecentFiles == null)
            {
                Properties.Settings.Default.RecentFiles = new StringCollection();
                Properties.Settings.Default.Save();
            }
            RefreshOpenRecentToolStripDropDown();
        }

        /// <summary>
        /// The OpenRecent files have changed. Refresh the view in menu.
        /// </summary>
        private void RefreshOpenRecentToolStripDropDown()
        {
            if (Properties.Settings.Default.RecentFiles.Count == 0)
            {
                openRecentToolStripMenuItem1.DropDownItems.Add("No recent files...");
                return;
            }

            openRecentToolStripMenuItem1.DropDownItems.Clear();
            foreach (var str in Properties.Settings.Default.RecentFiles)
            {
                var tool = openRecentToolStripMenuItem1.DropDownItems.Add(str);
                // so that "s" var is not cached by LINQ use "tool.Text"
                // only add to recent if file opened ok
                tool.Click += delegate {
                    if (OpenFileCSV(tool.Text)) { AddOpenRecentEntry(tool.Text); }
                };
            }
        }

        /// <summary>
        /// Add a new item to the Recent-Files menu and save it persistently
        /// </summary>
        /// <param name="file"></param>
        private void AddOpenRecentEntry(string file)
        {
            var recent = Properties.Settings.Default.RecentFiles;
            if (recent.Count > Properties.Settings.Default.QtyOfRecentFiles)
            {
                recent.RemoveAt(recent.Count - 1);
            }
            // Reinsert at 0 position (does not throw if not found)
            recent.Remove(file);
            recent.Insert(0, file);
            Properties.Settings.Default.Save();

            RefreshOpenRecentToolStripDropDown();
        }

        /// <summary>
        /// Show open file dialog to choose csv file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var recent_dir = Properties.Settings.Default.RecentDirectory;

            OpenFileDialog file_dialog = new OpenFileDialog();
            file_dialog.InitialDirectory = recent_dir;
            file_dialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            file_dialog.FilterIndex = 0;
            file_dialog.RestoreDirectory = true;
            file_dialog.Title = "Select a csv file...";

            if (file_dialog.ShowDialog() == DialogResult.OK)
            {
                // next time we start go to this direcory
                recent_dir = Path.GetDirectoryName(file_dialog.FileName);
                if (OpenFileCSV(file_dialog.FileName))
                {
                    // only add good files to open recent
                    AddOpenRecentEntry(file_dialog.FileName);
                }
            }
        }

        private bool OpenFileCSV(string filepath)
        {
            try
            {
                using (var input_stream = new StreamReader(filepath))
                {
                    _dt_binding_source = new BindingSource();
                    _dt_binding_source.DataSource = CSV.LoadArray<RegistryOfficeDataRow>(input_stream
                        , ignore_dimension_errors: false
                        , ignore_bad_columns: false
                        , ignore_type_conversion_errors: false
                        , delim: ','
                        , qual: '"'
                    );
                    this.dataGridView1.DataSource = _dt_binding_source;
                   // DataTable dt = CSV.LoadDataTable(input_stream
                   //     , first_row_are_headers: true
                   //     , ignore_dimension_errors: false
                   //     , delim: ','
                   //     , qual: '"'
                   // );
                   // this.dataGridView1.DataSource = dt;
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Method to check that user filter looks valid and contains no harmful code.
        /// </summary>
        /// <param name="user_filter"></param>
        /// <returns></returns>
        private bool VerifyUserFilter(string user_filter)
        {
            return true;
        }

        /// <summary>
        /// Run user submitted query.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_SubmitFilter_Click(object sender, EventArgs e)
        {
            string user_filter = this.richTextBox_FilterInput.Text;
            var basic_check = VerifyUserFilter(user_filter);
            // if (basic_check.Errors.HasError)
            // {
            //     MessageBox.Show("bla bla");
            //     return;
            // }
            string final_code = string.Format(_filter_code_template, user_filter); 

            var csc = new CSharpCodeProvider();
            var csc_params = new CompilerParameters(
                new string[]{"System.dll","System.Core.dll","mscorlib.dll","System.Windows.Forms.dll"}
            );
            // add this as refrence assembley so we can use our types
            csc_params.ReferencedAssemblies.Add(Path.GetFileName(Assembly.GetExecutingAssembly().Location));
            csc_params.GenerateExecutable = false;
            csc_params.GenerateInMemory = true;

            CompilerResults compile_result = csc.CompileAssemblyFromSource(csc_params, final_code);
            if (compile_result.Errors.HasErrors)
            {
                var errs = compile_result.Errors;
                StringBuilder err_log = new StringBuilder(errs.Count + " Errors in filter: ");
                for (int i = 0; i < errs.Count; i++)
                {
                    err_log.AppendFormat("\n{0}: at character {1} - {2}", i, errs[i].ErrorText, errs[i].Column);
                }
                MessageBox.Show(err_log + "\n\nFor this filter:\n" + user_filter, "Errors in supplied data table filter.");
                return;
            }

            Assembly assembly = compile_result.CompiledAssembly;
            Type compiled_class = assembly.GetType("kdz_manager.kdzManagerCompiledUserFilter");
            MethodInfo filter_method = compiled_class.GetMethod("DoRun");
            var filter_results = filter_method.Invoke(null, null);

            _dt_binding_source.DataSource = filter_results;
        }

    }
}
