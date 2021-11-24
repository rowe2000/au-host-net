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
            var plugin = Cache.GetItem<Plugin>(PluginId);
            Push(new ConnectNodeBypass(plugin));
            Push(new RemovePlugin(plugin));

            return base.Execute();
        }
    }
}