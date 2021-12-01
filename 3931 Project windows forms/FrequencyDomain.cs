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
        double[] Highlighted;
        double x1 = 0;
        double x2 = 0;
        int sampleRate;

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

        public void dftFreqChart(double[] selectedSamples, int srate, complex[] A)
        {
            dftData = A;
            sampleRate = srate;
            freqData = selectedSamples;
            for (int i = 0; i < freqData.Length; i++)
            {
                freqChart.Series["Series1"].Points.AddXY(i, freqData[i]);
            }
        }


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
                Highlighted = new double[(int)(x2 - x1)];
                for (double i = x1; i < x2; i++)
                {
                    Highlighted[(int)(i - x1)] = freqData[(int)i];
                }
            }
        }

        // Button to filter using low pass
        private void button1_Click(object sender, EventArgs e)
        {

            int filterSize = Int32.Parse(textBox1.Text);
            Console.WriteLine(filterSize);
            Console.WriteLine(x2);
            double fcut = x2 * sampleRate / filterSize;
            Console.WriteLine(fcut);
            Console.WriteLine(sampleRate);
            Console.WriteLine();

            complex[] filter = Filtering.lowPassFilter(filterSize, fcut, sampleRate);
            double[] fw = Fourier.inverseDFT(filter, filterSize);
            double[] filteredSamples = Filtering.convolution(fw, freqData);
            for (int i = 0; i < filteredSamples.Length; i++)
            {
                freqChart.Series["Series1"].Points.AddXY(i, filteredSamples[i]);
            }


            for (int i = 0; i < filter.Length; i++)
            {
                Console.WriteLine(filter[i].re);
            }
            Console.WriteLine();
            for (int i = 0; i < fw.Length; i++)
            {
                Console.WriteLine(fw[i]);
            }
            Console.WriteLine();
            for (int i = 0; i < filteredSamples.Length; i++)
            {
                Console.WriteLine(filteredSamples[i]);
            }
            Console.WriteLine();
        }

        // Button to filter using high pass
        private void button2_Click(object sender, EventArgs e)
        {
            //double fcut = freqData[(int)x2];

            //complex[] filter = Filtering.highPassFilter(freqData.Length, fcut, sampleRate);
            //double[] fw = Fourier.inverseDFT(filter, freqData.Length);
            //double[] filteredSamples = Filtering.convolution(fw, freqData);
            //for (int i = 0; i < filteredSamples.Length; i++)
            //{
            //    freqChart.Series["Series1"].Points.AddXY(i, filteredSamples[i]);
            //}


            //for (int i = 0; i < filter.Length; i++)
            //{
            //    Console.WriteLine(filter[i].re);
            //}
            //Console.WriteLine();
            //for (int i = 0; i < fw.Length; i++)
            //{
            //    Console.WriteLine(fw[i]);
            //}
            //Console.WriteLine();
            //for (int i = 0; i < filteredSamples.Length; i++)
            //{
            //    Console.WriteLine(filteredSamples[i]);
            //}
        }

        // IDFT Button
        private void button3_Click(object sender, EventArgs e)
        {
            double[] samples = Fourier.inverseDFT(dftData, dftData.Length);
            for (int i = 0; i < samples.Length; i++)
            {
                freqChart.Series["Series1"].Points.AddXY(i, samples[i]);
            }
        }
    }
}
