using System;
using System.Windows.Input;
using AuHost.Commands;
using AuHost.Plugins;
using Command = Xamarin.Forms.Command;

namespace AuHost.Models
{
    public class Strip : Slotable<Plugin, Zone>, IPresetable
    {
        public NoteTransform NoteTransform;

        public Strip()
        { 
            AddSynthPluginCmd = new Command(AddNewSynthPlugin);
            AddMidiPropCmd = new Command(PopupMidiProp);
            AddAudioPropCmd = new Command(PopupAudioProp);
        }

        public ICommand AddMidiPropCmd { get; }
        public ICommand AddAudioPropCmd { get; }
        public ICommand AddSynthPluginCmd { get; }

        public void PopupAudioProp()
        {
            // new PopupMenu (this, showPopupMenu);
            // menu.Inflate (Resource.Menu.popup_menu);
            // menu.Show ();
        }

        public void PopupMidiProp()
        {
        }

        private int number;
        public int Number
        {
            get => number;
            set
            {
                number = value; 
                OnPropertyChanged(nameof(Number));
            }
        }

        private void AddNewSynthPlugin()
        {
            var pluginGraph = PluginGraph.Instance;
            
            pluginGraph
                .CommandExecutor
                .Execute<AddPlugin>(this, pluginGraph.SelectedComponent?.Name, -1);
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