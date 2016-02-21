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
        // Will put information from csv into here
        DataTable _datatable;
        DataView _dataview;

        /// <summary>
        /// Get set current page to display in dataGridView1
        /// </summary>
        private int CurrentPage
        {
            get { return (int)this.numericUpDown_CurrentPage.Value;  }
            set
            {
                this.numericUpDown_CurrentPage.Value = value;
                RefreshDataGridViewPager(null, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Get set number of record per page in dataGridView1
        /// </summary>
        private int RowsPerPage
        {
            get { return (int)this.numericUpDown_RowsPerPage.Value;  }
            set
            {
                this.numericUpDown_RowsPerPage.Value = value;
                RefreshDataGridViewPager(null, EventArgs.Empty);
            }
        }
        

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
                // if (file opened ok) only then add it to recent
                tool.Click += delegate
                {
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
            OpenFileDialog file_dialog = new OpenFileDialog
            {
                InitialDirectory = Properties.Settings.Default.RecentDirectory,
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                FilterIndex = 0,
                RestoreDirectory = true,
                Title = "Select a csv file...",
            };
            if (file_dialog.ShowDialog() != DialogResult.OK) {
                return;
            }
            Properties.Settings.Default.RecentDirectory = Path.GetDirectoryName(file_dialog.FileName);
            if (OpenFileCSV(file_dialog.FileName))
            {
                AddOpenRecentEntry(file_dialog.FileName);
            }
        }

        public static DataTable ToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }
            return table;
        }

        private bool OpenFileCSV(string filepath)
        {
            try
            {
                using (var input_stream = new StreamReader(filepath))
                {
                    var datarows = CSV.LoadArray<RegistryOfficeDataRow>(input_stream
                        , ignore_dimension_errors: false
                        , ignore_bad_columns: false
                        , ignore_type_conversion_errors: false
                        , delim: ','
                        , qual: '"'
                    );
                    // convert list to table
                    _datatable = ToDataTable(datarows);
                    _dataview = new DataView(_datatable);
                    this.dataGridView1.DataSource = _dataview;
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                return false;
            }
        }

        public static string EscapeLikeFilterValue(string valueWithoutWildcards)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < valueWithoutWildcards.Length; i++)
            {
                char c = valueWithoutWildcards[i];
                if (c == '*' || c == '%' || c == '[' || c == ']')
                    sb.Append("[").Append(c).Append("]");
                else if (c == '\'')
                    sb.Append("''");
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }


        /// <summary>
        /// Run user submitted query.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_SubmitFilter_Click(object sender, EventArgs e)
        {
            string area_code = this.textBox_FilterAdmAreaCode.Text;
            string area_name = this.textBox_FilterAdmAreaName.Text;

            bool filter_code = area_code.Count() > 0;
            bool filter_name = area_name.Count() > 0;

            try
            {
                if (filter_code && filter_name)
                {
                    area_code = EscapeLikeFilterValue(area_code);
                    area_name = EscapeLikeFilterValue(area_name);
                    _dataview.RowFilter = string.Format(@"(AUTHOR like '*{0}*') AND (ISBN like '*{1}*')", area_name, area_code);
                    MessageBox.Show("__" + _dataview.RowFilter + "__");
                }
                else if (filter_code)
                {
                    area_code = EscapeLikeFilterValue(area_code);
                    _dataview.RowFilter = string.Format(@"ISBN like '*{0}*'", area_code);
                }
                else if (filter_name)
                {
                    area_name = EscapeLikeFilterValue(area_name);
                    _dataview.RowFilter = string.Format(@"AUTHOR like '*{0}*'", area_name);
                    MessageBox.Show("__" + _dataview.RowFilter + "__");
                }
                else
                {
                    _dataview.RowFilter = String.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        /// <summary>
        /// Called when we change to another page or change number rows per page.
        /// Takes the filters into account.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshDataGridViewPager(object sender, EventArgs e)
        {
            if (_datatable == null)
            {
                return;
            }
            int rows_per_page = (int)this.numericUpDown_RowsPerPage.Value;
            int cur_page = (int)this.numericUpDown_CurrentPage.Value;
            IEnumerable<DataRow> toshow = _datatable.Select(_dataview.RowFilter, _dataview.Sort)
                .Skip(cur_page * rows_per_page)
                .Take(rows_per_page);
            DataView paged_view = new DataView(toshow.Count() > 0 ? toshow.CopyToDataTable() : _datatable.Clone());
            this.dataGridView1.DataSource = paged_view;
            // EnumerableRowCollection<DataRow> query =
            //     from row in _datatable.AsEnumerable()
            //     select row;
            // DataView view = query.AsDataView();
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }
    }
}
