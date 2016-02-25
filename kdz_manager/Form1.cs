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

        RecentFilesFolders Recent = new RecentFilesFolders();
        EditRowForm Editor = new EditRowForm(typeof(MapDataRow));
        ViewData View;
        OpenData Open = new OpenData();

        public MainForm()
        {
            InitializeComponent();

            this.comboBox_FilterOperation.SelectedIndex = 0;

            View = new ViewData(this.numericUpDown_CurrentPage, this.numericUpDown_RowsPerPage);

            Recent.CurrentlyOpenFilePathChanged
                += (new_filepath) => this.Text = "CSV Manager current file: " + new_filepath;

            RefreshOpenRecentMenu();
        }


        /// <summary>
        /// от нагрузки файловой отрегулировать некоторые границы и сброса изменяемые значения пользователя.
        /// </summary>
        private void CalculateFileStats()
        {
            this.toolStripStatusLabel_TotalPages.Text = View.TotalPages.ToString() + "\t";
            this.toolStripStatusLabel_TotalRows.Text = View.TotalRows.ToString() + "\t";
            this.toolStripStatusLabel_CurrentFilteredPages.Text = View.TotalFilteredPages.ToString() + "\t";
            this.toolStripStatusLabel_CurrentFilteredRows.Text = View.TotalFilteredRows.ToString() + "\t";
            this.toolStripStatusLabel_CurrentSortColumn.Text = View.ViewOfData.Sort + "\t";
        }

        private void InitOnCreateNewTable()
        {
            // clear path so its not added to open recent
            Recent.CurrentlyOpenFilePath = null;
            // reset page and items per page
            this.numericUpDown_RowsPerPage.Value = Properties.Settings.Default.RowsPerPage;
            this.numericUpDown_CurrentPage.Value = 0;
            // assign to datagrid
            this.dataGridView1.DataSource = View.ViewOfData;
            RefreshDataGridViewPager();
            // update status bar (to show that the new table is empty)
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
            this.dataGridView1.DataSource = View.ViewOfData;
            RefreshDataGridViewPager();
            // update status bar
            CalculateFileStats();
        }

        /// <summary>
        /// Получить элементы, чтобы показать в открытом недавно меню
        /// </summary>
        private void RefreshOpenRecentMenu()
        {
            // just replace old menu item wth a new one to refresh it
            Recent.ReplaceOpenRecentMenu(openRecentToolStripMenuItem1
                , filepath => OpenFileCSV(filepath)
            );
        }

        /// <summary>
        /// Пользователь нажал на колонке для сортировки по ним.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            CalculateFileStats();
        }

        /// <summary>
        /// Открыть файл, читать и проверять данные, сделать таблицу данных и запустить контроля инициализацый.
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        private void OpenFileCSV(string filepath)
        {
            try {
                Open.ParseAsMapDataCSV(filepath);
                Open.ImportProcessing();
                View.TableOfData = Open.ToDataTable(Open.Raw);
                // Open.AddQtyOrRegionsPerAreaColumn(View.TableOfData);
            }
            catch (Exception ex) {
                MessageBox.Show("Error: Could not open file from disk. " + ex.Message);
                return;
            }
            InitOnOpenDiskFile(filepath);
        }

        /// <summary>
        /// Показать выбирать CSV файл.
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
        /// нажатия на пункт меню"New"создает пустую DataTable с типом по умолчанию.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dummy = new List<MapDataRow>();
            View.TableOfData = Open.ToDataTable(dummy);
            InitOnCreateNewTable();
        }


        /// <summary>
        /// очищает вид сетки внутренние фильтры и обновлений.
        /// не стирает строки, которые пользователь ввел в фильтр текстовых полях.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_ClearFilters_Click(object sender, EventArgs e)
        {
            View.DropFilters();
            RefreshDataGridViewPager();
            CalculateFileStats();
        }

        /// <summary>
        /// Применить введенные пользователем запрос таблицы данных строк.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_SubmitFilter_Click(object sender, EventArgs e)
        {
            View.DropFilters();
            string combine = this.comboBox_FilterOperation.SelectedItem.ToString();
            string area_name = this.textBox_FilterAdmAreaName.Text;
            string area_code = this.textBox_FilterAdmAreaCode.Text;
            string filter = View.MakeFilter(Open.FilterAdmArea, area_name) 
                + combine + View.MakeFilter(Open.FilterAdmAreaCode, area_code);
            View.AddFilter(filter);
            CalculateFileStats();
        }

        /// <summary>
        /// Вызывается при переходе к другой странице или изменения номера строк на странице.
        /// принимает во внимание фильтры.
        /// 
        /// Это обработчик событий для NumericUpDown.... ValueChanged событий
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshDataGridViewPager(object sender=null, EventArgs e = null)
        {
            if (View == null) {
                return;
            }
            // start crazy hack to set all rows to invisible
            CurrencyManager currencyManager1 = (CurrencyManager)BindingContext[this.dataGridView1.DataSource];
            currencyManager1.SuspendBinding();
            dataGridView1.CurrentCell = null;
            foreach (DataGridViewRow row  in this.dataGridView1.Rows)
            {
                row.Visible = false;
            }
            currencyManager1.ResumeBinding();
            // crazy hack end
            var makevisible = this.dataGridView1.Rows
                .Cast<DataGridViewRow>()
                .Skip(View.CurrentPage * View.RowsPerPage)
                .Take(View.RowsPerPage);
            foreach (DataGridViewRow r in makevisible)
            {
                r.Visible = true;
            }
        }

        /// <summary>
        /// Написать DataTable из памяти в файл.
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        private void SaveFileCSV(string filepath, bool append)
        {
            try {
                if (append) {
                    SaveData.AppendFileCSV<MapDataRow>(View.ViewOfData.ToTable(), filepath);
                }
                else {
                    SaveData.WriteFileCSV<MapDataRow>(View.ViewOfData.ToTable(), filepath);
                }
            }
            catch (Exception ex) {
                MessageBox.Show("Error: Could not write to disk. " + ex.Message);
                return;
            }
        }

        /// <summary>
        /// Сохранить как ( Новый файл ) нажатие кнопки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (View.TableOfData == null)
            {
                return;
            }
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
        /// сохранения файла ( добавление) пункт меню кнопки.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (View.TableOfData == null)
            {
                return;
            }
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
        /// Сохранить ( перезаписи ) Кнопка меню нажмите
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (View.TableOfData == null)
            {
                return;
            }
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

        /// <summary>
        /// Сохранить настройки пользователя при выходе.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Запускает форму для редактирования строки.
        /// </summary>
        /// <param name="source"></param>
        private void EditRow(DataRowView source)
        {
            source.BeginEdit();
            Editor.ReBindControlsToDataRow(source);
            if (Editor.ShowDialog() == DialogResult.OK)
            {
                source.EndEdit();
            }
            else
            {
                source.CancelEdit();
            }
        }

        /// <summary>
        /// Добавить новую запись
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (View.ViewOfData == null) {
                return;
            }
            DataRowView source = View.ViewOfData.AddNew();
            EditRow(source);
            this.dataGridView1.Invalidate();
            RefreshDataGridViewPager();
        }

        /// <summary>
        /// Редактировать существующая запись
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (View.ViewOfData == null) {
                return;
            }
            DataRowView source = (DataRowView)this.dataGridView1.CurrentRow.DataBoundItem;
            EditRow(source);
            this.dataGridView1.Invalidate();
        }

        /// <summary>
        /// Удалить строку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            if (View.ViewOfData == null) {
                return;
            }
            ((DataRowView)this.dataGridView1.CurrentRow.DataBoundItem).Delete();
            this.dataGridView1.Invalidate();
            RefreshDataGridViewPager();
        }

        private void button_ApplyAdvancedFilter_Click(object sender, EventArgs e)
        {
            if (View.ViewOfData == null) {
                return;
            }
            try
            {
                View.ViewOfData.RowFilter = this.comboBox_AdvancedFilter.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Invalid advanced filter (see 
http://www.csharp-examples.net/dataview-rowfilter/ for 
details and filter dropdown for exmaples): \n" + ex.Message);
            }
        }

    }
}
