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
        public static double[] Paste(double[] original, double[] copied, double x1, double x2)
        {
/*            if (!Clipboard.ContainsAudio())
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
            int bufferLength = (buffer.Length / waveReader.getBlockAlign());*/

            if (copied==null)
            {
                return original;
            }
            
            double[] newWave = new double[original.Length + copied.Length];

            for (int i = 0; i < newWave.Length; i++)
            {
                if (i<(int)x1)
                {
                    newWave[i] = original[i];
                } else if (i<(int)x1+copied.Length)
                {
                    newWave[i] = copied[i-(int)x1];
                } if (i >= x2 && i < original.Length)
                {
                    newWave[i+copied.Length-(int)(x2-x1)] = original[i];
                }
            }
            return newWave;
        }

        public static double[] Cut(double[] original, double[] selection, double x1, double x2)
        {
            //Copy(selection);
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

        public static byte[] BytePaste(byte[] original, byte[] copied, double x1, double x2)
        {
            /*            if (!Clipboard.ContainsAudio())
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
                        int bufferLength = (buffer.Length / waveReader.getBlockAlign());*/

            if (copied == null)
            {
                return original;
            }

            byte[] newWave = new byte[original.Length + copied.Length];

            for (int i = 0; i < newWave.Length; i++)
            {
                if (i < (int)x1)
                {
                    newWave[i] = original[i];
                }
                else if (i < (int)x1 + copied.Length)
                {
                    newWave[i] = copied[i - (int)x1];
                }
                if (i >= x2 && i < original.Length)
                {
                    newWave[i + copied.Length - (int)(x2 - x1)] = original[i];
                }
            }
            return newWave;
        }
    }
}
