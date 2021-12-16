using System.Windows.Input;
using AuHost.Models;
using AuHost.Plugins;

namespace AuHost.Commands
{
    public class AddPlugin : Command
    {
        private Strip strip;

        public override bool SaveInScene => true;
        public int PluginIndex { get; set; }
        public string PluginName { get; set; }
        public int StripId { get; set; }

        public Plugin Plugin { get; private set; }

        public AddPlugin(Strip strip, string pluginName, int pluginIndex = -1)
        {
            StripId = strip.Id;
            PluginName = pluginName;
            PluginIndex = pluginIndex;
        }

        public override bool Execute()
        {
            if (PluginName == null)
                return false;
            
            strip = Cache.Instance.GetItem<Strip>(StripId);
            Plugin = Cache.Instance.Create<Plugin>(PluginName);
            Plugin.Index = PluginIndex < 0  ? strip.Items.Count : PluginIndex;
            Plugin.Activate(strip);

            return base.Execute();
        }

        public override bool Undo()
        {
            if (Plugin is null)
                return false;
            
            strip.Items.Remove(Plugin);

            return base.Undo();
        }
    }
}