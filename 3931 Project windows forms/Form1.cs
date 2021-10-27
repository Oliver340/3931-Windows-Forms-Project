﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
        public static extern int DllMain(HINSTANCE hInstance);

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            HINSTANCE hInstance = Marshal.GetHINSTANCE(typeof(AppContext).Module);
            DllMain(hInstance);
        }

        public double[] waveData;
        public double[] plottedWaveData;

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
            short[] data = new short[buffer.Length / waveReader.getBlockAlign()];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = BitConverter.ToInt16(buffer, waveReader.getBlockAlign() * i);
            }

            double[] newData;
            newData = data.Select(x => (double)x).ToArray();

            WaveChart.ChartAreas[0].AxisX.Minimum = 0;
            WaveChart.ChartAreas[0].AxisX.Maximum = Double.NaN;
            WaveChart.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            WaveChart.ChartAreas[0].AxisX.ScaleView.Size = newData.Length / (vScrollBar1.Value + 1);

            WaveChart.ChartAreas[0].AxisY.Minimum = newData.Min();
            WaveChart.ChartAreas[0].AxisY.Maximum = newData.Max();
            trackBar1.Minimum = 2;
            trackBar1.Maximum = 8;
            trackBar1.Value = 5;

            plotWaveform(newData);

            waveData = newData;
            plottedWaveData = new double[waveData.Length];
        }

        private void plotWaveform(double[] newData)
        {
            WaveChart.ChartAreas[0].CursorX.IsUserEnabled = true;
            WaveChart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            WaveChart.ChartAreas[0].AxisX.ScaleView.Zoomable = false;
            WaveChart.Series["chartSeries"].Points.Clear();
            for (int i = 0; i < newData.Length; i++)
            {
                WaveChart.Series["chartSeries"].Points.AddXY(i, newData[i]);
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (waveData != null)
            {
                changeVolume(plottedWaveData, waveData, (double) trackBar1.Value / 5, waveData.Length);
                plotWaveform(plottedWaveData);
            }
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            if (waveData != null)
            {
                WaveChart.ResetAutoValues();
                WaveChart.ChartAreas[0].AxisX.ScaleView.Size = waveData.Length / (vScrollBar1.Value + 1);
            }
        }

        int mdown;
        List<DataPoint> Highlighted;
        private void WaveChart_MouseUp(object sender, MouseEventArgs e)
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
        }

        private void WaveChart_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mdown = e.Location.X;
            }
        }

        void changeVolume(double[] amplitudes, double[] originalAmplitudes, double change, int length)
        {
            for (int i = 0; i < length; i++)
            {
                amplitudes[i] = originalAmplitudes[i] * change;
            }
        }
    }
}
