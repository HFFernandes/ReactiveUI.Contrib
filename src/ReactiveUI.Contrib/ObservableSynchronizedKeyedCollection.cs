namespace ReactiveUI.Contrib
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics.Contracts;

    public class ObservableSynchronizedKeyedCollection<TKey, TItem> : SynchronizedKeyedCollection<TKey, TItem>, IObservableCollection<TItem>, INotifyCollectionChanged
    {
        private readonly Func<TItem, TKey> getKeyForItemDelegate;
        private bool deferNotifyCollectionChanged = false;

        // Constructor now requires a delegate to get the key from the item
        public ObservableSynchronizedKeyedCollection(Func<TItem, TKey> getKeyForItemDelegate)
            : base()
        {
            if (getKeyForItemDelegate == null)
            {
                throw new ArgumentNullException("getKeyForItemDelegate");
            }
            Contract.EndContractBlock();

            this.getKeyForItemDelegate = getKeyForItemDelegate;
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void AddRange(IEnumerable<TItem> items)
        {
            try
            {
                this.deferNotifyCollectionChanged = true;
                foreach (var item in items)
                {
                    this.Add(item);
                }
            }
            finally
            {
                this.deferNotifyCollectionChanged = false;
            }

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public virtual bool TryGetValue(TKey key, out TItem item)
        {
            lock (this.SyncRoot)
            {
                var dictionary = this.Dictionary;
                if (dictionary != null)
                {
                    return dictionary.TryGetValue(key, out item);
                }

                var list = (IList<TItem>)this;
                for (int index = 0; index < list.Count; index++)
                {
                    var curr = list[index];
                    var currKey = this.getKeyForItemDelegate(curr);
                    if (object.Equals(key, currKey))
                    {
                        item = curr;
                        return true;
                    }
                }
            }

            item = default(TItem);
            return false;
        }

        protected override TKey GetKeyForItem(TItem item)
        {
            return this.getKeyForItemDelegate(item);
        }

        // Overrides a lot of methods that can cause collection change
        protected override void SetItem(int index, TItem item)
        {
            base.SetItem(index, item);
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, index));
        }

        protected override void InsertItem(int index, TItem item)
        {
            base.InsertItem(index, item);
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected override void RemoveItem(int index)
        {
            TItem item = this[index];
            base.RemoveItem(index);
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.deferNotifyCollectionChanged)
            {
                return;
            }

            var handler = this.CollectionChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}