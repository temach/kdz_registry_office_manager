using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using csv_parser;
using System.IO;

namespace kdz_manager
{
    public partial class MainForm : Form
    {
        // sample string not used anymore
        string source = @"timestamp,TestString,SetComment,PropertyString,IntField,IntProperty
2012-05-01,test1,""Hi there, I said!"",Bob,57,0
2011-04-01,test2,""What's up, buttercup?"",Ralph,1,-999
1975-06-03,test3,""Bye and bye, dragonfly!"",Jimmy's The Bomb,12,13";

        // Will put information from csv into here
        List<RegistryOfficeDataRow> _data_rows = null;

        public MainForm()
        {
            InitializeComponent();

            // Configure settings
            var recent_dir = Properties.Settings.Default.RecentDirectory;
            // if the recent direcotry is invalid or not set
            if (recent_dir.Length < 2)
            {
                // Try to portably get the user's home directory
                recent_dir = 
                    (Environment.OSVersion.Platform == PlatformID.Unix 
                        || Environment.OSVersion.Platform == PlatformID.MacOSX)
                    ? Environment.GetEnvironmentVariable("HOME")
                    : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            }

            // Check that RecentFiles is initialised
            if (Properties.Settings.Default.RecentFiles == null)
            {
                Properties.Settings.Default.RecentFiles  = new ToolStripMenuItem(); 
            }
            // refil recent files
            var recent_files = Properties.Settings.Default.RecentFiles.DropDownItems;
            this.openRecentToolStripMenuItem1.DropDownItems.Clear();
            this.openRecentToolStripMenuItem1.DropDownItems.AddRange(recent_files);

            // Save settings for next run
            Properties.Settings.Default.Save();
        }


        public void LoadData()
        {
            DataTable dt = CSV.LoadString(source, true, false);
        }

        /// <summary>
        /// If we have already created the tool strip we just need to move it to the top.
        /// </summary>
        private void AddOpenRecentEntry(ToolStripMenuItem recent_file_entry)
        {
            // Add file to begginning of recent files
            var recent_files = Properties.Settings.Default.RecentFiles.DropDownItems;
            recent_files.Insert(0 , recent_file_entry);
            // If we exceed the limit, then drop the last item
            if (recent_files.Count > Properties.Settings.Default.QtyOfRecentFiles)
            {
                recent_files.RemoveAt(recent_files.Count - 1);
            }
            // refil the OpenRecent menu
            this.openRecentToolStripMenuItem1.DropDownItems.Clear();
            this.openRecentToolStripMenuItem1.DropDownItems.AddRange(recent_files);
        }

        /// <summary>
        /// This handles is custom registered to every MenuItem in OpenRecent 
        /// dialog.
        /// </summary>
        private void openRecentToolStipMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var recent_dir = Properties.Settings.Default.RecentDirectory;

            OpenFileDialog file_dialog = new OpenFileDialog();
            file_dialog.InitialDirectory = recent_dir;
            file_dialog.Filter = "CSV files (*.csv)|*.csv|txt files (*.txt)|*.txt|All files (*.*)|*.*";
            file_dialog.FilterIndex = 0;
            file_dialog.RestoreDirectory = true;
            file_dialog.Title = "Select a csv file.";

            if (file_dialog.ShowDialog() == DialogResult.OK)
            {
                // next time we start go to this direcory
                recent_dir = Path.GetDirectoryName(file_dialog.FileName);

                // Add file to recent files
                AddOpenRecentEntry(new ToolStripMenuItem(file_dialog.FileName
                    , null
                    , openRecentToolStipMenuItem_Click)
                );

                ReadDataRows(file_dialog.FileName);
            }
        }

        private void ReadDataRows(string filepath)
        {
            try
            {
                using (var input_stream = new StreamReader(filepath))
                {
                    // read data and put into Form1 member
                    // _data_rows = CSV.LoadArray<RegistryOfficeDataRow>(input_stream
                    //     , ignore_dimension_errors: false
                    //     , ignore_bad_columns: false
                    //     , ignore_type_conversion_errors: false
                    //     , delim: ','
                    //     , qual: '"'
                    // );
                    DataTable dt = CSV.LoadDataTable(input_stream
                        , first_row_are_headers: true
                        , ignore_dimension_errors: false
                        , delim: ','
                        , qual: '"'
                    );
                    this.dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
            }
        }


    }
}
