using AuHost.Plugins;

namespace AuHost.Commands
{
    public class LoadRackPreset : Command
    {
        private Preset previousPreset;

        public override bool SaveInScene => false;
        public int RackId { get; set; }
        public int RackPresetId { get; set; }
        
        public override bool Execute()
        {
            var rack = Cache.GetItem<Rack>(RackId);
            var preset = Cache.GetItem<Preset>(RackPresetId);
            previousPreset = rack.GetOrCreatePreset();

            rack.Preset = preset;

            return base.Execute();
        }

        public override bool Undo()
        {
            PopAll();

            var rack = Cache.GetItem<Rack>(RackId);
            rack.Preset = previousPreset;

            return base.Undo();
        }
    }
}