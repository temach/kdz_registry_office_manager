using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kdz_manager
{
    /// <summary>
    /// Maps one row in CSV file to properties.
    /// </summary>
    class MapDataRow : IDeepCopy<MapDataRow>
    {
        public DateTime REVIEW_DATE { get; set; }
        public string AUTHOR { get; set; }
        public string ISBN { get; set; }
        public decimal DISCOUNTED_PRICE { get; set; }

        // Used in DataTable construction
        public int QTY_DISCOUNT_PRICE { get; set; }

        /// <summary>
        /// Parameterless constructor necessary for use in CSV parser.
        /// </summary>
        public  MapDataRow()
        {
            REVIEW_DATE = DateTime.Today;
            AUTHOR = "Author";
            ISBN = new string('0', 12);
        	DISCOUNTED_PRICE = 0;
        }

        /// <summary>
        /// Constructor for making a deep copy of the class.
        /// </summary>
        /// <param name="original">Original object to deep copy from.</param>
        public  MapDataRow(MapDataRow original)
        {
            REVIEW_DATE = original.REVIEW_DATE;
            AUTHOR = original.AUTHOR;
            ISBN = original.ISBN;
        	DISCOUNTED_PRICE = original.DISCOUNTED_PRICE;
        }

        /// <summary>
        /// Implement deep copy interface. Then we have a sure way to dublicate a list of this classes.
        /// </summary>
        /// <returns></returns>
        public  MapDataRow DeepCopy()
        {
            return new  MapDataRow(this);
        }

    }
}
