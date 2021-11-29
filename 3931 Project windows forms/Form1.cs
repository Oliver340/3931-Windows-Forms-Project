using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using HINSTANCE = System.IntPtr;

namespace _3931_Project_windows_forms
{

    public partial class Form1 : Form
    {
        //double x1 = 0;
        //double x2 = 0;
        [DllImport("RecordDLL.dll")]
        public static extern int start();
        [DllImport("RecordDLL.dll")]
        public static extern IntPtr getPSaveBuffer();
        [DllImport("RecordDLL.dll")]
        public static extern uint getSizePSaveBuffer();
        [DllImport("RecordDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void setPSaveBuffer(byte[] values, int length);
        [DllImport("RecordDLL.dll")]
        public static extern void setSizePSaveBuffer(int length);

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                WaveChart.Series["ZeroSeries"].Points.AddXY(i, 0);
                WaveChart.Series["ZeroSeries"].Points.ElementAt(i).Color = Color.Red;
                WaveChart.ChartAreas[0].AxisY.Minimum = -50000;
                WaveChart.ChartAreas[0].AxisY.Maximum = 50000;
                WaveChart.ChartAreas[0].AxisX.Minimum = 0;
                WaveChart.ChartAreas[0].AxisX.Maximum = i;
            }
        }

        public double[] copied;
        public double[] waveData;
        public double[] plottedWaveData;
        public byte[] bufferWaveData;
        public byte[] bufferPlottedWaveData;
        double[] Highlighted;
        double x1 = 0;
        double x2 = 0;

        private void button1_Click(object sender, EventArgs e)
        {
            //Basically the menu where you select files
            OpenFileDialog openFileDialog = new OpenFileDialog();
            //add filter
            openFileDialog.Filter = "WAV File (*.wav)|*.wav|All files (*.*)|*.*";
            //check if file open
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            Highlighted = new double[0];
            BinaryReader binaryReader = new BinaryReader(System.IO.File.OpenRead(openFileDialog.FileName));

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

            double[] newData = new double[buffer.Length / waveReader.getBlockAlign()];
            for (int i = 0; i < newData.Length; i++)
            {
                newData[i] = BitConverter.ToInt16(buffer, waveReader.getBlockAlign() * i);
            }

            setPlot(newData);

            plotWaveform(newData);
            initWaveData(newData);
            //setSizePSaveBuffer(buffer.Length);
            //setPSaveBuffer(buffer, buffer.Length);
        }

        // Function to initialize the display wave data
        private void initWaveData(double[] newData)
        {
            waveData = newData;
            plottedWaveData = new double[waveData.Length];
        }

        // Function to plot a wave
        private void plotWaveform(double[] newData)
        {
            
            WaveChart.Series["ZeroSeries"].Points.Clear();
            WaveChart.Series["chartSeries"].Points.Clear();
            for (int i = 0; i < newData.Length; i++)
            {
                WaveChart.ChartAreas[0].AxisX.Maximum = i;
                WaveChart.Series["ZeroSeries"].Points.AddXY(i, 0);
                WaveChart.Series["ZeroSeries"].Points.ElementAt(i).Color = Color.Red;
                WaveChart.Series["chartSeries"].Points.AddXY(i, newData[i]);
                WaveChart.Series["chartSeries"].Points.ElementAt(i).Color = Color.Blue;
            }
        }

        // Function to set the plot of wave display
        private void setPlot(double[] newData)
        {
            WaveChart.ChartAreas[0].CursorX.IsUserEnabled = true;
            WaveChart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            WaveChart.ChartAreas[0].AxisX.ScaleView.Zoomable = false;
            WaveChart.ChartAreas[0].AxisX.Minimum = 0;
            WaveChart.ChartAreas[0].AxisX.Maximum = Double.NaN;
            WaveChart.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            WaveChart.ChartAreas[0].AxisX.ScaleView.Size = newData.Length / (vScrollBar1.Value + 1);
            if (newData.Min() > newData.Max())
            {
                WaveChart.ChartAreas[0].AxisY.Minimum = newData.Min();
                WaveChart.ChartAreas[0].AxisY.Maximum = -newData.Min();
            }
            else
            {
                WaveChart.ChartAreas[0].AxisY.Minimum = -newData.Max();
                WaveChart.ChartAreas[0].AxisY.Maximum = newData.Max();
            }
            //WaveChart.ChartAreas[0].AxisY.Minimum = -128;
            //WaveChart.ChartAreas[0].AxisY.Maximum = 128;

            trackBar1.Minimum = 0;
            trackBar1.Maximum = 11;
            trackBar1.Value = 5;
        }

        // Volume Track Bar
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (waveData != null)
            {
                changeVolume(plottedWaveData, waveData, (double) trackBar1.Value / 5, waveData.Length);
                plotWaveform(plottedWaveData);
            }
            if (bufferWaveData != null)
            {
                changeBufferVolume(bufferPlottedWaveData, bufferWaveData, (double)trackBar1.Value / 5, waveData.Length);
            }
        }

        // Function to change the wave display volume
        void changeVolume(double[] amplitudes, double[] originalAmplitudes, double change, int length)
        {
            for (int i = 0; i < length; i++)
            {
                amplitudes[i] = originalAmplitudes[i] * change;
            }

        }

        // Function to change the volume of recorded data
        void changeBufferVolume(byte[] amplitudes, byte[] originalAmplitudes, double change, int length)
        {
            for (int i = 0; i < length; i++)
            {
                amplitudes[i] = (byte)Math.Min(Math.Max(originalAmplitudes[i] * change, 0), 255);
            }
            setPSaveBuffer(amplitudes, amplitudes.Length);
        }

        // Vertical Scroll Bar
        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            if (waveData != null)
            {
                WaveChart.ResetAutoValues();
                WaveChart.ChartAreas[0].AxisX.ScaleView.Size = waveData.Length / (vScrollBar1.Value + 1);
            }
        }

        // Record Button
        private void button2_Click(object sender, EventArgs e)
        {
            start();
        }

        //int mdown;
        /*        private void WaveChart_MouseUp(object sender, MouseEventArgs e)
                {
                    double x1 = WaveChart.ChartAreas[0].CursorX.SelectionStart;
                    double x2 = WaveChart.ChartAreas[0].CursorX.SelectionEnd;
                    Highlighted = new List<DataPoint>();
                    Axis ax = WaveChart.ChartAreas[0].AxisX;
                    foreach (DataPoint wave in WaveChart.Series["chartSeries"].Points)
                    {
                        int x = (int)(ax.ValueToPixelPosition(wave.XValue)-94);
                        if ((x1 <= x && x <= x2) || (x2 <= x && x <= x1))
                        {
                            Highlighted.Add(wave);
                        }
                    }
                    foreach (DataPoint wave in WaveChart.Series["chartSeries"].Points)
                    {
                        wave.Color = Highlighted.Contains(wave) ? Color.Red : Color.Blue;
                    }
                    WaveChart.Refresh();
                }*/

        //function to control what selection area does
        private void chart_SelectionRangeChanged(object sender, CursorEventArgs e)
        {
            if (!double.IsNaN(e.NewSelectionStart) && !double.IsNaN(e.NewSelectionEnd))
            {
                if (e.NewSelectionStart < e.NewSelectionEnd)
                {
                    x1 = e.NewSelectionStart;
                    x2 = e.NewSelectionEnd;
                }
                else
                {
                    x2 = e.NewSelectionStart;
                    x1 = e.NewSelectionEnd;
                }
                Highlighted = new double[(int)(x2-x1)];
                for (double i = x1; i < x2; i++)
                {
                    Highlighted[(int)(i-x1)]=waveData[(int)i];
                }
            }
        }

        // Load Button
        private unsafe void button3_Click(object sender, EventArgs e)
        {
            byte* pSaveBuffer = (byte*)getPSaveBuffer();
            uint sizePBuffer = getSizePSaveBuffer();
            byte[] buffer = new byte[sizePBuffer];
            for (int i = 0; i < sizePBuffer; i++)
            {
                buffer[i] = *(pSaveBuffer + i * sizeof(byte));
            }

            double[] newData = new double[buffer.Length];
            for (int i = 0; i < newData.Length; i++)
            {
                newData[i] = buffer[i];
                newData[i] -= 128;
            }

            initWaveData(newData);
            bufferWaveData = buffer;
            bufferPlottedWaveData = new byte[bufferWaveData.Length];

            setPlot(waveData);
            plotWaveform(waveData);
        }

        //Cut button
        private void button6_Click(object sender, EventArgs e)
        {
            copied = Highlighted;
            waveData = CopyPaste.Cut(waveData, Highlighted, x1, x2);
            plotWaveform(waveData);
        }

        //Copy button
        private void button4_Click(object sender, EventArgs e)
        {
            //CopyPaste.Copy(Highlighted);
            copied = Highlighted;
        }

        //Paste button
        private void button5_Click(object sender, EventArgs e)
        {
            waveData=CopyPaste.Paste(waveData, copied, x1, x2);
            plotWaveform(waveData);
        }

        // Save Button
        private void button7_Click(object sender, EventArgs e)
        {
            // Save file pop up
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "WAV File (*.wav)|*.wav|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            // Binary Writer
            BinaryWriter wr = new BinaryWriter(System.IO.File.OpenWrite(saveFileDialog.FileName));

            //Header Bytes
            int subChunk1Size = 16;
            short audioFormat = 1;
            short bitsPerSample = 8; // old -> 16
            short numChannels = 2; // old -> 2
            int sampleRate = 11025; // old -> 22050
            int byteRate = sampleRate * numChannels * (bitsPerSample / 8);
            int numSamples = bufferWaveData.Length;
            short blockAlign = (short)(numChannels * (bitsPerSample / 8));

            int subChunk2Size = numSamples * numChannels * (bitsPerSample / 8);

            int chunkSize = 4 + (8 + subChunk1Size) + (8 + subChunk2Size);

            // Write header values
            wr.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
            wr.Write(chunkSize);
            wr.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));
            wr.Write(System.Text.Encoding.ASCII.GetBytes("fmt"));
            wr.Write((byte)32);
            wr.Write(subChunk1Size);
            wr.Write(audioFormat);
            wr.Write(numChannels);
            wr.Write(sampleRate);
            wr.Write(byteRate);
            wr.Write(blockAlign);
            wr.Write(bitsPerSample);
            wr.Write(System.Text.Encoding.ASCII.GetBytes("data"));
            wr.Write(subChunk2Size);

            //wr.Write(bufferWaveData);

            for (int i = 0; i < numSamples; i++)
            {
                wr.Write(bufferWaveData[i]);
                wr.Write(bufferWaveData[i]); // LEFT CHANNEL THEN RIGHT CHANNEL
            }

            wr.Close();
            wr.Dispose();
        }
    }
}
