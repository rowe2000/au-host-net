using AuHost.Plugins;

namespace AuHost.Commands
{
    public class RemoveStrip : Command
    {
        private Strip strip;
        private int stripIndex;
        private Zone zone;

        public override bool SaveInScene => true;
        public int StripId { get; set; }

        public override bool Execute()
        {
            strip = Cache.GetItem<Strip>(StripId);
            stripIndex = strip.Index;
            zone = strip.Parent;
            strip.RemoveFromParent();

            return base.Execute();
        }

        public override bool Undo()
        {
            zone.Insert(strip, stripIndex);
            strip = null;

            return base.Undo();
        }
    }
}