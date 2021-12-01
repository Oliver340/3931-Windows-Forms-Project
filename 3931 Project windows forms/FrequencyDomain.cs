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
            for (int i = 0; i < fw.Length; i++)
            {
                fw[i] /= filterSize;
            }
            double[] filteredSamples = Filtering.convolution(fw, originalWaveData);
            f.readFilter(filteredSamples);


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
            Console.WriteLine();


            //double[] ogTestSamples = { 2, 1, 5, 4, 9, 7, 8, 6, 4, 6, 4, 6, 1 };
            //complex[] testFilter = new complex[8];
            //testFilter[0].re = 1;
            //testFilter[1].re = 1;
            //testFilter[2].re = 1;
            //testFilter[3].re = 0;
            //testFilter[4].re = 0;
            //testFilter[5].re = 0;
            //testFilter[6].re = 1;
            //testFilter[7].re = 1;
            //double[] testFW = Fourier.inverseDFT(testFilter, 8);
            //for (int i = 0; i < testFW.Length; i++)
            //{
            //    testFW[i] /= 8;
            //}
            //Console.WriteLine();
            //for (int i = 0; i < testFW.Length; i++)
            //{
            //    Console.WriteLine(testFW[i]);
            //}
            //Console.WriteLine();
            //double[] testFilteredSamples = Filtering.convolution(testFW, ogTestSamples);
            //for (int i = 0; i < testFilteredSamples.Length; i++)
            //{
            //    Console.WriteLine(testFilteredSamples[i]);
            //}
        }

        // Button to filter using high pass
        private void button2_Click(object sender, EventArgs e)
        {
            int filterSize = Int32.Parse(textBox1.Text);
            Console.WriteLine(filterSize);
            Console.WriteLine(x2);
            double fcut = x2 * sampleRate / filterSize;
            Console.WriteLine(fcut);
            Console.WriteLine(sampleRate);
            Console.WriteLine();

            complex[] filter = Filtering.highPassFilter(filterSize, fcut, sampleRate);
            double[] fw = Fourier.inverseDFT(filter, filterSize);
            for (int i = 0; i < fw.Length; i++)
            {
                fw[i] /= filterSize;
            }
            double[] filteredSamples = Filtering.convolution(fw, originalWaveData);
            f.readFilter(filteredSamples);
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
