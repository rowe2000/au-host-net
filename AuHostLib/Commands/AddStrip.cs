using AuHost.Plugins;

namespace AuHost.Commands
{
    public class AddStrip : Command
    {
        private Strip strip;

        public override bool SaveInScene => true;
        public int RackIndex { get; set; }
        public int RackId { get; set; }
        public int StripIndex { get; set; }
        public int StripId { get; set; }
        public int ZoneId { get; set; }
        public StripType StripType { get; set; }

        public AddStrip(Zone zone, int stripIndex, StripType stripType)
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

            strip = Cache.Instance.Create<Strip>();
            StripId = strip.Id;
            zone.Items.Insert(StripIndex, strip);

            Push(new SelectStrip(strip));

            Push(new AddPlugin(strip, InternalPluginFormat.MidiInDesc, 0));
            Push(new AddPlugin(strip, InternalPluginFormat.AudioOutDesc, strip.Items.Count));

            return base.Execute();
        }

        public override bool Undo()
        {
            PopAll();

            strip.RemoveFromParent();

            return base.Undo();
        }
    }
}