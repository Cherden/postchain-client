using System;
using System.Linq;
using System.Collections.Generic;
using Chromia.Postchain.Client;

namespace Chromia.Postchain.Client.ASN1 
{
    public class AsnWriter
    {
        private List<byte> _buffer;
        private List<AsnWriter> _sequences;

        public AsnWriter()
        {
            _buffer = new List<byte>();
            _sequences = new List<AsnWriter>();
        }

        public void WriteNull()
        {
            var buffer = CurrentWriter()._buffer;

            buffer.Add((byte) 0x05);    // tag
            buffer.Add((byte) 0x00);    // content
        }

        public void WriteOctetString(byte[] octetString)
        {
            var buffer = CurrentWriter()._buffer;
            var content = octetString.ToList();

            buffer.Add((byte) 0x04);                        // tag
            buffer.AddRange(GetLengthBytes(content.Count));      // length
            buffer.AddRange(content);   // content
        }

        public void WriteUTF8String(string characterString)
        {
            var buffer = CurrentWriter()._buffer;
            var content = Util.StringToByteArray(characterString).ToList();

            buffer.Add((byte) 0x0c);                        // tag
            buffer.AddRange(GetLengthBytes(content.Count));      // length 
            buffer.AddRange(content);   // content
        }

        public void WriteInteger(int number)
        {
            var buffer = CurrentWriter()._buffer;
            var content = IntegerToBytes(number);
           
            buffer.Add((byte) 0x02);                        // tag
            buffer.AddRange(GetLengthBytes(content.Count));      // length 
            buffer.AddRange(content);// content
        }

        public void PushSequence()
        {
            _sequences.Add(new AsnWriter());
        }

        public void PopSequence()
        {
            var writer = CurrentWriter();
            _sequences.Remove(writer);

            var buffer = CurrentWriter()._buffer;
            var content = writer.Encode().ToList();
            
            buffer.Add((byte) 0x30);                        // tag
            buffer.AddRange(GetLengthBytes(content.Count));      // length
            buffer.AddRange(content);// content
        }

        public void WriteEncodedValue(byte[] encodedValue)
        {
            var buffer = CurrentWriter()._buffer;

            buffer.AddRange(encodedValue);
        }

        public int GetEncodedLength()
        {
            var buffer = CurrentWriter()._buffer;
            return buffer.Count;
        }

        public byte[] Encode()
        {
            if (_sequences.Count != 0)
            {
                throw new System.Exception("Tried to encode with open Sequence.");
            }

            return _buffer.ToArray();
        }

        private AsnWriter CurrentWriter()
        {
            return _sequences.Count == 0 ? this : _sequences[_sequences.Count - 1];
        }

        private List<byte> GetLengthBytes(int length)
        {
            var lengthBytes = new List<byte>();
            if (length < 128)
            {
                lengthBytes.Add((byte) length);
            }
            else
            {
                var sizeInBytes = IntegerToBytes(length);
                
                var sizeLength = (byte) sizeInBytes.Count;

                lengthBytes.Add((byte) (0x80 + sizeLength));
                lengthBytes.AddRange(sizeInBytes);
            }

            return lengthBytes;
        }

        private byte[] TrimByteList(byte[] byteList)
        {
            List<byte> trimmedBytes = new List<byte>();
            for (int i = byteList.Length - 1; i >= 0; i--)
            {
                if (byteList[i] != 0)
                {
                    for (int j = 0; j <= i; j++)
                    {
                        trimmedBytes.Add(byteList[j]);
                    }

                    break;
                }
            }

            return trimmedBytes.ToArray();
        }

        private List<byte> IntegerToBytes(int integer)
        {
            var sizeInBytes = TrimByteList(BitConverter.GetBytes(integer));
                
            if (BitConverter.IsLittleEndian)
            {
                sizeInBytes = sizeInBytes.Reverse().ToArray();
            }
            
            return sizeInBytes.ToList();
        }
    }
}