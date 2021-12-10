using AuHost.Plugins;

namespace AuHost.Models
{
    public class PresetLibrary : IParent
    {
        IContainer IParent.Items => Items;
        
        public Container<Preset> Items { get; }
        
        public PresetLibrary()
        {
            Items = new Container<Preset>(this);
        }

    }
}