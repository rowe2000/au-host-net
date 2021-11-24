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
            var srcPlugin = Cache.GetItem<Plugin>(SrcPluginId);
            var dstPlugin = Cache.GetItem<Plugin>(DstPluginId);
            PluginGraph.Instance.AddConnection(srcPlugin, SrcChannel, dstPlugin, DstChannel);

            return base.Execute();
        }

        public override bool Undo()
        {
            var srcPlugin = Cache.GetItem<Plugin>(SrcPluginId);
            var dstPlugin = Cache.GetItem<Plugin>(DstPluginId);
            PluginGraph.Instance.RemoveConnection(srcPlugin, SrcChannel, dstPlugin, DstChannel);

            return base.Execute();
        }

    }
}