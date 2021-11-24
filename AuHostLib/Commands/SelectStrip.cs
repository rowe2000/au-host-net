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
            var strip = Cache.GetItem<Strip>(StripId);
            if (strip == null)
                return false;

            undoStrip = PluginGraph.Instance.SelectedStrip;

            PluginGraph.Instance.SelectStrip(strip);

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