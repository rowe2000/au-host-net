using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using AppKit;
using AudioUnit;
using AVFoundation;
using Foundation;
using ObjCRuntime;

namespace MacTest
{
	public partial class ViewController : NSViewController
    {
        public ViewController(IntPtr handle) : base(handle)
        {
          
        }
        
        private AVAudioEngine engine = new AVAudioEngine();
        private AVAudioUnit midiInDevAV;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            AUAudioUnit.RegisterSubclass (
                new Class (typeof(AuMidiConnector)),
                AuMidiConnector.componentDescription,
                "Midi out connector",
                int.MaxValue
            );

            
            // Do any additional setup after loading the view.
            
            var anyDescription = new AudioComponentDescription
            {
                ComponentType = (AudioComponentType) AudioComponentType.MusicDevice,
                ComponentSubType = 0,
                ComponentManufacturer = (AudioComponentManufacturerType) 0,
                ComponentFlags = (AudioComponentFlag) 0,
                ComponentFlagsMask = 0
            };
            var components = AVAudioUnitComponentManager.SharedInstance.GetComponents(anyDescription)
                .OrderBy(o => o.ManufacturerName)
                //.Where(o => o.Name == "M1")
                ;

            AVAudioUnit.FromComponentDescription (
                AuMidiConnector.componentDescription,
                AudioComponentInstantiationOptions.OutOfProcess, 
                (unit, error) =>
                {
                    midiInDevAV = unit ?? throw new ArgumentNullException(nameof(unit));
                });

            foreach (var o in components)
            {
                Console.WriteLine($@"{o.Name }");
                 
                try
                {
                    if (o.Name != "M1")
                        continue;
    
                    AVAudioUnit.FromComponentDescription (o.AudioComponentDescription, AudioComponentInstantiationOptions.LoadedRemotely, (av, AVError) =>
                    {
                        if (AVError != null || !(av is AVAudioUnitMidiInstrument aui))
                        {
                            Console.WriteLine($" CREATE FAILED -----> {o.ManufacturerName}, {o.Name}, {AVError}");
                            return;
                        }
                        
                        engine.AttachNode(av);
                        engine.Connect(av, engine.MainMixerNode, engine.MainMixerNode.GetBusOutputFormat(0));
                        
                        engine.AttachNode(midiInDevAV);
                        AUMidiOutputEventBlock tap = Tap;
                        engine.ConnectMidi(midiInDevAV, av, null, tap);

                        engine.Prepare();
                        engine.StartAndReturnError(out var err);
                        Console.WriteLine(err);

                        av.AUAudioUnit.RequestViewController(view => View.AddSubview(view.View));

                        Console.WriteLine($" OK -----> {o.ManufacturerName}, {o.Name}");
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($" CRASHED -----> {o.ManufacturerName}, {o.Name}, {ex}, {ex.InnerException},");
                }
            }
        }

        private int Tap(long eventsampletime, byte cable, nint length, IntPtr midibytes)
        {
            if (length > 0)
            {
                Console.Write($@": TAP, ");
                //Console.Write($@": {length}, ");
                for (int i = 0; i < length; i+=3)
                {
                    string on_off = Marshal.ReadByte(midibytes + i) == 144 ? "ON" : "OFF";
                    Console.Write($@"{on_off}, ");
                    Console.Write($@"{Marshal.ReadByte(midibytes + i + 1)}, ");
                }
                Console.WriteLine();
            }

            return 0;
        }

        public override NSObject RepresentedObject
        {
            get
            {
                return base.RepresentedObject;
            }
            set
            {
                base.RepresentedObject = value;
                // Update the view, if already loaded.
            }
        }
    }
}
