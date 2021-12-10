using AuHost.Models;
using AuHost.Plugins;

namespace AuHost.Commands
{
    public class SelectZone : Command
    {
        private Zone undoSelectedZone;

        public override bool SaveInScene => false;
        public int ZoneId { get; set; }

        public SelectZone(Zone zone)
        {
            ZoneId = zone.Id;
        }

        public override bool Execute()
        {
            var pluginGraph = PluginGraph.Instance;
            
            var zone = Cache.Instance.GetItem<Zone>(ZoneId);
            if (zone == null)
                return false;

            undoSelectedZone = pluginGraph.SelectedZone;

            pluginGraph.SelectZone(zone);

            return base.Execute();
        }

        public override bool Undo()
        {
            if (base.Undo())
            {
                PluginGraph.Instance.SelectZone(undoSelectedZone);
                return true;
            }

            return false;
        }

    }
}