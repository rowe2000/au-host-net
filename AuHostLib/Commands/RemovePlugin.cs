using AuHost.Models;
using AuHost.Plugins;

namespace AuHost.Commands
{
    public class RemovePlugin : Command
    {
        private readonly Plugin plugin;
        private Strip strip;
        private int pluginIndex;

        public override bool SaveInScene => true;

        public RemovePlugin(Plugin plugin)
        {
            this.plugin = plugin;
        }

        public override bool Execute()
        {
            Push(new SetNodeBypass(plugin, true));

            strip = plugin.Parent;
            pluginIndex = plugin.Index;
            var rack = strip.GetParent<Rack>();
            
            foreach (var connection in plugin.GetConnections())
            {
                var srcPlugin = PluginGraph.Instance.GetPluginByAvAudioUnit(connection.Source.AVAudioUnit);
                var dstPlugin = PluginGraph.Instance.GetPluginByAvAudioUnit(connection.Destination.AVAudioUnit);
                var srcChannel = connection.Source.ChannelIndex;
                var dstChannel = connection.Destination.ChannelIndex;

                Push(new RemoveConnection(srcPlugin, srcChannel, dstPlugin, dstChannel));
            }

            plugin.RemoveFromParent();

            return base.Execute();
        }

        public override bool Undo()
        {
            plugin.Activate(strip);

            return base.Undo();
        }
    }
}