using System;
using System.Collections.Generic;

namespace AuHost.Plugins
{
    public class Cache
    {
        public static Cache Instance { get; } = new Cache();

        private readonly Dictionary<int, IItem> dict = new Dictionary<int, IItem>();
        private int maxId;

        private int MaxId
        {
            get => maxId;
            set
            {
                maxId = value;
                var doc = PluginGraph.Instance?.Document;
                if (doc != null)
                    doc.MaxId = MaxId;
            }
        }

        public T GetItem<T>(int id) where T : class => dict[id] as T;

        public int GetNextId() => ++MaxId;

        public T Create<T>() where T : IItem => Create<T>(GetNextId());

        public T Create<T>(int id) where T : IItem
        {
            MaxId = Math.Max(MaxId, id);
            var instance = Activator.CreateInstance<T>();
            instance.Id = id;
            dict[instance.Id] = instance;
            return instance;
        }
    }
}