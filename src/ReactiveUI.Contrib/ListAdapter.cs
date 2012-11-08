using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ReactiveUI.Contrib {
    public interface IListAdapter<in T> {
        void Add(T item);
        void Insert(int index, T item);
        bool Remove(T item);
        void RemoveAt(int index);
        void Clear();
    }

    public abstract class ListAdapter<T> : IListAdapter<T> { 
        protected ListAdapter() {
            
        }

        public static IListAdapter<T> Create(IList<T> list) {
            Contract.Ensures(list != null);
            return new ListAdapterFromList(list);
        } 

        public abstract void Add(T item);
        public abstract void Insert(int index, T item);
        public abstract bool Remove(T item);
        public abstract void RemoveAt(int index);
        public abstract void Clear();

        private class ListAdapterFromList : ListAdapter<T> {
            private readonly IList<T> _list;

            public ListAdapterFromList(IList<T> list) {
                Contract.Ensures(list != null);
                _list = list;
            }

            public override void Add(T item) {
                _list.Add(item);
            }

            public override void Insert(int index, T item) {
                _list.Insert(index, item);
            }

            public override bool Remove(T item) {
                return _list.Remove(item);
            }

            public override void RemoveAt(int index) {
                _list.RemoveAt(index);
            }

            public override void Clear() {
                _list.Clear();
            }
        }
    }
}