using AuHost.Models;
using AuHost.Plugins;

namespace AuHost.Commands
{
    public class AddPlugin : Command
    {
        private Plugin plugin;
        private Strip strip;

        public override bool SaveInScene => true;
        public int PluginIndex { get; set; }
        public string PluginName { get; set; }
        public int StripId { get; set; }

        public AddPlugin(Strip strip, InternalPluginFormat midiInDesc, int pluginIndex)
        {
            StripId = strip.Id;
            PluginIndex = pluginIndex;
        }

        
        public override bool Execute()
        {
            strip = Cache.Instance.GetItem<Strip>(StripId);
            plugin = Cache.Instance.Create<Plugin>(PluginName);
            plugin.Index = PluginIndex;
            plugin.Activate(strip);

            return base.Execute();
        }

        public override bool Undo()
        {
            strip.Items.Remove(plugin);

            return base.Undo();
        }
    }
}