namespace ReactiveUI.Contrib
{
    using System.Collections.Generic;
    using System.Collections.Specialized;

    public interface IObservableCollection<T> : ICollection<T>, INotifyCollectionChanged
    {
    }
}