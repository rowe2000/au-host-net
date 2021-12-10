using AuHost.Models;
using AuHost.Plugins;

namespace AuHost.Commands
{
    public class SelectStrip : Command
    {
        private Strip undoStrip;

        public override bool SaveInScene => false;
        public int StripId { get; set; }

        public SelectStrip(Strip strip)
        {
            StripId = strip.Id;
        }
        
        public override bool Execute()
        {
            var pluginGraph = PluginGraph.Instance;
            
            var strip = Cache.Instance.GetItem<Strip>(StripId);
            if (strip == null)
                return false;

            undoStrip = pluginGraph.SelectedStrip;

            pluginGraph.SelectStrip(strip);

            return base.Execute();
        }

        public override bool Undo()
        {
            if (!base.Undo())
                return false;
            
            PluginGraph.Instance.SelectStrip(undoStrip);
            return true;
        }
    }
}