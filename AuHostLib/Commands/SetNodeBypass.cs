using AuHost.Models;
using AuHost.Plugins;

namespace AuHost.Commands
{
    public class SetNodeBypass : Command
    {
        private Plugin plugin;
        private bool undoState;
        
        public override bool SaveInScene => true;
        public int PluginId { get; set; }
        public bool Bypassed { get; set; }

        public SetNodeBypass(Plugin plugin, bool bypassed)
        {
            PluginId = plugin.Id;
            Bypassed = bypassed;
        }

        public override bool Execute()
        {
            var pluginGraph = PluginGraph.Instance;
            
            plugin = Cache.Instance.GetItem<Plugin>(PluginId);
            undoState = plugin.IsBypassed;

            if (undoState != Bypassed)
                plugin.IsBypassed = Bypassed;

            return base.Execute();
        }

        public override bool Undo()
        {
            plugin.IsBypassed = undoState;

            return base.Undo();
        }
    }
}