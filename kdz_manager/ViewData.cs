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
        public DataView PagedViewOfData
        {
            get
            {
                var filter = ViewOfData.RowFilter;
                var sort = ViewOfData.Sort;
                IEnumerable<DataRow> toshow = TableOfData.Select(filter, sort)
                    .Skip(CurrentPage * RowsPerPage)
                    .Take(RowsPerPage);
                var pagedview = new DataView(toshow.Count() > 0 ? toshow.CopyToDataTable() : TableOfData.Clone());
                pagedview.RowFilter = filter;
                pagedview.Sort = sort;
                return pagedview;
            }
        }

        /// <summary>
        /// Получить общее количество строк, которые мы имеем ( после фильтрации и сортировки на DataTable )
        /// </summary>
        public int TotalRows
        {
            get { return (TableOfData == null) ? 0 : TableOfData.Rows.Count; }
        }

        /// <summary>
        /// Получить общее число страниц
        /// </summary>
        public int TotalPages
        {
            get { return (TableOfData == null) ? 0 : (TotalRows/RowsPerPage + 1); }
        }

        /// <summary>
        /// Получить количество записей после применения фильтров.
        /// </summary>
        public int TotalFilteredRows
        {
            get { return (ViewOfData == null) ? 0 : ViewOfData.Count; }
        }

        /// <summary>
        /// Получить количество страниц, полных записей после применения фильтров.
        /// </summary>
        public int TotalFilteredPages
        {
            get { return (ViewOfData == null) ? 0 : (TotalFilteredRows/RowsPerPage + 1); }
        }

        /// <summary>
        /// Получить набор количество записей на странице, чтобы показать в dataGridView1
        /// </summary>
        public int RowsPerPage
        {
            get { return (int)_row_per_page.Value;  }
        }

        /// <summary>
        /// Получить набор индекс текущей страницы для отображения в dataGridView1
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
        /// Полезный так что пользователь может поставить свой матч точно.  ( Мы избегаем странные символы )
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
        /// Создание базовой фильтр, основанный на имени столбца и строки, чтобы соответствовать.
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
        /// Применить введенные пользователем запрос таблицы данных строк.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="combining_operation">"how to combine the filter with previous: AND/OR"</param>
        public void AddFilter(string filter)
        {
            ViewOfData.RowFilter = filter;
        }

        /// <summary>
        /// Удалить все фильтры
        /// </summary>
        public void DropFilters()
        {
            ViewOfData.RowFilter = string.Empty;
        }

        /// <summary>
        /// Вызывается при переходе к другой странице или номер изменения строк на странице.
        /// принимает фильтры и сортировки учетом.
        /// </summary>
        public void RePageViewOfData()
        {
            if (ViewOfData == null) {
                return;
            }
            string filter = ViewOfData.RowFilter;
            string sort = ViewOfData.Sort;
            DataTable tmp = ViewOfData.ToTable();
            TableOfData.Merge(tmp);

            var old = this.TableOfData.AsEnumerable();
            var changed = this.ViewOfData.Table.AsEnumerable();
            var newitems = changed.Except(old);
            foreach (var addrow in newitems)
            {
                TableOfData.Rows.Add(addrow);
            }
            IEnumerable<DataRow> toshow = TableOfData.Select(ViewOfData.RowFilter, ViewOfData.Sort)
                .Skip(CurrentPage * RowsPerPage)
                .Take(RowsPerPage);
            ViewOfData = new DataView(toshow.Count() > 0 ? toshow.CopyToDataTable() : TableOfData.Clone());
            ViewOfData.RowFilter = filter;
            ViewOfData.Sort = sort;
        }

    }
}
