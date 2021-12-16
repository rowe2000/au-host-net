using AuHost.Plugins;

namespace AuHost.Models
{
    public class Preset : Storable<PresetRef, PresetLibrary>
    {
    }

    public class PresetRef : Item, IItem
    {
        public IContainer Items { get; }

        public new IParent Parent
        {
            get => base.Parent;
            protected set => base.Parent = value;
        }

        IParent IItem.Parent
        {
            get => Parent;
            set => Parent = value;
        }

        public T GetNextSibling<T>() where T : class
        {
            var index = Index + 1;
            return index < 0 || Items.Count <= index ? null : ((Preset)Parent).Items[index] as T;
        }

        public T GetPreviousSibling<T>() where T : class
        {
            var index = Index - 1;
            return index < 0 || Items.Count <= index ? null : ((Preset)Parent).Items[index] as T;
        }

        public IItem GetPreviousDeep()
        {
            return (IItem) GetPreviousSibling<PresetRef>() ?? Parent as Preset;
        }

        public IItem GetNextDeep()
        {
            return (IItem) GetNextSibling<PresetRef>() ?? (Parent as Preset)?.GetNextSibling<Preset>();
        }
    }
}