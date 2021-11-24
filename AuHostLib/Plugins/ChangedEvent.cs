using System.Collections.Generic;
using System.Collections.Specialized;

namespace AuHost.Plugins
{
    public class ChangedEvent<T>
    {
        public NotifyCollectionChangedAction ChangeType { get; }
        public IEnumerable<T> Items { get; }
        public int? Index { get; }

        public ChangedEvent(NotifyCollectionChangedAction changeType, IEnumerable<T> items, int? index)
        {
            Items = items;
            Index = index;
            ChangeType = changeType;
        }
    }
}