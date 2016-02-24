using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kdz_manager
{
    /// <summary>
    /// Aggregates a number of books under the author's name.
    /// </summary>
    class AdminAreaDataRow : IFromMapDataRow<AdminAreaDataRow>
    {
        public string AdmArea { get; set; }
        public string AdmAreaCode { get; set; }
        public List<RegistryOfficeDataRow> Offices { get; set; }

        /// <summary>
        /// Returns quantity of distinct book prices. 
        /// (e.g. if three books have the same price they count as one)
        /// </summary>
        /// <returns></returns>
        public int GetQtyOfDistinctBookPrices()
        {
            return Offices.GroupBy(o => o.District).Select(g => g.Key).Count();
        }

        /// <summary>
        /// Parameterless constructor necessary for use in CSV parser.
        /// </summary>
        public AdminAreaDataRow()
        {
            AdmArea = string.Empty;
            AdmAreaCode = string.Empty;
            Offices = new List<RegistryOfficeDataRow>();
        }

        /// <summary>
        /// Contruct author from MapDataRow
        /// </summary>
        /// <param name="input"></param>
        public AdminAreaDataRow(MapDataRow input)
            : this()
        {
            AdmAreaCode = input.AdmAreaCode;
            AdmArea = input.AdmArea;
        }

        /// <summary>
        /// Create new instance from parsed data
        /// </summary>
        /// <returns></returns>
        public AdminAreaDataRow FromMapDataRow(MapDataRow input)
        {
            return new AdminAreaDataRow(input);
        }

    }
}
