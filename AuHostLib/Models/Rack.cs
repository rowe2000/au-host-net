using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows.Input;
using AuHost.Commands;
using AuHost.Plugins;
using CoreMidi;

namespace AuHost.Models
{
    public class Rack : Slotable<Zone, Frame>, IPresetable
    {
        private MidiPort selectedMidiPort;

        public new Container<Zone> Items => base.Items;

        public ICommand AddZoneCmd { get; }

        public ObservableCollection<MidiPort> MidiInputs => PluginGraph.Instance.MidiDeviceManager.Inputs;

        public ICommand AddMidiPropCmd { get; }

        public MidiPort SelectedMidiPort
        {
            get => selectedMidiPort;
            set
            {
                if (selectedMidiPort == value)
                    return;
                
                if (selectedMidiPort != null)
                    selectedMidiPort.MessageReceived -= OnMidiMessageReceived;
                
                selectedMidiPort = value; 
                OnPropertyChanged();
    
                if (selectedMidiPort != null)
                    selectedMidiPort.MessageReceived += OnMidiMessageReceived;
            }
        }

        private void OnMidiMessageReceived(object sender, MidiPacketsEventArgs e)
        {
            var packetList = e.PacketListRaw;
            var len = Marshal.ReadInt32(packetList);
            var midiMessages = new MidiMessage[len];
            packetList += 4;

            for (var index = 0; index < len; ++index)
            {
                var midiMessage = new MidiMessage(packetList);
                midiMessages[index] = midiMessage;
                packetList += 10 + midiMessage.Bytes.Length;
            }

            foreach (var item in Items)
            {
                item.OnMidiMessageReceived(midiMessages);
            }
        }

        public Preset GetOrCreatePreset()
        {
            return Preset;
        }

        public Rack()
        {
            AddZoneCmd = new Xamarin.Forms.Command(() => AddNewZone());
            AddMidiPropCmd = new Xamarin.Forms.Command(() => {});
        }

        public void AddNewZone(bool before = false)
        {
            var zoneIndex = Items.Count;
            var addZone = new AddZone(this, zoneIndex);
            PluginGraph.Instance.CommandExecutor.Execute(addZone);

            addZone.NewZone.OnAddNewStrip();
        }
    }
}