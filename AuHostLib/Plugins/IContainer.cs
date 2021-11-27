using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace AuHost.Plugins
{
    public interface IContainer : INotifyCollectionChanged, INotifyPropertyChanged, IEnumerable 
    {
        T GetItem<T>(int index) where T : class;

        bool Remove(IItem item);
        List<T> GetPath<T>() where T : class, IContainer;
        int DeepCount<T>();
        T GetItemDeep<T>(int id) where T : class;
        bool HasItems();
        ICacheable GetLastItem();
        ICacheable GetFirstItem();
        void Add(IItem item);
        
        void Insert(IItem item, int index);

        void Move(IItem item, int index);

        void AddItems(IEnumerable addItems);
        void Move(int fromIndex, IContainer newOwner);

        void Clear();
        int Count { get; }
        void Move(int index, int newIndex);
        void FixIndices();
    }
}