// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Collections.Specialized;
// using System.Linq;
//
// namespace AuHost.Plugins
// {
//     public sealed class ItemList<T> : IList<T>, IItemList<T> where T : class, IItem
//     {
//         public ItemList(IContainer owner)
//         {
//             this.owner = owner;
//         }
//
//         private readonly IContainer owner;
//         private readonly List<T> list = new List<T>();
//         
//         public event Action<ChangedEvent<T>> Changed;
//         public IEnumerator<T> GetEnumerator() => list.GetEnumerator();
//
//         IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
//
//         public void Add(T item) => AddRange(item.ToMany());
//
//         public void AddRange(IEnumerable<T> items)
//         {
//             var index = list.Count;
//             var array = items.Where(o => o != null).ToArray();
//             foreach (var item in array)
//             {
//                 item.Parent = owner;
//                 item.Index = index;
//             }
//             
//             list.AddRange(array);
//             OnChanged(new ChangedEvent<T>(NotifyCollectionChangedAction.Add, array, index));
//         }
//
//         public void Clear()
//         {
//             foreach (var item in list)
//             {
//                 item.Parent = null;
//                 item.Index = -1;
//             }
//
//             list.Clear();
//             OnChanged(new ChangedEvent<T>(NotifyCollectionChangedAction.Reset, null, null));
//         }
//
//         public bool Contains(T item) => list.Contains(item);
//
//         public void CopyTo(T[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);
//
//         public bool Remove(T item)
//         {
//             if (item == null) 
//                 return false;
//             
//             item.Parent = null;
//             item.Index = -1;
//             
//             var removed = list.Remove(item);
//             if (removed)
//                 OnChanged(new ChangedEvent<T>(NotifyCollectionChangedAction.Remove, item.ToMany(), null));
//             
//             return removed;
//         }
//
//         public int Count => list.Count;
//         public bool IsReadOnly => false;
//         public int IndexOf(T item) => list.IndexOf(item);
//
//         public void Insert(int index, T item) => InsertRange(index, item.ToMany());
//
//         public void InsertRange(int index, IEnumerable<T> items)
//         {
//             var array = items.Where(o => o != null).ToArray();
//             
//             foreach (var item in array)
//             {
//                 item.Parent = owner;
//                 item.Index = index;
//             }
//
//             list.InsertRange(index, array);
//             
//             OnChanged(new ChangedEvent<T>(NotifyCollectionChangedAction.Add, array, index));
//         }
//
//         public void RemoveAt(int index)
//         {
//             if (index < 0 && index >= list.Count)
//                 throw new ArgumentOutOfRangeException();
//             
//             var item = list[index];
//             item.Parent = null;
//             item.Index = -1;
//             
//             list.RemoveAt(index);
//             OnChanged(new ChangedEvent<T>(NotifyCollectionChangedAction.Remove, item.ToMany(), index));
//         }
//
//         public void RemoveRange(IEnumerable<T> items)
//         {
//             var array = items.Where(o => o != null).ToArray();
//             
//             foreach (var item in array)
//             {
//                 item.Parent = null;
//                 item.Index = -1;
//                 list.Remove(item);
//             }
//
//             OnChanged(new ChangedEvent<T>(ChangedEnum.Removed, array, null));
//         }
//
//         public void RemoveRange(int index, int count)
//         {
//             if (index < 0 && count < 0 && index + count >= list.Count)
//                 throw new ArgumentOutOfRangeException();
//             
//             var items = list.Skip(index).Take(count).ToArray();
//             foreach (var item in items)
//             {
//                 item.Parent = null;
//                 item.Index = -1;
//             }
//             
//             list.RemoveRange(index, count);
//             OnChanged(new ChangedEvent<T>(ChangedEnum.Removed, items, index));
//         }
//
//         public T this[int index]
//         {
//             get => list[index];
//             set
//             {
//                 list[index].Index = -1;
//                 list[index].Parent = null;
//                 list[index] = value;
//                 value.Index = index;
//                 value.Parent = owner;
//             }
//         }
//
//         public void Move(int index, int newIndex)
//         {
//             var item = list[index];
//
//             list.RemoveAt(index);
//             list.Insert(index, item);
//             item.Index = newIndex;
//
//             OnChanged(new ChangedEvent<T>(ChangedEnum.Moved, item.ToMany(), index));
//         }
//
//         private void OnChanged(ChangedEvent<T> obj)
//         {
//             Changed?.Invoke(obj);
//         }
//         
//         public void FixIndices()
//         {
//             for (var i = 0; i < list.Count; i++) 
//                 list[i].Index = i;
//         }
//     }
// }