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
using System.IO;
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
        /// currently open file path
        /// </summary>
        string _openfilepath;

        /// <summary>
        /// Get total number of rows that we have (after filtering and sorting on the datatable)
        /// </summary>
        private int TotalRows
        {
            get { return (_dataview == null) ? 0 : _dataview.Count; }
        }

        /// <summary>
        /// Get set index of current page to display in dataGridView1
        /// </summary>
        private int CurrentPage
        {
            get { return (int)this.numericUpDown_CurrentPage.Value;  }
        }

        /// <summary>
        /// Get the total number of pages
        /// </summary>
        private int TotalPages
        {
            get { return (_dataview == null) ? 0 : (TotalRows/RowsPerPage + 1); }
        }

        /// <summary>
        /// Get set number of records per page to show in dataGridView1
        /// </summary>
        private int RowsPerPage
        {
            get { return (int)this.numericUpDown_RowsPerPage.Value;  }
        }

        RecentFilesFolders Recent = new RecentFilesFolders();

        public MainForm()
        {
            InitializeComponent();

            RefreshOpenRecentMenu();
            InitPagingCtrlsFromDefaults();
        }

        /// <summary>
        /// Initialise controls before any file is loaded.
        /// </summary>
        private void InitPagingCtrlsFromDefaults()
        {
            this.numericUpDown_CurrentPage.Maximum = int.MaxValue;
            this.numericUpDown_CurrentPage.Minimum = 0;
            this.numericUpDown_CurrentPage.Value = 0;
            this.numericUpDown_RowsPerPage.Maximum = int.MaxValue;
            this.numericUpDown_RowsPerPage.Minimum = 1;
            this.numericUpDown_RowsPerPage.Value = Properties.Settings.Default.RowsPerPage;
        }

        /// <summary>
        /// On file load adjust some boundaries and reset user modifiable values.
        /// </summary>
        private void InitPagingCtrlsFromFileLoad()
        {
            // Note: here the order of initialisation is very important. :)
            this.numericUpDown_RowsPerPage.Value = Properties.Settings.Default.RowsPerPage;
            this.numericUpDown_CurrentPage.Value = 0;
            // show some stats
            this.toolStripStatusLabel_CurrentSortColumn.Text = "none" + "\t";
            this.toolStripStatusLabel_TotalPages.Text = TotalPages.ToString() + "\t";
            this.toolStripStatusLabel_TotalRows.Text = TotalRows.ToString() + "\t";
        }

        private void InitOnOpenFileCSV()
        {
            _dataview = new DataView(_datatable);
            this.dataGridView1.DataSource = _dataview;
            InitPagingCtrlsFromFileLoad();
        }

        /// <summary>
        /// Get the items to show in open recent menu
        /// </summary>
        private void RefreshOpenRecentMenu()
        {
            // just replace old menu item wth a new one to refresh it
            Recent.ReplaceOpenRecentMenu(openRecentToolStripMenuItem1
                , (filepath) 
                => {
                    // if (file opened ok) then push it to recent
                    if (OpenFileCSV(filepath))
                    {
                        Recent.AddRecentFile(filepath);
                        RefreshOpenRecentMenu();
                    }
                }
            );
        }

        private string OpenFileDialogGetPath()
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
                return file_dialog.FileName;
            }
            return null;
        }

        /// <summary>
        /// Show open file dialog to choose csv file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string filepath = OpenFileDialogGetPath();
            if (filepath == null) {
                return;
            }
            Properties.Settings.Default.RecentDirectory = Path.GetDirectoryName(filepath);
            if (OpenFileCSV(filepath))
            {
                Recent.AddRecentFile(filepath);
                RefreshOpenRecentMenu();
            }
        }

        /// <summary>
        /// Clicking on "New" menu entry creates empty datatable with default type.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _datatable = EmptyTableFromType<RegistryOfficeDataRow>();
            _openfilepath = null;
            InitOnOpenFileCSV();
        }

        /// <summary>
        /// Read through the properties of T and 
        /// assemble a DataTable that would represent it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(IList<T> data)
        {
            var table = EmptyTableFromType<T>();
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
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

        /// <summary>
        /// Make an empty data table with layout to contain type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataTable EmptyTableFromType<T>()
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
            {
               // handle nullable types
               table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            return table;
        }

        /// <summary>
        /// Open file, read and verify data, make data table and run control intialisations.
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        private bool OpenFileCSV(string filepath)
        {
            try
            {
                using (var input_stream = new StreamReader(filepath))
                {
                    var datarows 
                        = OpenData.LoadArray<RegistryOfficeDataRow>(input_stream);
                    // convert list to table
                    _openfilepath = filepath;
                    _datatable = ToDataTable(datarows);
                    _openfilepath = filepath;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                return false;
            }
            // do various initialisations on file opening
            InitOnOpenFileCSV();
            return true;
        }

        /// <summary>
        /// Useful so user can supply their match exactly.
        /// </summary>
        /// <param name="valueWithoutWildcards"></param>
        /// <returns></returns>
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
        /// Clears the internal filters and updates grid view.
        /// Does not erase the strings that user entered into Fileter text boxes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_ClearFilters_Click(object sender, EventArgs e)
        {
            _dataview.RowFilter = String.Empty;
            RefreshDataGridViewPager();
        }

        /// <summary>
        /// Apply user submitted query to data table rows.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_SubmitFilter_Click(object sender, EventArgs e)
        {
            string area_code = this.textBox_FilterAdmAreaCode.Text;
            string area_name = this.textBox_FilterAdmAreaName.Text;
            bool filter_code = area_code.Count() > 0;
            bool filter_name = area_name.Count() > 0;
            if (filter_code && filter_name)
            {
                area_code = EscapeLikeFilterValue(area_code);
                area_name = EscapeLikeFilterValue(area_name);
                _dataview.RowFilter = string.Format(@"(AUTHOR like '*{0}*') AND (ISBN like '*{1}*')", area_name, area_code);
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
            }
            else
            {
                _dataview.RowFilter = String.Empty;
            }
            RefreshDataGridViewPager();
        }

        /// <summary>
        /// Called when we change to another page or change number rows per page.
        /// Takes the filters into account.
        /// 
        /// This is the event handler for numericUpDown.... ValueChanged event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshDataGridViewPager(object sender=null, EventArgs e = null)
        {
            if (_datatable == null)
            {
                return;
            }
            IEnumerable<DataRow> toshow = _datatable.Select(_dataview.RowFilter, _dataview.Sort)
                .Skip(CurrentPage * RowsPerPage)
                .Take(RowsPerPage);
            DataView paged_view = new DataView(toshow.Count() > 0 ? toshow.CopyToDataTable() : _datatable.Clone());
            this.dataGridView1.DataSource = paged_view;
        }

        /// <summary>
        /// Write the datatable from memory to file.
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="append"></param>
        private void SaveFileCSV(string filepath, bool append)
        {
            try
            {
                using (var output_stream = new StreamWriter(filepath, append))
                {
                    SaveData.WriteToStream(_dataview.ToTable()
                        , output_stream
                    );
                    _openfilepath = filepath;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not save file to disk. Original error: " + ex.Message);
            }
        }

        /// <summary>
        /// Opens the dialog to get the path at which to save the current data.
        /// </summary>
        /// <returns></returns>
        private string SaveFileDialogGetPath()
        {
            SaveFileDialog file_dialog = new SaveFileDialog
            {
                InitialDirectory = Properties.Settings.Default.RecentDirectory,
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                FilterIndex = 0,
                RestoreDirectory = true,
                Title = "Select where to save your csv file...",
            };
            if (file_dialog.ShowDialog() != DialogResult.Yes) {
                return file_dialog.FileName;
            }
            return null;
        }

        /// <summary>
        /// Save As (new file) button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string filepath = SaveFileDialogGetPath();
            if (filepath == null) {
                // user canceled save operation
                return;
            }
            _openfilepath = filepath;
            Properties.Settings.Default.RecentDirectory = Path.GetDirectoryName(filepath);
            SaveFileCSV(filepath, append: false);
            Recent.AddRecentFile(filepath);
            RefreshOpenRecentMenu();
        }

        /// <summary>
        /// Save To (append) menu entry is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // if we just created this file in memory and don't know where to save it yet.
            // run file selection dialog
            if (_openfilepath == null)
            {
                string filepath = SaveFileDialogGetPath();
                if (filepath == null) {
                    // user canceled save operation
                    return;
                }
                _openfilepath = filepath;
                SaveFileCSV(_openfilepath, append: true);
                Recent.AddRecentFile(filepath);
                RefreshOpenRecentMenu();
            }
        }

        /// <summary>
        /// Save (overwrite) menu button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // if we just created this file in memory and don't know where to save it yet.
            // run file selection dialog
            if (_openfilepath == null)
            {
                string filepath = SaveFileDialogGetPath();
                if (filepath == null) {
                    // user canceled save operation
                    return;
                }
                _openfilepath = filepath;
                SaveFileCSV(filepath, append: false);
                Recent.AddRecentFile(filepath);
                RefreshOpenRecentMenu();
            }
        }
    }
}
