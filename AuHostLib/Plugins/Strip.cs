using System.Windows.Input;
using Xamarin.Forms;

namespace AuHost.Plugins
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