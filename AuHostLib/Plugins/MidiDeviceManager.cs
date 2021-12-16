using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.InteropServices;
using AudioUnit;
using CoreMidi;
using Xamarin.CommunityToolkit.ObjectModel;

namespace AuHost
{
    public class MidiDeviceManager
    {
        private readonly byte[] bytes = new byte[150];
        private AUMidiOutputEventBlock midiOutputEventBlock = null;
        private AUHostTransportStateBlock transportStateBlock = null;
        //private AUHostMusicalContextBlock musicalContextBlock;
        
        private readonly ConcurrentQueue<MidiMessage> packets = new ConcurrentQueue<MidiMessage>();
        private readonly MidiClient midiClient;

        private readonly ConcurrentDictionary<string, (MidiEndpoint midiEndpoint, MidiPort inPort)> inPorts = new ConcurrentDictionary<string, (MidiEndpoint midiEndpoint, MidiPort inPort)>();
        private readonly ConcurrentDictionary<string, (MidiEndpoint midiEndpoint, MidiPort outPort)> outPorts = new ConcurrentDictionary<string, (MidiEndpoint midiEndpoint, MidiPort outPort)>();
        public ObservableRangeCollection<MidiPort> Inputs { get; } = new ObservableRangeCollection<MidiPort>();
        public ObservableRangeCollection<MidiPort> Outputs { get; } = new ObservableRangeCollection<MidiPort>();

        public MidiDeviceManager()
        {
            Midi.Restart();
            midiClient = GetMidiClient();
            SetupMidi();
        }
        
        private void SetupMidi()
        {
            for (var i = 0; i < Midi.SourceCount; i++) 
                TryConnectInPort(MidiEndpoint.GetSource(i));
            
            for (var i = 0; i < Midi.DestinationCount; i++) 
                TryConnectOutPort(MidiEndpoint.GetDestination(i));
        }

        private void TryConnectOutPort(MidiEndpoint midiEndpoint)
        {
            var entityName = midiEndpoint.Entity?.Name;
            if (entityName == null)
                return;
            
            var outPort = midiClient.CreateOutputPort(entityName);
            //outPort.MessageReceived += OnOutputPortOnMessageReceived;

            var code = outPort.ConnectSource(midiEndpoint);
            if (code != MidiError.Ok)
            {
                Console.WriteLine("Failed to connect");
                return;
            }

            outPorts.TryAdd(midiEndpoint.Name, (midiEndpoint, outPort));
            Outputs.Add(outPort);
        }

        private void TryConnectInPort(MidiEndpoint midiEndpoint)
        {
            var entityName = midiEndpoint.Entity?.Name;
            if (entityName == null)
                return;
            
            var inPort = midiClient.CreateInputPort(entityName);
            inPort.MessageReceived += OnInputPortOnMessageReceived;

            var code = inPort.ConnectSource(midiEndpoint);
            if (code != MidiError.Ok)
            {
                Console.WriteLine("Failed to connect");
                return;
            }

            inPorts.TryAdd(midiEndpoint.Name, (midiEndpoint, inPort));
            Inputs.Add(inPort);
        }

        private void OnInputPortOnMessageReceived(object sender, MidiPacketsEventArgs e)
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
            
            MessagesReceived?.Invoke(midiMessages);
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

        private MidiClient GetMidiClient()
        {
            var client = new MidiClient("Midi connector");
            client.ObjectAdded += delegate(object sender, ObjectAddedOrRemovedEventArgs e)
            {
                var midiEndpoint = (MidiEndpoint) e.Child;
                var entity = midiEndpoint.Entity;
                var device = entity.Device;
                Console.WriteLine($@"Added {midiEndpoint}, {Props(midiEndpoint)}, {Props(entity)}, {Props(device)}");
                TryConnectInPort(midiEndpoint);
                TryConnectOutPort(midiEndpoint);
            };
            client.ObjectRemoved += delegate(object sender, ObjectAddedOrRemovedEventArgs e)
            {
                var midiEndpoint = (MidiEndpoint) e.Child;
                var entity = midiEndpoint.Entity;
                var device = entity.Device;
                Console.WriteLine($@"Removed {midiEndpoint}, {Props(midiEndpoint)}, {Props(entity)}, {Props(device)}");
                TryDisconnectInPort(midiEndpoint);
                TryDisconnectOutPort(midiEndpoint);
            };
            client.PropertyChanged += delegate(object sender, ObjectPropertyChangedEventArgs e)
            {
                var midiDevice = e.MidiObject as MidiDevice;
                Console.WriteLine($@"Changed {e.PropertyName}, {midiDevice?.Name}");
            };
            client.ThruConnectionsChanged += delegate { Console.WriteLine("Thru connections changed"); };
            client.SerialPortOwnerChanged += delegate { Console.WriteLine("Serial port changed"); };
            return client;
        }

        private void TryDisconnectOutPort(MidiEndpoint midiEndpoint)
        {
            if (outPorts.TryRemove(midiEndpoint.Name, out var value))
            {
                value.outPort.Disconnect(value.midiEndpoint);
                Outputs.Remove(value.outPort);
            }
        }

        private void TryDisconnectInPort(MidiEndpoint midiEndpoint)
        {
            if (inPorts.TryRemove(midiEndpoint.Name, out var value))
            {
                value.inPort.Disconnect(value.midiEndpoint);
                Inputs.Remove(value.inPort);
            }
        }

        public event Action<MidiMessage[]> MessagesReceived;

        private AudioUnitStatus InternalRenderBlockProc()
        {
            var len = 0;
            unsafe
            {
                while (!packets.IsEmpty)
                {
                    packets.TryDequeue(out var packet);
                    Array.Copy(packet.Bytes, 0, bytes, len, packet.Bytes.Length);

                    len += packet.Bytes.Length;
                }

                fixed (byte* ptr = &bytes[0])
                    midiOutputEventBlock(1, 0, len, (IntPtr)ptr);

                return AudioUnitStatus.NoError;
            }
        }
    }
}