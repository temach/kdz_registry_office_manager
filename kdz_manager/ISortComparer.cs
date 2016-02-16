using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace kdz_manager
{
    /// <summary>
    /// Use this to implement Type-Specific Sort Comparer. 
    /// You can create a custom sorter for every type of object that you 
    /// might have sorted in a grid and provide that custom sorter to the BindingList.
    /// Benefits: Supports advanced sorting and comparison of complex data types. Supports 
    /// sorting of types which do not implement IComparable.  
    /// Drawbacks: Must implement a new sort comparer for each data type to be sorted. 
    /// Must maintain the sort comparer as the class changes.  
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISortComparer<T> : IComparer<T>
    {
        PropertyDescriptor SortProperty { get; set; }
        ListSortDirection SortDirection { get; set; }
    }
}
