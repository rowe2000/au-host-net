using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using AudioToolbox;
using AudioUnit;
using AVFoundation;
using CoreMidi;
using Foundation;

namespace MacTest
{
    public class AuMidiConnector : AUAudioUnit 
    {
        public static AudioComponentDescription componentDescription = new AudioComponentDescription {
            ComponentType = AudioComponentType.MIDIProcessor,
            ComponentSubType = 0x666c747,
            ComponentManufacturer = (AudioComponentManufacturerType)0x44656d6e,
            ComponentFlags = 0,
            ComponentFlagsMask = 0
        };

        private byte[] bytes = new byte[150];
        private int length = 0;
        private AUMidiOutputEventBlock midiOutputEventBlock = null;
        private AUHostTransportStateBlock transportStateBlock = null;
        //private AUHostMusicalContextBlock musicalContextBlock;
        private List<MidiPacketsEventArgs> midiPacketsEventArgs = new List<MidiPacketsEventArgs>();
        private List<MidiEndpoint> endPoints = new List<MidiEndpoint>();
        private MidiPort outputPort, inputPort;
        private ConcurrentQueue<MidiMessage> packets = new ConcurrentQueue<MidiMessage>();
        private MidiMessage[] readPacketList;

        public override AudioComponentDescription ComponentDescription { get; } = componentDescription;

        public override AUParameterTree ParameterTree { get; set; }

        public override AUAudioUnitBusArray InputBusses { get; }

        public override AUAudioUnitBusArray OutputBusses { get; }

        public override bool AllocateRenderResources(out NSError outError)
        {
            outError = null;
            midiOutputEventBlock = base.MidiOutputEventBlock;
            transportStateBlock = base.TransportStateBlock;
            //musicalContextBlock = base.MusicalContextBlock;
            
            return true;
        }

        void SetupMidi ()
        {
            var client = new MidiClient("CoreMidiSample MIDI CLient");
            client.ObjectAdded += delegate(object sender, ObjectAddedOrRemovedEventArgs e) {

            };
            client.ObjectAdded += delegate {
                //ReloadDevices ();
            };
            client.ObjectRemoved += delegate {
                //ReloadDevices ();
            };
            client.PropertyChanged += delegate(object sender, ObjectPropertyChangedEventArgs e) {
                Console.WriteLine ("Changed");
            };
            client.ThruConnectionsChanged += delegate {
                Console.WriteLine ("Thru connections changed");
            };
            client.SerialPortOwnerChanged += delegate {
                Console.WriteLine ("Serial port changed");
            };

            outputPort = client.CreateOutputPort ("CoreMidiSample Output Port");
            inputPort = client.CreateInputPort ("CoreMidiSample Input Port");
            inputPort.MessageReceived += delegate(object sender, MidiPacketsEventArgs e) {
                var readPacketList = ReadPacketList(e.PacketListRaw);
                Console.WriteLine(readPacketList.Length);
                foreach (var packet in readPacketList)
                {
                    packets.Enqueue(packet);
                    PrintPacket(packet, "IN");
                }
            };

            ConnectExistingDevices ();

            var session = MidiNetworkSession.DefaultSession;
            if (session != null) {
                session.Enabled = true;
                session.ConnectionPolicy = MidiNetworkConnectionPolicy.Anyone;
            }
        }

        private static void PrintPacket(MidiMessage packet, string s)
        {
                Console.Write($@": {s}, ");
                var on_off = packet.Status == 144 ? "ON" : "OFF";
                Console.Write($@"{on_off}, ");
                Console.Write($@"{packet.Byte1}, ");
                Console.WriteLine();
            
        }

        private MidiMessage[] ReadPacketList(IntPtr packetList)
        {
            var length1 = Marshal.ReadInt32(packetList);
            var midiPacketArray = new MidiMessage[length1];
            packetList += 4;
            for (var index = 0; index < length1; ++index)
            {
                midiPacketArray[index] = ReadPacket(packetList, out var length2);
                packetList += length2;
            }
            return midiPacketArray;

        }

        private static MidiMessage ReadPacket(IntPtr ptr, out int length)
        {
            var timestamp = Marshal.ReadInt64(ptr);
            var num = (ushort) Marshal.ReadInt16(ptr, 8);
            length = 10 + num;
            return new MidiMessage(timestamp, num, ptr + 10);
        }

        void ConnectExistingDevices ()
        {
            for (int i = 0; i < Midi.SourceCount; i++) {
                var code = inputPort.ConnectSource (MidiEndpoint.GetSource (i));
                if (code != MidiError.Ok)
                    Console.WriteLine ("Failed to connect");
            }
        }
        public override string[] MidiOutputNames => new[] {"Midi connector"};

        public override AUInternalRenderBlock InternalRenderBlock => InternalRenderBlockProc;

        public AuMidiConnector (IntPtr handle) : base (handle)
        {
        }

        [Export ("initWithComponentDescription:options:error:")]
        public AuMidiConnector (AudioComponentDescription description, AudioComponentInstantiationOptions options, out NSError error) :
            base (description, options, out error)
        {
            var defaultFormat = new AVAudioFormat (44100, 2);

            NSError err;
            var inputBus = new AUAudioUnitBus (defaultFormat, out err);
            var outputBus = new AUAudioUnitBus (defaultFormat, out err);

            InputBusses = new AUAudioUnitBusArray (this, AUAudioUnitBusType.Input, new [] { inputBus });
            OutputBusses = new AUAudioUnitBusArray (this, AUAudioUnitBusType.Output, new [] { outputBus });

            MaximumFramesToRender = 512;

            Midi.Restart();
            SetupMidi();
        }

        private AudioUnitStatus InternalRenderBlockProc (ref AudioUnitRenderActionFlags actionFlags, ref AudioTimeStamp timestamp, uint frameCount, nint outputBusNumber, AudioBuffers outputData, AURenderEventEnumerator realtimeEventListHead, AURenderPullInputBlock pullInputBlock)
        {
            unsafe
            {
                length = 0;
                while(!packets.IsEmpty)
                {
                    Console.WriteLine(packets.Count);
                    packets.TryDequeue(out var packet);

                    PrintPacket(packet, "DQ");

                    Array.Copy(packet.Bytes, 0, bytes, length, packet.Bytes.Length);

                    length += packet.Bytes.Length;
                    //
                    // for (var i = 0; i < packet.Bytes.Length; i++)
                    // {
                    //     bytes[length] = Marshal.ReadByte(packet.Bytes + i);
                    //     length++;
                    //     // string on_off = bytes[i] == 144 ? "ON" : "OFF";
                    //     // Console.Write($@"{on_off}, ");
                    //     // Console.Write($@"{bytes[i + 1]}, ");
                    // }
                }

                readPacketList = null;


                if (length> 0)
                {
                    Console.Write($@": PROC, ");
                    //Console.Write($@": {length}, ");
                    for (int i = 0; i < length; i+=3)
                    {
                        string on_off = bytes[i] == 144 ? "ON" : "OFF";
                        Console.Write($@"{on_off}, ");
                        Console.Write($@"{bytes[i + 1]}, ");
                    }
                    Console.WriteLine();
                }

                fixed (byte* bytes = &this.bytes[0])
                    midiOutputEventBlock(0, 0, length, (IntPtr) (void*) bytes);

                return AudioUnitStatus.NoError;

                //
                // var p = new List<MidiPacket>();
                //
                // while (!packets.IsEmpty)
                //     if (packets.TryDequeue(out var packet))
                //         p.Add(packet);
                //
                // var l = p.Sum(o => o.Length);
                // var data = new byte[l];
                //
                // foreach (var midiPacket in p)
                // {
                //     midiPacket.Bytes
                // }
                //
                //
                // var midiDataLength = readPacketList.Sum(o => o.Length);
                // midiData = new byte[midiDataLength];
            }
        }
    }

    internal class MidiMessage
    {
        public byte[] Bytes { get; }
        public byte Status => Bytes[0];
        public byte Byte1 => Bytes[1];
        public byte Byte2 => Bytes[2];

        public MidiMessage(long timestamp, ushort num, IntPtr ptr)
        {
            Bytes = new byte[num];
            for (var i = 0; i < num; i++)
            {
                Bytes[i] = Marshal.ReadByte(ptr + i);
            }
        }
    }
}