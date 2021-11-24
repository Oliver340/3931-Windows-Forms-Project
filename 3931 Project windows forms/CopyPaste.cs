using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _3931_Project_windows_forms
{
    internal class CopyPaste
    {
        public static void Copy(double[] copy)
        {
            byte[] copyBytes = new byte[copy.Length*sizeof(double)];
            Buffer.BlockCopy(copy, 0, copyBytes, 0, copyBytes.Length);
            Clipboard.SetAudio(copyBytes);
        }
        public static double[] Paste(double[] original, double location)
        {
            if (!Clipboard.ContainsAudio())
            {
                return original;
            }
            BinaryReader binaryReader = new BinaryReader(Clipboard.GetAudioStream());

            //Initializing Header
            WavReader waveReader = new WavReader(
                binaryReader.ReadInt32(),
                binaryReader.ReadInt32(),
                binaryReader.ReadInt32(),
                binaryReader.ReadInt32(),
                binaryReader.ReadInt32(),
                binaryReader.ReadInt16(),
                binaryReader.ReadInt16(),
                binaryReader.ReadInt32(),
                binaryReader.ReadInt32(),
                binaryReader.ReadInt16(),
                binaryReader.ReadInt16(),
                binaryReader.ReadInt32(),
                binaryReader.ReadInt32()
                );
            byte[] buffer = binaryReader.ReadBytes(waveReader.getSubChunk2Size());
            int bufferLength = (buffer.Length / waveReader.getBlockAlign());

            double[] newWave = new double[original.Length + bufferLength];

            for (int i = 0; i < newWave.Length; i++)
            {
                if (i<(int)location)
                {
                    newWave[i] = original[i];
                } else if (i<(int)location + bufferLength)
                {
                    newWave[i] = BitConverter.ToInt16(buffer, waveReader.getBlockAlign() * i);
                } else
                {
                    newWave[i] = original[i - bufferLength];
                }
            }
            return newWave;
        }

        public static double[] Cut(double[] original, double[] selection, double x1, double x2)
        {
            Copy(selection);
            double[] newWave = new double[original.Length - selection.Length];
            for (int i = 0; i < newWave.Length; i++)
            {
                if (i < x1)
                {
                    newWave[i] = original[i];
                }
                else if (i > x2)
                {
                    newWave[i + (int)(x1 - x2)] = original[i];
                }
            }
            return newWave;
        }
    }
}
