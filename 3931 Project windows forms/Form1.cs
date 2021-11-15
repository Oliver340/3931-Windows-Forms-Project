﻿using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace _3931_Project_windows_forms
{

    public partial class Form1 : Form
    {
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

        }

        public double[] waveData;
        public double[] plottedWaveData;
        public byte[] bufferWaveData;
        public byte[] bufferPlottedWaveData;

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
            WaveChart.Series["chartSeries"].Points.Clear();
            for (int i = 0; i < newData.Length; i++)
            {
                WaveChart.Series["chartSeries"].Points.AddXY(i, newData[i]);
            }
        }

        // Function to set the plot of wave display
        private void setPlot(double[] newData)
        {
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
    }
}
