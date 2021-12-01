using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace _3931_Project_windows_forms
{
    public partial class FrequencyDomain : Form
    {
        complex[] dftData;
        double[] freqData;
        double[] originalWaveData;
        double[] Highlighted;
        double x1 = 0;
        double x2 = 0;
        int sampleRate;
        Form1 f;

        public FrequencyDomain()
        {
            InitializeComponent();
            Highlighted = new double[0];
            freqChart.ChartAreas[0].CursorX.IsUserEnabled = true;
            freqChart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            freqChart.ChartAreas[0].AxisX.ScaleView.Zoomable = false;
            freqChart.ChartAreas[0].AxisX.Minimum = 0;
            freqChart.ChartAreas[0].AxisX.Maximum = Double.NaN;
        }

        public void dftFreqChart(double[] selectedSamples, int srate, complex[] A, double[] waveData, Form1 form)
        {
            f = form;
            originalWaveData = waveData;
            dftData = A;
            sampleRate = srate;
            freqData = selectedSamples;
            for (int i = 0; i < freqData.Length; i++)
            {
                freqChart.Series["Series1"].Points.AddXY(i, freqData[i]);
            }
        }

        // IDFT Button
        private void button3_Click(object sender, EventArgs e)
        {
            freqChart.Series["Series1"].Points.Clear();
            double[] samples = Fourier.inverseDFT(dftData, dftData.Length);
            for (int i = 0; i < samples.Length; i++)
            {
                freqChart.Series["Series1"].Points.AddXY(i, samples[i]);
            }
        }

        // DFT Button
        private void button1_Click_1(object sender, EventArgs e)
        {
            freqChart.Series["Series1"].Points.Clear();
            for (int i = 0; i < freqData.Length; i++)
            {
                freqChart.Series["Series1"].Points.AddXY(i, freqData[i]);
            }
        }
    }
}
