using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using AudioUnit;
using AuHost.Plugins;
using AVFoundation;
using Xamarin.Forms;

namespace AuHost.Models
{
    public class Plugin : Slotable<IItem, Strip>, IPresetable
    {
        private bool isShowing;

        public ICommand ToggleGuiCmd { get; }

        public Plugin()
        {
            ToggleGuiCmd = new Command(ToggleGui);
        }
        
        public Plugin(string name) : this()
        {
            Name = name;
        }

        private void ToggleGui()
        {
            isShowing = !isShowing;
            ShowWindow(isShowing);
        }

        public AVAudioUnit AvAudioUnit { get; private set; }
        
        public Preset GetOrCreatePreset()
        {
            var currentPreset = AvAudioUnit.AUAudioUnit.CurrentPreset;
            var dict = AvAudioUnit.AUAudioUnit.GetPresetState(currentPreset, out var err);
            return new PluginPreset(currentPreset) ;
        }

        public bool IsBypassed { get; set; }
        public bool IsDeleting { get; set; }

        public async Task BeginActivate(Strip strip)
        {
            AvAudioUnit = await PluginGraph.Instance.Fetch(Name, AvAudioUnit);
            if (AvAudioUnit == null)
            {
                Console.WriteLine($@"Couldn't create audio unit");
                return;
            }
            
            PluginGraph.Instance.Register(this);
            strip.Items.Insert(Index, this);
        }
    
        public IEnumerable<Connection> GetConnections()
        {
            throw new NotImplementedException();
        }

        public bool AcceptsMidi() => true;
        public bool ProducesMidi() => true;

        public int GetTotalNumOutputChannels() => (int)AvAudioUnit.NumberOfOutputs;
        public int GetTotalNumInputChannels() => (int)AvAudioUnit.NumberOfInputs;

        public bool IsWindowShowing()
        {
            return false;
        }

        public void ShowWindow(bool b)
        {
            
        }

        public void Activate(Strip strip)
        {
            Task.Run(async () => await BeginActivate(strip));
        }

        public void OnMidiMessageReceived(MidiMessage[] midiMessages)
        {
            foreach (var midiMessage in midiMessages)
            {
                AvAudioUnit.AudioUnit.MusicDeviceMIDIEvent(midiMessage.StatusByte, midiMessage.Byte1, midiMessage.Byte2 ?? 0);
                Console.WriteLine($@"{AvAudioUnit.Name}, {midiMessage}");
            }
        }
    }
}