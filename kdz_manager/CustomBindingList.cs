using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;

namespace kdz_manager
{
    /// <summary>
    /// You can implement sorting and the DataGridView will take care of the rest. 
    /// Let me briefly explain how the DataGridView sorting works in relation to the BindingList. 
    /// The DataGridView calls the BindingList.ApplySortCore method, passing a 
    /// PropertyDescriptor for the sort column and the sort direction. Internally, you’ll 
    /// use the List<T>.Sort() method to handle the sort algorithm, but it is up to you for the 
    /// item comparison logic. The DataGridView will then display the sorted data.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CustomBindingList<T> : BindingList<T>
    {
        private bool isSorting;

        /// <summary>
        /// Raised when the list is sorted.
        /// </summary>
        public event EventHandler Sorted;

        public CustomBindingList()
            : this(null)
        { }

        public CustomBindingList(IEnumerable<T> contents)
            : this(contents, null)
        { }

        public CustomBindingList(IEnumerable<T> contents, ISortComparer<T> comparer)
        {
            if (contents != null)
                AddRange(contents);

            if (comparer == null)
                SortComparer = new GenericSortComparer<T>();
            else
                SortComparer = comparer;
        }

        #region Properties
        private ISortComparer<T> sortComparer;
        public ISortComparer<T> SortComparer
        {
            get { return sortComparer; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("SortComparer", "Value cannot be null.");
                sortComparer = value;
            }
        }

        private bool isSorted;
        protected override bool IsSortedCore
        {
            get { return isSorted; }
        }

        /// <summary>
        /// When bound to a BindingList, the DataGridView checks the BindingList.SupportsSortingCore 
        /// property. This is normally false, but if you create your own BindingList 
        /// (using inheritance) and override the sort-specific properties, you can 
        /// implement sorting and the DataGridView will take care of the rest.
        /// </summary>
        protected override bool SupportsSortingCore
        {
            get { return true; }
        }

        private ListSortDirection sortDirection;
        protected override ListSortDirection SortDirectionCore
        {
            get { return sortDirection; }
        }

        private PropertyDescriptor sortProperty;
        protected override PropertyDescriptor SortPropertyCore
        {
            get { return sortProperty; }
        }
        #endregion

        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            if (prop == null)
                return;

            isSorting = true;
            sortDirection = direction;
            sortProperty = prop;
            this.SortComparer.SortProperty = prop;
            this.SortComparer.SortDirection = direction;
            // supress events while sorting
            bool oldRaise = RaiseListChangedEvents;
            RaiseListChangedEvents = false;
            ((List<T>)this.Items).Sort(this.SortComparer);
            isSorted = true;
            isSorting = false;
            ResetBindings(); // raises ListChanged event with Reset enum.
            OnSorted(null, new EventArgs());
            RaiseListChangedEvents = oldRaise;
        }

        protected override void RemoveSortCore()
        {
            throw new NotSupportedException();
        }

        protected override void OnListChanged(ListChangedEventArgs e)
        {
            if (!isSorting)
                base.OnListChanged(e);
        }

        protected virtual void OnSorted(object sender, EventArgs e)
        {
            if (Sorted != null)
                Sorted(sender, e);
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            if (!isSorting)
                this.ApplySortCore(this.SortPropertyCore, this.SortDirectionCore);
        }

        protected override void SetItem(int index, T item)
        {
            base.SetItem(index, item);
            if (!isSorting)
                this.ApplySortCore(this.SortPropertyCore, this.SortDirectionCore);
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            if (!isSorting)
                this.ApplySortCore(this.SortPropertyCore, this.SortDirectionCore);
        }

        protected override void ClearItems()
        {
            base.ClearItems();
        }

        public void AddRange(IEnumerable<T> items)
        {
            bool oldRaise = RaiseListChangedEvents;
            RaiseListChangedEvents = false;
            try
            {
                ClearItems();
                if (items != null)
                    foreach (T item in items)
                        this.Items.Add(item);
            }
            finally
            {
                RaiseListChangedEvents = oldRaise;
                ResetBindings();
            }
        }
    }
}
