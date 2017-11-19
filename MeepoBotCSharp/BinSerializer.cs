//trineroks 2017

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeepoBotCSharp {
    class BinSerializer {
        public byte[] data;
        public int offset = 0;

        public BinSerializer() {

        }

        public BinSerializer(int size) {
            data = new byte[size];
        }

        public BinSerializer(byte[] data) {
            this.data = data;
        }

        public void writeByte(byte value) { 
            data[offset] = value;
            offset++;
        }

        public void writeUInt64(ulong value) {
            writeUInt32((uint)(value >> 32));
            writeUInt32((uint)(value));
        }

        public void writeInt(int value) { // 4 bytes, 32 bit int
            writeByte((byte)(value >> 24));
            writeByte((byte)(value >> 16));
            writeByte((byte)(value >> 8));
            writeByte((byte)(value));
        }

        public void writeUInt32(uint value) {
            writeByte((byte)(value >> 24));
            writeByte((byte)(value >> 16));
            writeByte((byte)(value >> 8));
            writeByte((byte)(value));
        }
    }
}
