using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeepoBotCSharp {
    class BinReader {
        public byte[] data;
        public int offset = 0;

        public BinReader() {

        }

        public BinReader(int size) {
            data = new byte[size];
        }

        public BinReader(byte[] data) {
            this.data = data;
        }

        public byte readByte() {
            return data[offset++];
        }

        public int readInt() {
            return (((data[offset++]) << 24) | ((data[offset++] & byte.MaxValue) << 16) | ((data[offset++] & byte.MaxValue) << 8) | data[offset++] & byte.MaxValue);
        }

        public uint readUInt32() {
            return (uint)(((data[offset++]) << 24) | ((data[offset++] & byte.MaxValue) << 16) | ((data[offset++] & byte.MaxValue) << 8) | data[offset++] & byte.MaxValue);
        }

        public ulong readUInt64() {
            return (((ulong)readInt() << 32) | ((ulong)readInt() & uint.MaxValue));
        }
    }
}
