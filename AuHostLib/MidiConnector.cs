using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using AudioUnit;
using CoreMidi;

namespace AuHost
{
    public class MidiConnector
    {
        private byte[] bytes = new byte[150];
        private int length = 0;
        private AUMidiOutputEventBlock midiOutputEventBlock = null;
        private AUHostTransportStateBlock transportStateBlock = null;
        //private AUHostMusicalContextBlock musicalContextBlock;
        private MidiPort outputPort, inputPort;
        private readonly ConcurrentQueue<MidiMessage> packets = new ConcurrentQueue<MidiMessage>();

        void SetupMidi()
        {
            var client = new MidiClient("AU Midi connector");
            client.ObjectAdded += delegate (object sender, ObjectAddedOrRemovedEventArgs e) {
                //ReloadDevices ();
            };
            client.ObjectAdded += delegate {
                //ReloadDevices ();
            };
            client.ObjectRemoved += delegate {
                //ReloadDevices ();
            };
            client.PropertyChanged += delegate (object sender, ObjectPropertyChangedEventArgs e)
            {
                var midiDevice = e.MidiObject as MidiDevice;
                Console.WriteLine($@"Changed {e.PropertyName}, {midiDevice?.Name}");
            };
            client.ThruConnectionsChanged += delegate {
                Console.WriteLine("Thru connections changed");
            };
            client.SerialPortOwnerChanged += delegate {
                Console.WriteLine("Serial port changed");
            };

            outputPort = client.CreateOutputPort("CoreMidiSample Output Port");
            inputPort = client.CreateInputPort("CoreMidiSample Input Port");
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

        void ConnectExistingDevices()
        {
            for (int i = 0; i < Midi.SourceCount; i++)
            {
                var code = inputPort.ConnectSource(MidiEndpoint.GetSource(i));
                if (code != MidiError.Ok)
                    Console.WriteLine("Failed to connect");
            }
        }
        public string[] MidiOutputNames => new[] { "Midi connector" };

        public event Action<MidiMessage[]> MessagesReceived;

        public MidiConnector()
        {
            Midi.Restart();
            SetupMidi();
        }

        private AudioUnitStatus InternalRenderBlockProc()
        {
            unsafe
            {
                length = 0;
                while (!packets.IsEmpty)
                {
                    packets.TryDequeue(out var packet);
                    Array.Copy(packet.Bytes, 0, bytes, length, packet.Bytes.Length);

                    length += packet.Bytes.Length;
                }

                fixed (byte* ptr = &bytes[0])
                    midiOutputEventBlock(1, 0, length, (IntPtr)ptr);

                return AudioUnitStatus.NoError;

            }
        }
    }
}