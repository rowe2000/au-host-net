using AudioUnit;

namespace AuHost.Models
{
    public class PluginPreset : Preset
    {
        public AUAudioUnitPreset AuPreset { get; set; }

        public PluginPreset(AUAudioUnitPreset auAudioUnitPreset)
        {
            AuPreset = auAudioUnitPreset;
        }
    }
}
