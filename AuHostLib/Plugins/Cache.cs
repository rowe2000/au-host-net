using System;
using System.Collections.Generic;
using System.Linq;
using AuHost.Models;

namespace AuHost.Plugins
{
    public sealed class Cache
    {
        private readonly Document document;
        public static Cache Instance { get; private set; }

        private readonly Dictionary<int, Item> dict = new Dictionary<int, Item>();
        private int maxId;
        public event Action<int> MaxIdChanged;

        private Cache(Document document)
        {
            this.document = document;
            ((IItem)document).Id = 0;
            dict[((IItem)document).Id] = document;
        }

        private int MaxId
        {
            get => maxId;
            set
            {
                maxId = value;
                if (document != null)
                    document.MaxId = maxId;

                OnMaxIdChanged(maxId);
            }
        }

        public T GetItem<T>(int id) where T : class => dict[id] as T;

        public int GetNextId()
        {
            return ++MaxId;
        }

        public T CreateWithId<T>(int id) where T : Item
        {
            var instance = Activator.CreateInstance<T>();
            instance.Id = id;
            dict[instance.Id] = instance;
            return instance;
        }

        public T Create<T>(params object[] args) where T : Item
        {
            var instance = (T)Activator.CreateInstance(typeof(T), args);
            Register(instance);
            return instance;
        }

        private void OnMaxIdChanged(int obj)
        {
            MaxIdChanged?.Invoke(obj);
        }

        public void Register<T>(T instance) where T : Item 
        {
            if (dict.Values.Contains(instance))
                return;

            var id = GetNextId();
            Item.SetId(instance, id);
            dict[instance.Id] = instance;
        }

        public static void Setup(Document document)
        {
            var cache = new Cache(document);
            cache.MaxId = document.MaxId;
            cache.MaxIdChanged += maxId => document.MaxId = maxId;
            Instance = cache;
        }
    }
}