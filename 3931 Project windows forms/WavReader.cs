using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3931_Project_windows_forms
{
    class WavReader
    {
        private int chunkID;
        private int chunkSize;
        private int format;
        private int subChunk1ID;
        private int subChunk1Size;
        private short audioFormat;
        private short numChannels;
        private int sampleRate;
        private int byteRate;
        private short blockAlign;
        private short bitsPerSample;
        private int subChunk2ID;
        private int subChunk2Size;

        public WavReader(int v1, int v2, int v3, int v4, int v5, short v6, short v7, int v8, int v9, short v10, short v11, int v12, int v13)
        {
            this.chunkID = v1;
            this.chunkSize = v2;
            this.format = v3;
            this.subChunk1ID = v4;
            this.subChunk1Size = v5;
            this.audioFormat = v6;
            this.numChannels = v7;
            this.sampleRate = v8;
            this.byteRate = v9;
            this.blockAlign = v10;
            this.bitsPerSample = v11;
            this.subChunk2ID = v12;
            this.subChunk2Size = v13;
        }

        public int getSubChunk2Size()
        {
            return subChunk2Size;
        }

        public short getBlockAlign()
        {
            return blockAlign;
        }


    }
}
