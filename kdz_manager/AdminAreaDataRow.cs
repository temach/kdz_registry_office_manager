using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kdz_manager
{
    /// <summary>
    /// агрегатов ряд книг под именем автора.
    /// </summary>
    class AdminAreaDataRow : IFromMapDataRow<AdminAreaDataRow>
    {
        public string AdmArea { get; set; }
        public string AdmAreaCode { get; set; }
        public List<RegistryOfficeDataRow> Offices { get; set; }

        /// <summary>
        /// Возвращает количество различных цен на книги.
        /// (например, если три книги имеют ту же цену они считаться одним )
        /// </summary>
        /// <returns></returns>
        public int GetQtyOfDistinctBookPrices()
        {
            return Offices.GroupBy(o => o.District).Select(g => g.Key).Count();
        }

        /// <summary>
        /// Беспараметрическая конструктор необходимо для использования в CSV парсер.
        /// </summary>
        public AdminAreaDataRow()
        {
            AdmArea = string.Empty;
            AdmAreaCode = string.Empty;
            Offices = new List<RegistryOfficeDataRow>();
        }

        /// <summary>
        /// Построить автора от карт DataRow
        /// </summary>
        /// <param name="input"></param>
        public AdminAreaDataRow(MapDataRow input)
            : this()
        {
            AdmAreaCode = input.AdmAreaCode;
            AdmArea = input.AdmArea;
        }

        /// <summary>
        /// Создать новый экземпляр из разобранного данных
        /// </summary>
        /// <returns></returns>
        public AdminAreaDataRow FromMapDataRow(MapDataRow input)
        {
            return new AdminAreaDataRow(input);
        }

    }
}
