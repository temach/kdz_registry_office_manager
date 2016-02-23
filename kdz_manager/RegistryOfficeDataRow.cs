using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kdz_manager
{
    /// <summary>
    /// Represents some books.
    /// </summary>
    class RegistryOfficeDataRow : IDeepCopy<RegistryOfficeDataRow>, IFromMapDataRow<RegistryOfficeDataRow>
    {
        public DateTime REVIEW_DATE { get; set; }
        public decimal DISCOUNTED_PRICE { get; set; }

        /// <summary>
        /// Parameterless constructor necessary for use in CSV parser.
        /// </summary>
        public RegistryOfficeDataRow()
        {
            REVIEW_DATE = DateTime.Today;
        	DISCOUNTED_PRICE = 0;
        }

        /// <summary>
        /// Constructor for making a deep copy of the class.
        /// </summary>
        /// <param name="original">Original object to deep copy from.</param>
        public RegistryOfficeDataRow(RegistryOfficeDataRow original)
        {
            REVIEW_DATE = original.REVIEW_DATE;
        	DISCOUNTED_PRICE = original.DISCOUNTED_PRICE;
        }

        /// <summary>
        /// Implement deep copy interface. Then we have a sure way to 
        /// dublicate a list of this classes.
        /// </summary>
        /// <returns></returns>
        public RegistryOfficeDataRow DeepCopy()
        {
            return new RegistryOfficeDataRow(this);
        }

        /// <summary>
        /// Constructor for making a deep copy of the class.
        /// </summary>
        /// <param name="input">Original object to deep copy from.</param>
        public RegistryOfficeDataRow(MapDataRow input)
            : this()
        {
            REVIEW_DATE = input.REVIEW_DATE;
        	DISCOUNTED_PRICE = input.DISCOUNTED_PRICE;
        }

        /// <summary>
        /// Create new instance from parsed data
        /// </summary>
        /// <returns></returns>
        public RegistryOfficeDataRow FromMapDataRow(MapDataRow input)
        {
            return new RegistryOfficeDataRow(input);
        }

    }
}
