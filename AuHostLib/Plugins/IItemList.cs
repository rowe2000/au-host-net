using System;
using System.Collections.Generic;

namespace AuHost.Plugins
{
    public interface IItemList<T> where T : class, IItem
    {
        event Action<ChangedEvent<T>> Changed;
        int Count { get; }
        bool IsReadOnly { get; }
        IEnumerator<T> GetEnumerator();
        void Add(T item);
        void AddRange(IEnumerable<T> items);
        void Clear();
        bool Contains(T item);
        void CopyTo(T[] array, int arrayIndex);
        bool Remove(T item);
        int IndexOf(T item);
        void Insert(int index, T item);
        void InsertRange(int index, IEnumerable<T> items);
        void RemoveAt(int index);
        void RemoveRange(IEnumerable<T> items);
        void RemoveRange(int index, int count);
        T this[int index] { get; set; }
        void Move(int index, int newIndex);
        void FixIndices();
    }
}