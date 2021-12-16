using AuHost.Models;
using AuHost.Plugins;

namespace AuHost.Commands
{
    public class AddStrip : Command
    {
        public Strip NewStrip { get; private set; }

        public override bool SaveInScene => true;
        public int RackIndex { get; set; }
        public int RackId { get; set; }
        public int StripIndex { get; set; }
        public int StripId { get; set; }
        public int ZoneId { get; set; }
        public StripType StripType { get; set; }

        public AddStrip(Zone zone, StripType stripType, int stripIndex = -1)
        {
            StripId = Cache.Instance.GetNextId();
            ZoneId = zone.Id;
            StripIndex = stripIndex;
            StripType = stripType;
        }

        public override bool Execute()
        {
            var zone = Cache.Instance.GetItem<Zone>(ZoneId);
            if (zone == null)
                return false;

            NewStrip = Cache.Instance.CreateWithId<Strip>(StripId);
            var stripIndex = StripIndex < 0 ? Items.Count : StripIndex;
            zone.Items.Insert(stripIndex, NewStrip);

            Push(new SelectStrip(NewStrip));

            // Push(new AddPlugin(strip, InternalPluginFormat.MidiInDesc, 0));
            // Push(new AddPlugin(strip, InternalPluginFormat.AudioOutDesc, strip.Items.Count));

            return base.Execute();
        }

        public override bool Undo()
        {
            PopAll();

            NewStrip.RemoveFromParent();

            return base.Undo();
        }
    }
}