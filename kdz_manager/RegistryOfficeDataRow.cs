using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kdz_manager
{
    class RegistryOfficeDataRow : IDeepCopy<RegistryOfficeDataRow>
    {
        public DateTime REVIEW_DATE { get; set; }
        public string AUTHOR { get; set; }
        public string ISBN { get; set; }
        public decimal DISCOUNTED_PRICE { get; set; }

        /// <summary>
        /// Parameterless constructor necessary for use in CSV parser.
        /// </summary>
        public RegistryOfficeDataRow()
        {
            REVIEW_DATE = DateTime.Today;
            AUTHOR = "Author";
            ISBN = new string('0', 12);
        	DISCOUNTED_PRICE = 0;
        }

        public RegistryOfficeDataRow(DateTime review_date, string author, string isbn, int discounted_price)
        {
            REVIEW_DATE = review_date;
            AUTHOR = author;
            ISBN = isbn;
        	DISCOUNTED_PRICE = discounted_price;
        }

        /// <summary>
        /// Constructor for making a deep copy of the class.
        /// </summary>
        /// <param name="original">Original object to deep copy from.</param>
        public RegistryOfficeDataRow(RegistryOfficeDataRow original)
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
        public RegistryOfficeDataRow DeepCopy()
        {
            return new RegistryOfficeDataRow(this);
        }

        public override string ToString()
        {
            return "On: " + REVIEW_DATE + "\n" 
                + ", Mr./Miss " + AUTHOR + "\n" 
                + "ISBN:" + ISBN + "\n" 
                + "Discounted Price: " + DISCOUNTED_PRICE;
        }
    }
}
