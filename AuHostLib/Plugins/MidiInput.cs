using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.InteropServices;
using AudioUnit;
using CoreMidi;
using Xamarin.CommunityToolkit.ObjectModel;

namespace AuHost
{
    public class MidiInput
    {
        private byte[] bytes = new byte[150];
        private int length = 0;
        private AUMidiOutputEventBlock midiOutputEventBlock = null;
        private AUHostTransportStateBlock transportStateBlock = null;
        //private AUHostMusicalContextBlock musicalContextBlock;
        private MidiPort outputPort, inputPort;
        private readonly ConcurrentQueue<MidiMessage> packets = new ConcurrentQueue<MidiMessage>();
        private static readonly MidiClient Client;

        public ObservableRangeCollection<MidiInput> MidiInputs { get; } = new ObservableRangeCollection<MidiInput>();
        

        static MidiInput()
        {
            Client = SetupClient();
        }
        
        public MidiInput()
        {
            Midi.Restart();
            SetupMidi();
        }


        private void  SetupMidi()
        {
            outputPort = Client.CreateOutputPort("CoreMidiSample Output Port");
            inputPort = Client.CreateInputPort("CoreMidiSample Input Port");
            inputPort.MessageReceived += delegate (object sender, MidiPacketsEventArgs e) {
                var packetList = e.PacketListRaw;
                var length1 = Marshal.ReadInt32(packetList);
                var midiMessages = new MidiMessage[length1];
                packetList += 4;

                for (var index = 0; index < length1; ++index)
                {
                    var midiMessage = new MidiMessage(packetList);
                    midiMessages[index] = midiMessage;
                    packetList += 10 + midiMessage.Bytes.Length;
                }

                MessagesReceived?.Invoke(midiMessages);
            };

            ConnectExistingDevices();

        }

        private static string Props<T>(T obj) where T : class
        {
            var props = obj?
                .GetType()
                .GetProperties()
                .Select(o => 
                {
                    try
                    {
                        return new {o, str = o.GetValue(obj).ToString()};
                    }
                    catch (Exception e)
                    {
                        return null;
                    } 
                })
                .Where(o => !string.IsNullOrWhiteSpace(o?.str))
                .Select(i => $"{i.o.Name}:'{i.str}'");
            
            return props is null ? "" : $"//{typeof(T).Name}::{string.Join(", ", props)}";
        }

        private static MidiClient SetupClient()
        {
            var client = new MidiClient("Midi connector");
            client.ObjectAdded += delegate(object sender, ObjectAddedOrRemovedEventArgs e)
            {
                var midiEndpoint = (MidiEndpoint) e.Child;
                var entity = midiEndpoint.Entity;
                var device = entity.Device;
                var source = entity.GetSource(entity.Sources);
                Console.WriteLine($@"Added {midiEndpoint}, {Props(source)}, {Props(midiEndpoint)}, {Props(midiEndpoint)}");
            };
            client.ObjectRemoved += delegate(object sender, ObjectAddedOrRemovedEventArgs e)
            {
                Console.WriteLine($@"Removed {e.Child}, {Props(e.Child)}");
            };
            client.PropertyChanged += delegate(object sender, ObjectPropertyChangedEventArgs e)
            {
                var midiDevice = e.MidiObject as MidiDevice;
                Console.WriteLine($@"Changed {e.PropertyName}, {midiDevice?.Name}");
            };
            client.ThruConnectionsChanged += delegate
            {
                Console.WriteLine("Thru connections changed");
            };
            client.SerialPortOwnerChanged += delegate { Console.WriteLine("Serial port changed"); };
            return client;
        }

        private void ConnectExistingDevices()
        {
            for (int i = 0; i < Midi.SourceCount; i++)
            {
                var midiEndpoint = MidiEndpoint.GetSource(i);
                var code = inputPort.ConnectSource(midiEndpoint);
                Console.WriteLine($@"{inputPort.PortName}, {Props(midiEndpoint)}");
                if (code != MidiError.Ok)
                    Console.WriteLine("Failed to connect");
            }
        }
        public string[] MidiOutputNames => new[] { "Midi connector" };

        public event Action<MidiMessage[]> MessagesReceived;
    }
}