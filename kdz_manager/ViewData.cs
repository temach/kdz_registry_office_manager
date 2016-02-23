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
    class ViewData
    {
        private NumericUpDown _current_page;
        private NumericUpDown _row_per_page;

        private DataTable _tableofdata;
        public DataTable TableOfData
        {
            get { return _tableofdata; }
            set {
                _tableofdata = value;
                ViewOfData = new DataView(_tableofdata);
            }
        }

        public DataView ViewOfData;

        /// <summary>
        /// Get total number of rows that we have (after filtering and sorting on the datatable)
        /// </summary>
        public int TotalRows
        {
            get { return (TableOfData == null) ? 0 : TableOfData.Rows.Count; }
        }

        /// <summary>
        /// Get the total number of pages
        /// </summary>
        public int TotalPages
        {
            get { return (TableOfData == null) ? 0 : (TotalRows/RowsPerPage + 1); }
        }

        /// <summary>
        /// Get number of records after filters have been applied.
        /// </summary>
        public int TotalFilteredRows
        {
            get { return (ViewOfData == null) ? 0 : ViewOfData.Count; }
        }

        /// <summary>
        /// Get number of pages full of records after filters have been applied.
        /// </summary>
        public int TotalFilteredPages
        {
            get { return (ViewOfData == null) ? 0 : (TotalFilteredRows/RowsPerPage + 1); }
        }

        /// <summary>
        /// Get set number of records per page to show in dataGridView1
        /// </summary>
        public int RowsPerPage
        {
            get { return (int)_row_per_page.Value;  }
        }

        /// <summary>
        /// Get set index of current page to display in dataGridView1
        /// </summary>
        public int CurrentPage
        {
            get { return (int)_current_page.Value;  }
        }

        public ViewData(NumericUpDown current_page, NumericUpDown rows_per_page)
        {
            _current_page = current_page;
            _row_per_page = rows_per_page;
            // set sane defaults
            _current_page.Maximum = int.MaxValue;
            _current_page.Minimum = 0;
            _current_page.Value = 0;
            _row_per_page.Maximum = int.MaxValue;
            _row_per_page.Minimum = 1;
            _row_per_page.Value = Properties.Settings.Default.RowsPerPage;
        }

        /// <summary>
        /// Useful so user can supply their match exactly. (we escape wierd characters)
        /// </summary>
        /// <param name="valueWithoutWildcards"></param>
        /// <returns></returns>
        public string EscapeLikeFilterValue(string valueWithoutWildcards)
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
        /// Create a basic filter based on column name and string to match.
        /// </summary>
        /// <param name="column"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public string MakeFilter(string column, string match)
        {
            column = EscapeLikeFilterValue(column);
            match = EscapeLikeFilterValue(match);
            return string.Format(@" ({0} like '*{1}*') ", column, match);
        }

        /// <summary>
        /// Apply user submitted query to data table rows.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="combining_operation">"how to combine the filter with previous: AND/OR"</param>
        public void AddFilter(string filter)
        {
            ViewOfData.RowFilter = filter;
            RePageViewOfData();
        }

        /// <summary>
        /// Remove all filters
        /// </summary>
        public void DropFilters()
        {
            ViewOfData.RowFilter = string.Empty;
            RePageViewOfData();
        }

        /// <summary>
        /// Called when we change to another page or change number rows per page.
        /// Takes the filters and sorting into account.
        /// </summary>
        public void RePageViewOfData()
        {
            if (ViewOfData == null) {
                return;
            }
            string filter = ViewOfData.RowFilter;
            string sort = ViewOfData.Sort;
            IEnumerable<DataRow> toshow = TableOfData.Select(ViewOfData.RowFilter, ViewOfData.Sort)
                .Skip(CurrentPage * RowsPerPage)
                .Take(RowsPerPage);
            ViewOfData = new DataView(toshow.Count() > 0 ? toshow.CopyToDataTable() : TableOfData.Clone());
            ViewOfData.RowFilter = filter;
            ViewOfData.Sort = sort;
        }

    }
}
