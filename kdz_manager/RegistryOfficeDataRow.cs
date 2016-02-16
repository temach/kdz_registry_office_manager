using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kdz_manager
{
    class RegistryOfficeDataRow
    {
        public DateTime REVIEW_DATE { get; set; }
        public string AUTHOR { get; set; }
        public string ISBN { get; set; }
        public decimal DISCOUNTED_PRICE { get; set; }

        public RegistryOfficeDataRow()
        {
            REVIEW_DATE = DateTime.Today;
            AUTHOR = "Author";
            ISBN = new string('0', 12);
        	DISCOUNTED_PRICE = 0;
        }

        public  RegistryOfficeDataRow(DateTime review_date, string author, string isbn, int discounted_price)
        {
            REVIEW_DATE = review_date;
            AUTHOR = author;
            ISBN = isbn;
        	DISCOUNTED_PRICE = discounted_price;
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
