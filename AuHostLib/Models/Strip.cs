using System.Windows.Input;
using AuHost.Plugins;
using Xamarin.Forms;

namespace AuHost.Models
{
    public class Strip : Slotable<Plugin, Zone>, IPresetable
    {
        public NoteTransform NoteTransform;
        
        public ICommand AddPluginTask => new Command(AddNewPlugin);

        private void AddNewPlugin()
        {
            
        }

        public Preset GetOrCreatePreset()
        {
            return Preset;
        }
    }
}