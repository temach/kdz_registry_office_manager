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
    class AdminAreaDataRow : IDeepCopy<AdminAreaDataRow>, IFromMapDataRow<AdminAreaDataRow>
    {
        public string ISBN { get; set; }
        public string AUTHOR { get; set; }
        public List<RegistryOfficeDataRow> BOOKS { get; set; }

        /// <summary>
        /// Returns quantity of distinct book prices. 
        /// (e.g. if three books have the same price they count as one)
        /// </summary>
        /// <returns></returns>
        public int GetQtyOfDistinctBookPrices()
        {
            return BOOKS.GroupBy(o => o.DISCOUNTED_PRICE).Select(g => g.Key).Count();
        }

        /// <summary>
        /// Parameterless constructor necessary for use in CSV parser.
        /// </summary>
        public AdminAreaDataRow()
        {
            ISBN = new string('0', 12);
            AUTHOR = "Author";
            BOOKS = new List<RegistryOfficeDataRow>();
        }

        /// <summary>
        /// Constructor for making a deep copy of the class.
        /// </summary>
        /// <param name="original">Original object to deep copy from.</param>
        public AdminAreaDataRow(AdminAreaDataRow original)
        {
            ISBN = original.ISBN;
            AUTHOR = original.AUTHOR;
            BOOKS = original.BOOKS.Select(b => b.DeepCopy()).ToList();
        }

        /// <summary>
        /// Implement deep copy interface. Then we have a sure way to 
        /// dublicate a list of this classes.
        /// </summary>
        /// <returns></returns>
        public AdminAreaDataRow DeepCopy()
        {
            return new AdminAreaDataRow(this);
        }

        /// <summary>
        /// Contruct author from MapDataRow
        /// </summary>
        /// <param name="input"></param>
        public AdminAreaDataRow(MapDataRow input)
            : this()
        {
            AUTHOR = input.AUTHOR;
            ISBN = input.ISBN;
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
