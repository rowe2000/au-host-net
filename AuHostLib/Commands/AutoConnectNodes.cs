using System;
using AuHost.Plugins;

namespace AuHost.Commands
{
    public class AutoConnectNodes : Command
    {
        public override bool SaveInScene => true;
        public AutoConnectNodes(Plugin srcPlugin, Plugin dstPlugin)
        {
            SrcPluginId = srcPlugin.Id;
            DstPluginId = dstPlugin.Id;
        }

        public int DstPluginId { get; set; }

        public int SrcPluginId { get; set; }

        public override bool Execute()
        {
            var srcPlugin = Cache.Instance.GetItem<Plugin>(SrcPluginId);
            var dstPlugin = Cache.Instance.GetItem<Plugin>(DstPluginId);

            if (srcPlugin != null && dstPlugin != null)
            {
                // Add midi to from srcNode to dstNode if they produce and accept  midi
                if (srcPlugin.ProducesMidi() && dstPlugin.AcceptsMidi())
                    Push(new AddConnection(srcPlugin, 4096, dstPlugin, 4096));

                // Add audio to from srcNode to dstNode if they produce and accept  audio
                for (var n = 0; n < Math.Min(srcPlugin.GetTotalNumOutputChannels(), dstPlugin.GetTotalNumInputChannels()); n++)
                    Push(new AddConnection(srcPlugin, n, dstPlugin, n));
            }

            return base.Execute();
        }

    }
}