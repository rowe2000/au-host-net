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
            var zone = Cache.GetItem<Zone>(ZoneId);
            if (zone == null)
                return false;

            undoSelectedZone = PluginGraph.Instance.SelectedZone;

            PluginGraph.Instance.SelectZone(zone);

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