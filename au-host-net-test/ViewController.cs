using System;
using System.Collections.Generic;
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
        
        private readonly AVAudioEngine engine = new AVAudioEngine();
        private AVAudioUnit midiInDev;
        private readonly List<AVAudioUnitComponent> plugins = new List<AVAudioUnitComponent>(); 

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
                .GroupBy(o => o.ManufacturerName)
                //.Where(o => o.Name == "M1")
                ;

            AVAudioUnit.FromComponentDescription (
                AuMidiConnector.componentDescription,
                AudioComponentInstantiationOptions.OutOfProcess, 
                (unit, error) =>
                {
                    midiInDev = unit ?? throw new ArgumentNullException(nameof(unit));
                });

            foreach (var group in components)
            {
                Console.WriteLine();
                Console.WriteLine(group.Key);
                Console.Write("     ");
                switch (group.Key)
                {
                    //case "Roland Cloud":
                    //case "KORG":
                    case "Arturia":
                    //case "GG Audio":
                    //case "Plogue Art et Technologie":
                    //case "Modartt":
                    //case "SonicProjects":
                    //case "Applied Acoustics Systems":
                    //case "Digital Suburban":
                        foreach (var o in group)
                        {
                            switch (o.Name)
                            {
                                case "DX7 V":
                                    plugins.Add(o);
                                    break;
                                default:
                                    Console.Write($@"{o.Name}, ");
                                    break;
                            }
                        }

                        break;

                    default:
                        foreach (var o in group)
                            Console.Write($@"{o.Name}, "); 
                        break;
                }
            }


            foreach (var component in plugins)
            {
                CreatePlugin(component);
            }
        }

        private void CreatePlugin(AVAudioUnitComponent o)
        {
            try
            {
                AVAudioUnit.FromComponentDescription(o.AudioComponentDescription,
                    AudioComponentInstantiationOptions.LoadedRemotely, (av, AVError) =>
                    {
                        if (AVError != null || !(av is AVAudioUnitMidiInstrument aui))
                        {
                            Console.WriteLine($" CREATE FAILED -----> {o.ManufacturerName}, {o.Name}, {AVError}");
                            return;
                        }

                        engine.AttachNode(av);
                        engine.Connect(av, engine.MainMixerNode, 0, 0, engine.MainMixerNode.GetBusOutputFormat(0));

                        engine.AttachNode(midiInDev);
                        engine.ConnectMidi(midiInDev, av, null, null);

                        engine.Prepare();
                        engine.StartAndReturnError(out var err);
                        Console.WriteLine(err);

                        av.AUAudioUnit.RequestViewController(view =>
                        {
                            View.Frame = view.View.Frame;
                            View.AddSubview(view.View);
                        });
                        
                        av.AudioUnit.Start();
                        aui.Volume = 0.5f;

                        Console.WriteLine($" OK -----> {o.ManufacturerName}, {o.Name}");
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine($" CRASHED -----> {o.ManufacturerName}, {o.Name}, {ex}, {ex.InnerException},");
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
