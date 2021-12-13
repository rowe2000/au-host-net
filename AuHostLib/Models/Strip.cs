using System.Windows.Input;
using AuHost.Commands;
using AuHost.Plugins;
using Command = Xamarin.Forms.Command;

namespace AuHost.Models
{
    public class Strip : Slotable<Plugin, Zone>, IPresetable
    {
        public NoteTransform NoteTransform;

        public new Container<Plugin> Items => base.Items;
        public Strip()
        { 
            AddSynthPluginCmd = new Command(AddNewSynthPlugin);
            AddMidiPropCmd = new Command(() => { });
            AddAudioPropCmd = new Command(() => { });
        }

        private int number;
        public int Number
        {
            get => number;
            set
            {
                number = value; 
                OnPropertyChanged();
            }
        }

        public ICommand AddMidiPropCmd { get; }
        public ICommand AddAudioPropCmd { get; }
        public ICommand AddSynthPluginCmd { get; }

        private void AddNewSynthPlugin()
        {
            var addPlugin = new AddPlugin(this, PluginGraph.Instance.SelectedComponent.Name, Items.Count);
            addPlugin.Execute();
            Items.Add(addPlugin.Plugin);
        }

        public Preset GetOrCreatePreset()
        {
            return Preset;
        }

        public void OnMidiMessageReceived(MidiMessage[] midiMessages)
        {
            foreach (var item in Items)
            {
                item.OnMidiMessageReceived(midiMessages);
            }
        }
    }
}