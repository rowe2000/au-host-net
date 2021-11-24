using System;
using System.Collections.Generic;

namespace AuHost.Plugins
{
    public class Cache
    {
        private static readonly Dictionary<int, IItem> dict = new Dictionary<int, IItem>();
        public static T GetItem<T>(int id) where T : class
        {
            return dict[id] as T;
        }

        public static T Create<T>() where T : IItem
        {
            return Create<T>(Document.GetNextId());
        }
        public static T Create<T>(int id) where T : IItem
        {
            var instance = Activator.CreateInstance<T>();
            instance.Id = id;
            dict[instance.Id] = instance;
            return instance;
        }
    }
}