using AuHost.Plugins;

namespace AuHost.Commands
{
    public class RemoveZone : Command
    {
        private Zone zone;
        private Rack rack;
        private int zoneIndex;
        public override bool SaveInScene => true;
        public int ZoneId { get; set; }
        public RemoveZone(Zone zone)
        {
            ZoneId = zone.Id;
        }

        public override bool Execute()
        {
            zone = Cache.Instance.GetItem<Zone>(ZoneId);
            rack = zone.Parent;
            zoneIndex = zone.Index;
            zone.RemoveFromParent();

            return base.Execute();
        }

        public override bool Undo()
        {
            rack.Insert(zone, zoneIndex);
            zone = null;

            return base.Undo();
        }
    }
}