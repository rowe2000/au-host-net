using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace AuHost.Plugins
{
    public interface IContainer : INotifyCollectionChanged, INotifyPropertyChanged, IList
    {
        T GetItem<T>(int index) where T : class;

        bool Remove(IItem item);
        List<T> GetPath<T>() where T : class, IParent;
        int DeepCount<T>();
        T GetItemDeep<T>(int id) where T : class;
        bool HasItems();
        IItem GetLastItem();
        IItem GetFirstItem();
        void Add(IItem item);
        
        void Insert(int index, IItem item);

        void Move(IItem item, int index);

        void AddItems(IEnumerable addItems);
        void Move(int fromIndex, IContainer newOwner);

        void Move(int index, int newIndex);
        void FixIndices();
    }
}