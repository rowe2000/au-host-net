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
        private MidiPort outputPort, inputPort;
        private readonly ConcurrentQueue<MidiMessage> packets = new ConcurrentQueue<MidiMessage>();
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
            var client = new MidiClient("AU Midi connector");
            client.ObjectAdded += delegate(object sender, ObjectAddedOrRemovedEventArgs e) {
                //ReloadDevices ();
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
                var packetList = e.PacketListRaw;
                var length1 = Marshal.ReadInt32(packetList);
                packetList += 4;
                
                for (var index = 0; index < length1; ++index)
                {
                    var midiMessage = new MidiMessage(packetList);
                    packets.Enqueue(midiMessage);
                    packetList += 10 + midiMessage.Bytes.Length;
                }
            };

            ConnectExistingDevices ();

            var session = MidiNetworkSession.DefaultSession;
            if (session != null) {
                session.Enabled = true;
                session.ConnectionPolicy = MidiNetworkConnectionPolicy.Anyone;
            }
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
                    packets.TryDequeue(out var packet);
                    Array.Copy(packet.Bytes, 0, bytes, length, packet.Bytes.Length);

                    length += packet.Bytes.Length;
                }

                fixed (byte* ptr = &bytes[0])
                    midiOutputEventBlock(0, 0, length, (IntPtr)ptr);

                return AudioUnitStatus.NoError;

            }
        }
    }

    internal class MidiMessage
    {
        public long Timestamp { get; }
        public byte[] Bytes { get; }
        public byte Status => Bytes[0];
        public byte Byte1 => Bytes[1];
        public byte Byte2 => Bytes[2];

        public MidiMessage( IntPtr ptr)
        {
            Timestamp = Marshal.ReadInt64(ptr);
            var num = (ushort) Marshal.ReadInt16(ptr + 8);
            Bytes = new byte[num];
            for (var i = 0; i < num; i++)
            {
                Bytes[i] = Marshal.ReadByte(ptr + i + 10);
            }
        }
    }
}