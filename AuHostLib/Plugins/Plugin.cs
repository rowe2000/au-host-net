using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using AVFoundation;

namespace AuHost.Plugins
{
    public class Plugin : Slotable<IItem, Strip>, IPresetable
    {
         
        public AVAudioUnit AVAudioUnit { get; private set; }
        
        public Preset GetOrCreatePreset()
        {
            return Preset;
        }

        public bool IsBypassed { get; set; }
        public bool IsDeleting { get; set; }

        public Plugin(string name)
        {
            Name = name;
        }

        public async Task BeginActivate(Strip strip)
        {
            AVAudioUnit = await PluginGraph.Instance.Fetch(Name, AVAudioUnit);
            if (AVAudioUnit == null)
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

        public int GetTotalNumOutputChannels() => (int)AVAudioUnit.NumberOfOutputs;
        public int GetTotalNumInputChannels() => (int)AVAudioUnit.NumberOfInputs;

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
    }
}