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
    /// <summary>
    /// Class for the seperate window that allows dft and idft
    /// </summary>
    public partial class FrequencyDomain : Form
    {
        // Globals for various data, like the freq array and dft array
        complex[] dftData;
        double[] freqData;
        double[] originalWaveData;
        double[] Highlighted;
        double x1 = 0;
        double x2 = 0;
        int sampleRate;
        // Reference to the other form class
        Form1 f;

        /// <summary>
        /// Constructor to initialize the global data and enable a chart
        /// </summary>
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

        /// <summary>
        /// Function to dft the chart and plot the freqData
        /// </summary>
        /// <param name="selectedSamples">The selected samples to dft</param>
        /// <param name="srate">sample rate</param>
        /// <param name="A">DFT data of the selected parts of the array</param>
        /// <param name="waveData">all of the original wave double data</param>
        /// <param name="form">Reference to the main form that displays the main wave</param>
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

        /// <summary>
        /// IDFT Button to display the current dft back to the wave data
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Button events</param>
        private void button3_Click(object sender, EventArgs e)
        {
            freqChart.Series["Series1"].Points.Clear();
            double[] samples = Fourier.inverseDFT(dftData, dftData.Length);
            for (int i = 0; i < samples.Length; i++)
            {
                freqChart.Series["Series1"].Points.AddXY(i, samples[i]);
            }
        }

        /// <summary>
        /// DFT Button, button to display the frequency domain if the DFT button is pressed
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Button events</param>
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
