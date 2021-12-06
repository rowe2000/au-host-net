using AuHost.Plugins;

namespace AuHost.Commands
{
    public class RemoveConnection : Command
    {
        public override bool SaveInScene => false;
        public int DstChannel { get; set; }
        public int SrcChannel { get; set; }
        public int DstPluginId { get; set; }
        public int SrcPluginId { get; set; }

        public RemoveConnection(Plugin srcPlugin, int srcChannel, Plugin dstPlugin, int dstChannel)
        {
            SrcPluginId = srcPlugin.Id;
            DstPluginId = dstPlugin.Id;
            SrcChannel = srcChannel;
            DstChannel = dstChannel;
        }

        public override bool Execute()
        {
            var pluginGraph = PluginGraph.Instance;
            
            var srcPlugin = Cache.Instance.GetItem<Plugin>(SrcPluginId);
            var dstPlugin = Cache.Instance.GetItem<Plugin>(DstPluginId);
            
            pluginGraph.RemoveConnection(srcPlugin, SrcChannel, dstPlugin, DstChannel);

            return base.Execute();
        }

        public override bool Undo()
        {
            var pluginGraph = PluginGraph.Instance;
            
            var srcPlugin = Cache.Instance.GetItem<Plugin>(SrcPluginId);
            var dstPlugin = Cache.Instance.GetItem<Plugin>(DstPluginId);
            
            pluginGraph.AddConnection(srcPlugin, SrcChannel, dstPlugin, DstChannel);

            return base.Undo();
        }
    }
}