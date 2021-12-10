using AuHost.Models;
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

        public int PluginId { get; }

        public override bool Execute()
        {
            var pluginGraph = PluginGraph.Instance;
            
            var connections = Cache.Instance.GetItem<Plugin>(PluginId).GetConnections();

            foreach (var connection in connections)
            {
                var srcPlugin = pluginGraph.GetPluginByAvAudioUnit(connection.Source.AVAudioUnit);
                var dstPlugin = pluginGraph.GetPluginByAvAudioUnit(connection.Destination.AVAudioUnit);
                var srcChannel = connection.Source.ChannelIndex;
                var dstChannel = connection.Destination.ChannelIndex;

                Push(new RemoveConnection(srcPlugin, srcChannel, dstPlugin, dstChannel));
            }

            return base.Execute();
        }
    }
}