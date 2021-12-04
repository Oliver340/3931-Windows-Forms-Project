using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3931_Project_windows_forms
{
    /// <summary>
    /// Header for the structure of a wave
    /// </summary>
    class WavReader
    {
        // Header data for the wav structure, not including the data samples itself
        public int chunkID;
        public int chunkSize;
        public int format;
        public int subChunk1ID;
        public int subChunk1Size;
        public ushort audioFormat;
        public ushort numChannels;
        public uint sampleRate;
        public uint byteRate;
        public ushort blockAlign;
        public ushort bitsPerSample;
        public int subChunk2ID;
        public int subChunk2Size;

        /// <summary>
        /// Wav Header for the wav header params
        /// </summary>
        /// <param name="v1">chunkID</param>
        /// <param name="v2">chunkSIze</param>
        /// <param name="v3">format</param>
        /// <param name="v4">subChunk1ID</param>
        /// <param name="v5">subChunk1Size</param>
        /// <param name="v6">audioFormat</param>
        /// <param name="v7">numChannels</param>
        /// <param name="v8">sampleRate</param>
        /// <param name="v9">byteRate</param>
        /// <param name="v10">blockAlign</param>
        /// <param name="v11">bitsPerSample</param>
        /// <param name="v12">subChunk2ID</param>
        /// <param name="v13">subChunk2Size</param>
        public WavReader(int v1, int v2, int v3, int v4, int v5, ushort v6, ushort v7, uint v8, uint v9, ushort v10, ushort v11, int v12, int v13)
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

        /// <summary>
        /// Default Constructor
        /// </summary>
        public WavReader() { }

        // Getters for the wave header data

        public int getSubChunk2Size()
        {
            return subChunk2Size;
        }

        public ushort getBlockAlign()
        {
            return blockAlign;
        }

        public uint getSamplesPerSecond()
        {
            return sampleRate;
        }

        public ushort getBitsPerSample()
        {
            return bitsPerSample;
        }



        public int getSubChunk1Size()
        {
            return subChunk1Size;
        }

        public ushort getAudioFormat()
        {
            return audioFormat;
        }

        public ushort getNumChannels()
        {
            return numChannels;
        }

        public uint getSampleRate()
        {
            return sampleRate;
        }

        public uint getByteRate()
        {
            return byteRate;
        }

        public int getChunkSize()
        {
            return chunkSize;
        }

    }
}
