using System;
using System.Text;

namespace DialogueSystem
{
    public class Buffer
    {
        public const int KiB = 1024;

        public byte[] Bytes;

        public int Length => m_Length;
        int m_Length = 0;

        static Encoding m_Encoding = Encoding.Unicode;

        public static implicit operator byte[](Buffer buffer) { return buffer.Bytes; }
        public static implicit operator Buffer(byte[] array) { return new Buffer(array); }

        public Buffer(int size)
        {
            Bytes = new byte[size];
        }

        public Buffer(byte[] array)
        {
            Bytes = array;
        }

        public static void SetDefaultStringEncoding(Encoding encoding)
        {
            m_Encoding = encoding;
        }

        public void Shrink()
        {
            Array.Resize(ref Bytes, Length);
        }

        public void Write(byte value)
        {
            Bytes[m_Length] = value;

            m_Length += sizeof(byte);
        }

        public void Write(short value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            bytes.CopyTo(Bytes, m_Length);

            m_Length += bytes.Length;
        }

        public void Write(bool value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            bytes.CopyTo(Bytes, m_Length);
            
            m_Length += bytes.Length;
        }

        public void Write(ushort value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            bytes.CopyTo(Bytes, m_Length);

            m_Length += bytes.Length;
        }

        public void Write(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            bytes.CopyTo(Bytes, m_Length);

            m_Length += bytes.Length;
        }

        public void Write(Enum value)
        {
            Write((int)(object)value);
        }

        public void Write(uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            bytes.CopyTo(Bytes, m_Length);

            m_Length += bytes.Length;
        }

        public void Write(long value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            bytes.CopyTo(Bytes, m_Length);

            m_Length += bytes.Length;
        }

        public void Write(ulong value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            bytes.CopyTo(Bytes, m_Length);

            m_Length += bytes.Length;
        }

        public void Write(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            bytes.CopyTo(Bytes, m_Length);

            m_Length += bytes.Length;
        }

        public void Write(double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            bytes.CopyTo(Bytes, m_Length);

            m_Length += bytes.Length;
        }

        public void Write(string value) => Write(value, m_Encoding);
        public void Write(string value, Encoding encoding)
        {
            int length = encoding.GetByteCount(value);

            Write(length);

            byte[] bytes = encoding.GetBytes(value);
            bytes.CopyTo(Bytes, m_Length);

            m_Length += length;
        }

        public void Read(out byte value)
        {
            value = Bytes[m_Length];
            m_Length += sizeof(byte);
        }

        public void Read(out short value)
        {
            value = BitConverter.ToInt16(Bytes, m_Length);
            m_Length += sizeof(int);
        }

        public void Read(out ushort value)
        {
            value = BitConverter.ToUInt16(Bytes, m_Length);
            m_Length += sizeof(int);
        }

        public void Read(out bool value)
        {
            value = BitConverter.ToBoolean(Bytes, m_Length);
            m_Length += sizeof(bool);
        }
        
        public void Read(out int value)
        {
            value = BitConverter.ToInt32(Bytes, m_Length);
            m_Length += sizeof(int);
        }

        public void Read<TEnum>(out TEnum value)
            where TEnum : Enum
        {
            Read(out int valueAsInt);
            value = (TEnum)(object)valueAsInt;
        }

        public void Read(out uint value)
        {
            value = BitConverter.ToUInt32(Bytes, m_Length);
            m_Length += sizeof(int);
        }

        public void Read(out long value)
        {
            value = BitConverter.ToInt64(Bytes, m_Length);
            m_Length += sizeof(int);
        }

        public void Read(out ulong value)
        {
            value = BitConverter.ToUInt64(Bytes, m_Length);
            m_Length += sizeof(int);
        }

        public void Read(out float value)
        {
            value = BitConverter.ToSingle(Bytes, m_Length);
            m_Length += sizeof(float);
        }

        public void Read(out double value)
        {
            value = BitConverter.ToDouble(Bytes, m_Length);
            m_Length += sizeof(float);
        }

        public void Read(out string value) => Read(out value, m_Encoding);
        public void Read(out string value, Encoding encoding)
        {
            Read(out int length);

            value = encoding.GetString(Bytes, m_Length, length);
            m_Length += length;
        }
    }
}
