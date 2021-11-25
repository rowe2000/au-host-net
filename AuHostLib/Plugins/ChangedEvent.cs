using System.Collections.Generic;
using System.Collections.Specialized;

namespace AuHost.Plugins
{
    public class ChangedEvent<T>
    {
        public NotifyCollectionChangedAction ChangeType { get; }
        public IEnumerable<T> Items { get; }
        public int? Index { get; }

        public ChangedEvent(NotifyCollectionChangedAction changeType, T items = default, int? index = null) : this(changeType, items.ToMany(), index)
        {
            
        }

        public ChangedEvent(NotifyCollectionChangedAction changeType, IEnumerable<T> items, int? index = null)
        {
            Items = items;
            Index = index;
            ChangeType = changeType;
        }
    }
}