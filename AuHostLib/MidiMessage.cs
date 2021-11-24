using System;
using System.Runtime.InteropServices;

namespace AuHost
{
    public class MidiMessage
    {
        public long Timestamp { get; }
        public byte[] Bytes { get; }
        public MidiStatus Status => (MidiStatus)(byte)((Bytes[0]-128) / 16);
        public byte Channel => (byte)(Bytes[0] % 16);
        public byte StatusByte => Bytes[0];
        public byte Byte1 => Bytes[1];
        public byte? Byte2 => Bytes.Length > 2 ? Bytes[2] : (byte?)null;

        public MidiMessage(IntPtr ptr)
        {
            Timestamp = Marshal.ReadInt64(ptr);
            var num = (ushort) Marshal.ReadInt16(ptr + 8);
            Bytes = new byte[num];
            for (var i = 0; i < num; i++)
            {
                Bytes[i] = Marshal.ReadByte(ptr + i + 10);
            }
        }

        public MidiMessage(nint length, IntPtr midibytes)
        {
            Bytes = new byte[length];
            for (var i = 0; i < length; i++)
            {
                Bytes[i] = Marshal.ReadByte(midibytes + i);
            }
        }

        public override string ToString()
        {
            return string.Format($@"{Status}, {Channel}, {Byte1}, {Byte2}");
        }
    }
}