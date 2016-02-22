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
        private void CalculateFileStats()
        {
            this.toolStripStatusLabel_CurrentSortColumn.Text = "none" + "\t";
            this.toolStripStatusLabel_TotalPages.Text = TotalPages.ToString() + "\t";
            this.toolStripStatusLabel_TotalRows.Text = TotalRows.ToString() + "\t";
        }

        private void InitOnCreateNewTable()
        {
            // clear path so its not added to open recent
            Recent.CurrentlyOpenFilePath = null;
            // reset page and items per page
            this.numericUpDown_RowsPerPage.Value = Properties.Settings.Default.RowsPerPage;
            this.numericUpDown_CurrentPage.Value = 0;
            // assign to datagrid
            _dataview = new DataView(_datatable);
            this.dataGridView1.DataSource = _dataview;
            // update status bar
            CalculateFileStats();
        }

        private void InitOnSaveDiskFile(string filepath)
        {
            Recent.CurrentlyOpenFilePath = filepath;
            // add to open recent
            Recent.AddRecentFile(filepath);
            RefreshOpenRecentMenu();
        }

        private void InitOnOpenDiskFile(string filepath)
        {
            Recent.CurrentlyOpenFilePath = filepath;
            // add to open recent
            Recent.AddRecentFile(filepath);
            RefreshOpenRecentMenu();
            // reset page and items per page
            this.numericUpDown_RowsPerPage.Value = Properties.Settings.Default.RowsPerPage;
            this.numericUpDown_CurrentPage.Value = 0;
            // assign to datagrid
            _dataview = new DataView(_datatable);
            this.dataGridView1.DataSource = _dataview;
            // update status bar
            CalculateFileStats();
        }

        /// <summary>
        /// Get the items to show in open recent menu
        /// </summary>
        private void RefreshOpenRecentMenu()
        {
            // just replace old menu item wth a new one to refresh it
            Recent.ReplaceOpenRecentMenu(openRecentToolStripMenuItem1
                , filepath => OpenFileCSV(filepath)
            );
        }

        /// <summary>
        /// Open file, read and verify data, make data table and run control intialisations.
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        private void OpenFileCSV(string filepath)
        {
            try {
                _datatable = OpenData.ParseFileCSV<RegistryOfficeDataRow>(filepath);
            }
            catch (Exception ex) {
                MessageBox.Show("Error: Could not open file from disk. " + ex.Message);
                return;
            }
            InitOnOpenDiskFile(filepath);
        }

        /// <summary>
        /// Show open file dialog to choose csv file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string filepath = OpenData.OpenFileDialogGetPath();
            if (filepath == null) {
                return;
            }
            Properties.Settings.Default.RecentDirectory = Path.GetDirectoryName(filepath);
            OpenFileCSV(filepath);
        }

        /// <summary>
        /// Clicking on "New" menu entry creates empty datatable with default type.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _datatable = OpenData.EmptyTableFromType<RegistryOfficeDataRow>();
            InitOnCreateNewTable();
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
            if (_datatable == null) {
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
        /// <returns></returns>
        private void SaveFileCSV(string filepath, bool append)
        {
            try {
                if (append) {
                    SaveData.AppendFileCSV(_dataview.ToTable(), filepath);
                }
                else {
                    SaveData.WriteFileCSV(_dataview.ToTable(), filepath);
                }
            }
            catch (Exception ex) {
                MessageBox.Show("Error: Could not write to disk. " + ex.Message);
                return;
            }
        }

        /// <summary>
        /// Save As (new file) button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string filepath = SaveData.SaveFileDialogGetPath();
            if (filepath == null) {
                // user canceled save operation
                return;
            }
            SaveFileCSV(filepath, append: false);
            InitOnSaveDiskFile(filepath);
            // set recent directory
            Properties.Settings.Default.RecentDirectory = Path.GetDirectoryName(filepath);
        }

        /// <summary>
        /// Save To (append) menu entry is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string filepath = SaveData.AppendFileDialogGetPath();
            if (filepath == null) {
                // user canceled save operation
                return;
            }
            SaveFileCSV(filepath, append: true);
            // set recent directory
            Properties.Settings.Default.RecentDirectory = Path.GetDirectoryName(Recent.CurrentlyOpenFilePath);
        }

        /// <summary>
        /// Save (overwrite) menu button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // if user created this file in memory and we don't 
            // know where to save it, run file selection dialog
            if (Recent.CurrentlyOpenFilePath == null)
            {
                Recent.CurrentlyOpenFilePath = SaveData.SaveFileDialogGetPath();
                if (Recent.CurrentlyOpenFilePath == null) {
                    // user canceled save operation
                    return;
                }
            }
            SaveFileCSV(Recent.CurrentlyOpenFilePath, append: false);
            InitOnSaveDiskFile(Recent.CurrentlyOpenFilePath);
        }
    }
}
