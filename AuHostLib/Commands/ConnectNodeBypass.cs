using AuHost.Plugins;

namespace AuHost.Commands
{
    public class ConnectNodeBypass : Command
    {
        public override bool SaveInScene => true;
        public ConnectNodeBypass(Plugin plugin)
        {
            PluginId = plugin.Id;
        }

        public int PluginId { get; set; }

        public override bool Execute()
        {
            var plugin = Cache.Instance.GetItem<Plugin>(PluginId);
            Push(new DisconnectNode(plugin));

            var prevNode = plugin.GetPreviousSibling<Plugin>();
            var nextNode = plugin.GetNextSibling<Plugin>();
            if (prevNode != null && nextNode != null)
                Push(new AutoConnectNodes(prevNode, nextNode));

            return base.Execute();
        }

    }
}