using System;
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
        List<DataPoint> Highlighted = new List<DataPoint>();
        private void WaveChart_MouseUp(object sender, MouseEventArgs e)
        {
            Axis ax = WaveChart.ChartAreas[0].AxisX;
            Axis ay = WaveChart.ChartAreas[0].AxisY;
            Point p1 = new Point(mdown, 0);
            Point p2 = new Point(e.X, Bottom);
            Rectangle area = new Rectangle(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y), Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y));
            foreach(DataPoint wave in WaveChart.Series["chartSeries"].Points)
            {
                int x = (int)ax.ValueToPixelPosition(wave.XValue);
                int y = (int)ay.ValueToPixelPosition(wave.YValues[0]);
                if (area.Contains(new Point(x, y))) {
                    Highlighted.Add(wave);
                }
            }
            foreach (DataPoint wave in WaveChart.Series["chartSeries"].Points) {
                wave.Color = Highlighted.Contains(wave) ? Color.Red : Color.Blue;
            }
            WaveChart.Refresh();
        }

        private void WaveChart_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                WaveChart.Refresh();
                using (Graphics g = WaveChart.CreateGraphics())
                {
                    g.DrawLine(new Pen(Color.Red, 1), new Point(mdown, 20), new Point(mdown, 266));
                    g.DrawLine(new Pen(Color.Red, 1), new Point(e.X, 20), new Point(e.X, 266));
                }
            }
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
