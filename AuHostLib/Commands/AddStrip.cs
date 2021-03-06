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
            StripId = Document.GetNextId();
            ZoneId = zone.Id;
            StripIndex = stripIndex;
            StripType = stripType;
        }

        public override bool Execute()
        {
            var zone = Cache.GetItem<Zone>(ZoneId);
            if (zone == null)
                return false;

            strip = Cache.Create<Strip>(StripId);
            zone.Insert(strip, StripIndex);

            Push(new SelectStrip(strip));

            Push(new AddPlugin(strip, InternalPluginFormat.MidiInDesc, 0));
            Push(new AddPlugin(strip, InternalPluginFormat.AudioOutDesc, strip.Count));

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