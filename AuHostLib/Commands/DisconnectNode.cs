using AuHost.Plugins;

namespace AuHost.Commands
{
    public class DisconnectNode : Command
    {
        public override bool SaveInScene => true;
        public DisconnectNode(Plugin plugin)
        {
            PluginId = plugin.Id;
        }

        public int PluginId { get; set; }

        public override bool Execute()
        {
            var plugin = Cache.GetItem<Plugin>(PluginId);
            var rack = plugin.GetParent<Rack>();
            var connections = plugin.GetConnections();

            foreach (var connection in connections)
            {
                var srcPlugin = PluginGraph.GetPluginByAvAudioUnit(connection.Source.AVAudioUnit);
                var dstPlugin = PluginGraph.GetPluginByAvAudioUnit(connection.Destination.AVAudioUnit);
                var srcChannel = connection.Source.ChannelIndex;
                var dstChannel = connection.Destination.ChannelIndex;

                Push(new RemoveConnection(srcPlugin, srcChannel, dstPlugin, dstChannel));
            }

            return base.Execute();
        }

    }
}