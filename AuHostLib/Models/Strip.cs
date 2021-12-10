using System.Windows.Input;
using AuHost.Plugins;
using Xamarin.Forms;

namespace AuHost.Models
{
    public class Strip : Slotable<Plugin, Zone>, IPresetable
    {
        public NoteTransform NoteTransform;

        public Strip()
        { 
            AddSynthCmd = new Command(AddNewPlugin);
            AddMidiPropCmd = new Command(()=>{ });
            AddAudioPropCmd = new Command(()=>{ });
        }


        public int Number => Index + 1;

        public ICommand AddMidiPropCmd { get; }
        public ICommand AddAudioPropCmd { get; }
        public ICommand AddSynthCmd { get; }

        private void AddNewPlugin()
        {
        }

        public Preset GetOrCreatePreset()
        {
            return Preset;
        }
    }
}