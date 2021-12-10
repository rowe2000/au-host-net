using AuHost.Models;
using AuHost.Plugins;

namespace AuHost.Commands
{
    public class AddConnection : Command
    {
        public override bool SaveInScene => true;

        public int SrcPluginId { get; set; }
        public int DstPluginId { get; set; }
        public int SrcChannel { get; set; }
        public int DstChannel { get; set; }

        public AddConnection(Plugin srcPlugin, int srcChannel, Plugin dstPlugin, int dstChannel)
        {
            SrcPluginId = srcPlugin.Id;
        }

        public override bool Execute()
        {
            var pluginGraph = PluginGraph.Instance;
            
            var srcPlugin = Cache.Instance.GetItem<Plugin>(SrcPluginId);
            var dstPlugin = Cache.Instance.GetItem<Plugin>(DstPluginId);
            
            pluginGraph.AddConnection(srcPlugin, SrcChannel, dstPlugin, DstChannel);

            return base.Execute();
        }

        public override bool Undo()
        {
            var pluginGraph = PluginGraph.Instance;
            
            var srcPlugin = Cache.Instance.GetItem<Plugin>(SrcPluginId);
            var dstPlugin = Cache.Instance.GetItem<Plugin>(DstPluginId);
            
            pluginGraph.RemoveConnection(srcPlugin, SrcChannel, dstPlugin, DstChannel);

            return base.Execute();
        }

    }
}