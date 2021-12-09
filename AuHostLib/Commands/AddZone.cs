using AuHost.Plugins;

namespace AuHost.Commands
{
    public class AddZone : Command
    {
        public Zone NewZone { get; private set; }

        public override bool SaveInScene => true;

        public int ZoneIndex { get; set; }
        public int RackId { get; set; }
        public int ZoneId { get; set; }

        public AddZone(Rack rack, int zoneIndex)
        {
            RackId = rack.Id;
            ZoneIndex = zoneIndex;
        }

        public override bool Execute()
        {
            var rack = Cache.Instance.GetItem<Rack>(RackId);
            NewZone = Cache.Instance.Create<Zone>();
            ZoneId = NewZone.Id;
            
            rack.Items.Insert(ZoneIndex, NewZone);

            Push(new SelectZone(NewZone));

            return base.Execute();
        }

        public override bool Undo()
        {
            NewZone.RemoveFromParent();

            return base.Undo();
        }
    }
}