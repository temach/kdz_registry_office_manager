using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace kdz_manager
{
    /// <summary>
    /// Generic Sort Comparer.
    /// You can accomplish sorting using some generic code that tries to 
    /// compare the properties using the IComparable interface for the property value.  
    /// Benefits: Reusable, automatically covers any new data types.  
    /// Drawbacks: Does not compare types that do not implement IComparable.
    /// Not as flexible for complex sorting algorithm.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericSortComparer<T> : ISortComparer<T>
    {
        public GenericSortComparer()
        {
        }

        public GenericSortComparer(string sortProperty, ListSortDirection sortDirection)
            : this(TypeDescriptor.GetProperties(typeof(T)).Find(sortProperty, true), sortDirection)
        {
        }

        public GenericSortComparer(PropertyDescriptor sortProperty, ListSortDirection sortDirection)
        {
            this.SortDirection = sortDirection;
            this.SortProperty = sortProperty;
        }

        public PropertyDescriptor SortProperty { get; set; }
        public ListSortDirection SortDirection { get; set; }

        public int Compare(T x, T y)
        {
            if (this.SortProperty == null)
                return 0;

            IComparable obj1 = this.SortProperty.GetValue(x) as IComparable;
            IComparable obj2 = this.SortProperty.GetValue(y) as IComparable;
            if (obj1 == null || obj2 == null)
                return 0;

            if (this.SortDirection == ListSortDirection.Ascending)
                return (obj1.CompareTo(obj2));
            else
                return (obj2.CompareTo(obj1));
        }
    }
}
