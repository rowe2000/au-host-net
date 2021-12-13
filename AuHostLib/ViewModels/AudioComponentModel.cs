using AudioUnit;
using AVFoundation;

namespace AuHost.ViewModels
{
    public class AudioComponentModel
    {
        public string Name => AudioUnitComponent?.Name;
        public string Manufacture => AudioUnitComponent?.ManufacturerName;
        public bool Enable { get; set; }

        public AVAudioUnitComponent AudioUnitComponent { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}