using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AudioUnit;
using AuHost.Models;
using AVFoundation;
using Xamarin.CommunityToolkit.ObjectModel;

namespace AuHost.Plugins
{
    public class AudioUnitManager
    {
        private readonly ConcurrentDictionary<AudioComponentDescription, List<AVAudioUnit>> recycleBin = new ConcurrentDictionary<AudioComponentDescription, List<AVAudioUnit>>();
        private readonly List<AVAudioUnit> pendingAVAudioUnits = new List<AVAudioUnit>();

        private readonly Dictionary<string, AVAudioUnitComponent> components;

        public readonly ObservableRangeCollection<AuHost.ViewModels.AudioComponent> ViewModelAudioComponents;
        
        public AudioUnitManager()
        {
            var anyMusicDevice = new AudioComponentDescription
            {
                ComponentType = AudioComponentType.MusicDevice,
                ComponentSubType = 0,
                ComponentManufacturer = 0,
                ComponentFlags = 0,
                ComponentFlagsMask = 0
            };

            components = AVAudioUnitComponentManager
                .SharedInstance
                .GetComponents(anyMusicDevice)
                .ToDictionary(o => o.Name, o => o);

            var viewAudioComponents = components.Select(o => new AuHost.ViewModels.AudioComponent { Name = o.Value.Name, Manufacture = o.Value.ManufacturerName });
            ViewModelAudioComponents = new ObservableRangeCollection<AuHost.ViewModels.AudioComponent>(viewAudioComponents);

            foreach(var g in components.Values.GroupBy(o => o.ManufacturerName))
            {
                Console.WriteLine(g.Key);
                Console.Write("    ");
                foreach(var p in g)
                {
                    Console.Write($@"{p.Name}, ");
                }
                Console.WriteLine("");
            }
        }

        public async Task<AVAudioUnit> Fetch(string name, AVAudioUnit avAudioUnit)
        {
            // Check bin to reuse existing AudioUnit
            var component = components[name];
            if (recycleBin.TryGetValue(component.AudioComponentDescription, out var units) && units.Any())
            {
                if (avAudioUnit == null || !units.Contains(avAudioUnit))
                    avAudioUnit = units.Last();

                units.Remove(avAudioUnit);
                return avAudioUnit;
            }

            // Create a new AudioUnit instead
            try
            {
                return await AVAudioUnit.FromComponentDescriptionAsync(component.AudioComponentDescription, AudioComponentInstantiationOptions.LoadedRemotely);
            }
            catch (Exception ex)
            {
                Console.WriteLine($" CRASHED -----> {component.ManufacturerName}, {component.Name}, {ex}, {ex.InnerException},");
            }

            return null;
        }
        
        public void Return(Plugin plugin)
        {
            if (plugin?.IsDeleting != false)
                return;

            plugin.IsDeleting = true;
            var avAudioUnit = plugin.AVAudioUnit;
            pendingAVAudioUnits.Add(avAudioUnit);
            plugin.ShowWindow(false);
            plugin.Dispose();
            
            //TODO: Create PlayingState class 
            if (avAudioUnit.AudioUnit.IsPlaying)
            {
                //node->getPlayingState()->stopPlaying([this, node] { endRecycleNode(node); });
                //return;
            }
            
            EndRecycleNode(avAudioUnit);
        }

        private void EndRecycleNode(AVAudioUnit avAudioUnit)
        {
            if (!pendingAVAudioUnits.Contains(avAudioUnit)) 
                return;
            
            pendingAVAudioUnits.Remove(avAudioUnit);
            avAudioUnit.AudioUnit.Stop();
            avAudioUnit.Engine?.DetachNode(avAudioUnit);

            if (!recycleBin.TryGetValue(avAudioUnit.AUAudioUnit.ComponentDescription, out var units))
            {
                units = new List<AVAudioUnit>();
                recycleBin.TryAdd(avAudioUnit.AUAudioUnit.ComponentDescription, units);
            }
            
            units.Add(avAudioUnit);
        }
    }
}