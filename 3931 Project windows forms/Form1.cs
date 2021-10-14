using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace _3931_Project_windows_forms
{

    public partial class Form1 : Form
    {
        [DllImport("Asn3DLL.dll")]
        public static extern void changeVolume(double[] amplitudes, double[] originalAmplitudes, double change, int length);

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {

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
            WaveChart.ChartAreas[0].AxisX.ScaleView.Size = newData.Length / 100;

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
    }
}
