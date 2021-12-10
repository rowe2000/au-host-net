using AuHost.Models;
using AuHost.Plugins;

namespace AuHost.Commands
{
    public class RemoveAndReconnect : Command
    {
        public override bool SaveInScene => false;
        public int PluginId { get; }

        public RemoveAndReconnect(Plugin plugin)
        {
            PluginId = plugin.Id;
        }

        public override bool Execute()
        {
            var pluginGraph = PluginGraph.Instance;
            
            var plugin = Cache.Instance.GetItem<Plugin>(PluginId);
            
            Push(new ConnectNodeBypass(plugin));
            Push(new RemovePlugin(plugin));

            return base.Execute();
        }
    }
}