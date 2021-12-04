using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using HINSTANCE = System.IntPtr;

// Form class represents the main window and displays the wave, contains buttons for various functionality.
namespace _3931_Project_windows_forms
{

    public unsafe partial class Form1 : Form
    {
        //double x1 = 0;
        //double x2 = 0;
        [DllImport("RecordDLL.dll")]
        // Function to start the Record dll
        public static extern int start();
        [DllImport("RecordDLL.dll")]
        // Function to get the wave data from the dll
        public static extern IntPtr getPSaveBuffer();
        [DllImport("RecordDLL.dll")]
        // Function to get the the length of wave data
        public static extern uint getSizePSaveBuffer();
        [DllImport("RecordDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        // Function to set the psave buffer of the local byte wave data (bufferWaveData)
        public static extern void setPSaveBuffer(byte* values, int length, int samplesPerSec, short blockAlign, short bitsPerSample, short numChannels);
        [DllImport("RecordDLL.dll")]
        // Function to set the size of the wave data in the dll
        public static extern void setSizePSaveBuffer(int length);
        [DllImport("RecordDLL.dll")]
        // Function to send a record message to the dll
        public static extern void recordData();
        [DllImport("RecordDLL.dll")]
        // Function to send a stop record message to the dll
        public static extern void stopRecordData();
        [DllImport("RecordDLL.dll")]
        // Function to send the play message to the dll
        public static extern void playData();
        [DllImport("RecordDLL.dll")]
        // Function to send the pause message to the dll
        public static extern void pauseData();
        [DllImport("RecordDLL.dll")]
        // Function to send the stop message to the dll
        public static extern void stopPlayData();


        public Form1()
        {
            InitializeComponent();
        }
        // Function that initializes some components
        private void Form1_Load(object sender, EventArgs e)
        {
            //hScrollBar1.Visible = false;
            freqChart.Series["Series1"].Points.AddXY(0, 0);
            for (int i = 0; i < 10; i++)
            {
                WaveChart.Series["ZeroSeries"].Points.AddXY(i, 0);
                WaveChart.Series["ZeroSeries"].Points.ElementAt(i).Color = Color.Red;
                WaveChart.ChartAreas[0].AxisY.Minimum = -50000;
                WaveChart.ChartAreas[0].AxisY.Maximum = 50000;
                WaveChart.ChartAreas[0].AxisX.Minimum = 0;
                WaveChart.ChartAreas[0].AxisX.Maximum = i;
            }
            // Start the DLL
            start();
        }

        //Globals for double array of wave data and byte array of wave data
        // as well as some other stuff for highlighting and.
        public double[] copied = null;
        complex[] windowDFT;
        WavReader waveReader;
        public double[] windowedData = null;
        public double[] waveData;
        public double[] plottedWaveData;
        public byte[] bufferWaveData;
        public byte[] bufferPlottedWaveData;
        public double[] Highlighted;
        double x1 = 0;
        double x2 = 0;
        double x3 = 0;
        double x4 = 0;

        /// <summary>
        /// Open Button to initialize dialog and set/plot the current wave data
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Button event</param>
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
            //Initialize header data
            waveReader = new WavReader();
            waveReader.chunkID = binaryReader.ReadInt32();
            waveReader.chunkSize = binaryReader.ReadInt32();
            waveReader.format = binaryReader.ReadInt32();
            waveReader.subChunk1ID = binaryReader.ReadInt32();
            waveReader.subChunk1Size = binaryReader.ReadInt32();
            waveReader.audioFormat = binaryReader.ReadUInt16();
            waveReader.numChannels = binaryReader.ReadUInt16();
            waveReader.sampleRate = binaryReader.ReadUInt32();
            waveReader.byteRate = binaryReader.ReadUInt32();
            waveReader.blockAlign = binaryReader.ReadUInt16();
            waveReader.bitsPerSample = binaryReader.ReadUInt16();
            int dataIndex = 0;
            while (binaryReader.ReadByte() < 0)
            {
                ++dataIndex;
            }
            binaryReader.BaseStream.Seek((4 * 7) + (2 * 4) + dataIndex, SeekOrigin.Begin);
            waveReader.subChunk2ID = binaryReader.ReadInt32();
            waveReader.subChunk2Size = binaryReader.ReadInt32();

            byte[] buffer = binaryReader.ReadBytes(waveReader.getSubChunk2Size());

            short[] shortBuffer = new short[buffer.Length / waveReader.getBlockAlign()];
            if (waveReader.bitsPerSample == 8)
            {
                for (int i = 0; i < shortBuffer.Length - 1; i++)
                {
                    shortBuffer[i] = (short) (buffer[i] - 128);
                }
            }
            else
            {
                for (int i = 0; i < shortBuffer.Length - 1; i++)
                {
                    shortBuffer[i] = BitConverter.ToInt16(buffer, waveReader.getBlockAlign() * i);
                }
            }
            double[] newData = shortBuffer.Select(x => (double)(x)).ToArray();

            initWaveData(newData);
            initBufferData(buffer);

            setPlot(newData);
            plotWaveform(newData);

            // Set the dll data
            fixed (byte* array = bufferWaveData)
            {
                setPSaveBuffer(array, bufferWaveData.Length, (int) waveReader.getSamplesPerSecond(), (short) waveReader.getBlockAlign(), (short) waveReader.getBitsPerSample(), (short) waveReader.getNumChannels());
            }
            hScrollBar1.Visible = true;
        }

        /// <summary>
        /// Function to initialize the display wave data
        /// </summary>
        /// <param name="newData">new wave data samples</param>
        public void initWaveData(double[] newData)
        {
            waveData = newData;
            plottedWaveData = new double[waveData.Length];
        }

        /// <summary>
        /// Function to initialize the buffer wave data
        /// </summary>
        /// <param name="buffer">samples for the byte array</param>
        private void initBufferData(byte[] buffer)
        {
            bufferWaveData = buffer;
            bufferPlottedWaveData = new byte[bufferWaveData.Length];
        }

        /// <summary>
        /// Function to plot a wave
        /// </summary>
        /// <param name="newData">new wave data samples (double)</param>
        public void plotWaveform(double[] newData)
        {
            
            WaveChart.Series["ZeroSeries"].Points.Clear();
            WaveChart.Series["chartSeries"].Points.Clear();
            int scrollStart = hScrollBar1.Value;
            int scrollEnd = (int) WaveChart.ChartAreas[0].AxisX.ScaleView.Size + scrollStart;
            int index = 0;
            for (int i = scrollStart; i < scrollEnd && i < newData.Length; i++)
            {
                WaveChart.ChartAreas[0].AxisX.Maximum = index;
                WaveChart.Series["ZeroSeries"].Points.AddXY(index, 0);
                WaveChart.Series["chartSeries"].Points.AddXY(index, newData[i]);
                index++;
            }
        }

        /// <summary>
        /// Horizontal ScrollBar, plots the data after a scroll
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Button event</param>
        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            if (WaveChart.ChartAreas[0].AxisX.ScaleView.Size + hScrollBar1.Value < waveData.Length)
            {
                plotWaveform(waveData);
            }
        }

        /// <summary>
        /// Mouse scroll zoom, deletes some data when zooming in and adds points when zooming out
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Button event</param>
        private void chart1_MouseWheel(object sender, MouseEventArgs e)
        {
            int initSize = (int)WaveChart.ChartAreas[0].AxisX.ScaleView.Size;
            double zoomConstant = e.Delta * 0.5;
            if (WaveChart.ChartAreas[0].AxisX.ScaleView.Size - zoomConstant > 0 && WaveChart.ChartAreas[0].AxisX.ScaleView.Size - zoomConstant < waveData.Length
                && WaveChart.ChartAreas[0].AxisX.ScaleView.Size - zoomConstant + hScrollBar1.Value < waveData.Length)
            {
                WaveChart.ChartAreas[0].AxisX.ScaleView.Size -= zoomConstant;
                if (WaveChart.ChartAreas[0].AxisX.ScaleView.Size > initSize)
                {
                    for (int i = initSize; i < WaveChart.ChartAreas[0].AxisX.ScaleView.Size && i + hScrollBar1.Value < waveData.Length; i++)
                    {
                        WaveChart.ChartAreas[0].AxisX.Maximum = i;
                        WaveChart.Series["ZeroSeries"].Points.AddXY(i, 0);
                        WaveChart.Series["chartSeries"].Points.AddXY(i, waveData[i + hScrollBar1.Value]);
                    }
                } else
                {
                    for (int i = initSize - 1; i >= (int)WaveChart.ChartAreas[0].AxisX.ScaleView.Size; i--)
                    {
                        WaveChart.Series["ZeroSeries"].Points.RemoveAt(i);
                        WaveChart.Series["chartSeries"].Points.RemoveAt(i);
                        WaveChart.ChartAreas[0].AxisX.ScrollBar.Enabled = false;
                    }
                }
                hScrollBar1.Maximum = waveData.Length - (int)WaveChart.ChartAreas[0].AxisX.ScaleView.Size;
            }
        }

        /// <summary>
        /// Function to set the plot of wave display, initialize the starting chart areas
        /// </summary>
        /// <param name="newData">new wave data for the samples for the double array</param>
        public void setPlot(double[] newData)
        {
            hScrollBar1.Value = 0;
            hScrollBar1.Minimum = 0;
            WaveChart.ChartAreas[0].CursorX.IsUserEnabled = true;
            WaveChart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            WaveChart.ChartAreas[0].AxisX.ScaleView.Zoomable = false;
            WaveChart.ChartAreas[0].AxisX.Minimum = 0;
            WaveChart.ChartAreas[0].AxisX.Maximum = Double.NaN;
            WaveChart.Series["ZeroSeries"].Color = Color.Red;
            WaveChart.Series["chartSeries"].Color = Color.Blue;
            WaveChart.ChartAreas[0].AxisX.ScaleView.Size = 100;
            hScrollBar1.Maximum = newData.Length - (int)WaveChart.ChartAreas[0].AxisX.ScaleView.Size;
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

            trackBar1.Minimum = 0;
            trackBar1.Maximum = 11;
            trackBar1.Value = 5;
            this.WaveChart.MouseWheel += chart1_MouseWheel;
        }

        /// <summary>
        /// Volume Track Bar, changes the plot of the data, multiplies original display
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Button event</param>
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

        /// <summary>
        /// Function to change the wave display volume
        /// </summary>
        /// <param name="amplitudes">the amplitudes to change after the volume change</param>
        /// <param name="originalAmplitudes">the original wave data samples</param>
        /// <param name="change">the constant to multiply the wave by</param>
        /// <param name="length">the amount of samples</param>
        void changeVolume(double[] amplitudes, double[] originalAmplitudes, double change, int length)
        {
            for (int i = 0; i < length; i++)
            {
                amplitudes[i] = originalAmplitudes[i] * change;
            }

        }

        /// <summary>
        /// Function to change the volume of recorded data
        /// </summary>
        /// <param name="amplitudes">the amplitudes to change after the volume change</param>
        /// <param name="originalAmplitudes">the original wave data samples in bytes</param>
        /// <param name="change">the constant to multiply the wave by</param>
        /// <param name="length">the amount of samples</param>
        void changeBufferVolume(byte[] amplitudes, byte[] originalAmplitudes, double change, int length)
        {
            for (int i = 0; i < length; i++)
            {
                amplitudes[i] = (byte)Math.Min(Math.Max(originalAmplitudes[i] * change, 0), 255);
            }
            // Does not set the dll data
            //fixed (byte* bytePtr = amplitudes)
            //{
            //    setPSaveBuffer(bytePtr, amplitudes.Length, waveReader.getSamplesPerSecond(), waveReader.getBlockAlign(), waveReader.getBitsPerSample());
            //}
        }


        /// <summary>
        /// function to control what selection area does
        /// Highlighted is an array x values are start and end index of selection
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Button event</param>
        private void chart_SelectionRangeChanged(object sender, CursorEventArgs e)
        {
            if (!double.IsNaN(e.NewSelectionStart) && !double.IsNaN(e.NewSelectionEnd))
            {
                freqChart.Series["Series1"].Points.Clear();
                freqChart.Series["Series2"].Points.Clear();
                if (e.NewSelectionStart < e.NewSelectionEnd)
                {
                    x1 = e.NewSelectionStart + hScrollBar1.Value;
                    x2 = e.NewSelectionEnd + hScrollBar1.Value;
                }
                else
                {
                    x2 = e.NewSelectionStart + hScrollBar1.Value;
                    x1 = e.NewSelectionEnd + hScrollBar1.Value;
                }
                Highlighted = new double[(int)(x2-x1)];
                for (double i = x1; i < x2; i++)
                {
                    Highlighted[(int)(i-x1)]=waveData[(int)i];
                }
            }
        }

        /// <summary>
        /// function to control what second selection area does, for the byte array
        /// (similar to above function)
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Button event</param>
        private void chart2_SelectionRangeChanged(object sender, CursorEventArgs e)
        {
            if (!double.IsNaN(e.NewSelectionStart) && !double.IsNaN(e.NewSelectionEnd))
            {
                if (e.NewSelectionStart < e.NewSelectionEnd)
                {
                    x3 = e.NewSelectionStart + hScrollBar1.Value;
                    x4 = e.NewSelectionEnd + hScrollBar1.Value;
                }
                else
                {
                    x3 = e.NewSelectionStart + hScrollBar1.Value;
                    x4 = e.NewSelectionEnd + hScrollBar1.Value;
                }
            }
        }

        /// <summary>
        /// Cut button, replots data on wave and changes the buffer data and sets the dll
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Button event</param>
        private void button6_Click(object sender, EventArgs e)
        {
            copied = Highlighted;
            waveData = CopyPaste.Cut(waveData, Highlighted, x1, x2);
            setPlot(waveData);
            plotWaveform(waveData);

            if (waveReader.bitsPerSample > 16)
            {
                bufferWaveData = waveData.Select(x => Convert.ToInt32(x))
                                  .SelectMany(x => BitConverter.GetBytes(x))
                                  .ToArray();
            }
            else if (waveReader.bitsPerSample > 8)
            {
                bufferWaveData = waveData.Select(x => Convert.ToInt16(x))
                                  .SelectMany(x => BitConverter.GetBytes(x))
                                  .ToArray();
            }
            else
            {
                for (int i = 0; i < waveData.Length; i++)
                {
                    bufferWaveData[i] = (byte)(waveData[i] + 128);
                }
            }

            fixed (byte* array = bufferWaveData)
            {
                setPSaveBuffer(array, bufferWaveData.Length, (int)waveReader.getSamplesPerSecond(), (short)waveReader.getBlockAlign(), (short)waveReader.getBitsPerSample(), (short)waveReader.getNumChannels());
            }
        }

        /// <summary>
        /// Copy button, to set the Highlighted array to what you select
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Button event</param>
        private void button4_Click(object sender, EventArgs e)
        {
            //CopyPaste.Copy(Highlighted);
            copied = Highlighted;
        }

        /// <summary>
        /// Paste button to get the data from the Highlighted array to put into the byte array and the waveData double array
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Button event</param>
        private void button5_Click(object sender, EventArgs e)
        {
            double[] newData = CopyPaste.Paste(waveData, copied, x1, x2);
            waveData = newData;

            if (waveReader.bitsPerSample > 16)
            {
                bufferWaveData = waveData.Select(x => Convert.ToInt32(x))
                                  .SelectMany(x => BitConverter.GetBytes(x))
                                  .ToArray();
            }
            else if (waveReader.bitsPerSample > 8)
            {
                bufferWaveData = waveData.Select(x => Convert.ToInt16(x))
                                  .SelectMany(x => BitConverter.GetBytes(x))
                                  .ToArray();
            }
            else
            {
                for (int i = 0; i < waveData.Length; i++)
                {
                    bufferWaveData[i] = (byte)(waveData[i] + 128);
                }
            }

            setPlot(newData);
            plotWaveform(newData);

            fixed (byte* array = bufferWaveData)
            {
                setPSaveBuffer(array, bufferWaveData.Length, (int)waveReader.getSamplesPerSecond(), (short)waveReader.getBlockAlign(), (short)waveReader.getBitsPerSample(), (short)waveReader.getNumChannels());
            }
        }

        /// <summary>
        /// Save button to write to a file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            int subChunk1Size = waveReader.getSubChunk1Size();
            Console.WriteLine("subchunk1size" + subChunk1Size);
            short audioFormat = (short)waveReader.getAudioFormat();
            Console.WriteLine("audioformat" + audioFormat);
            short bitsPerSample = (short)waveReader.getBitsPerSample();
            Console.WriteLine("bitspersample" + bitsPerSample);
            short numChannels = (short)waveReader.getNumChannels();
            Console.WriteLine("numchannels" + numChannels);
            int sampleRate = (int)waveReader.getSampleRate();
            Console.WriteLine("samplerate" + sampleRate);
            int byteRate = (int)waveReader.getByteRate();
            Console.WriteLine("byterate" + byteRate);
            int numSamples = bufferWaveData.Length;
            Console.WriteLine("numsamples" + numSamples);
            short blockAlign = (short)waveReader.getBlockAlign();
            Console.WriteLine("blockalign" + blockAlign);
            int subChunk2Size = waveReader.getSubChunk2Size();
            Console.WriteLine("subchunk2size" + subChunk2Size);
            int chunkSize = waveReader.getChunkSize();
            Console.WriteLine("chunksize" + chunkSize);

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
            wr.Write(numSamples);

            for (int i = 0; i < numSamples; i++)
            {
                wr.Write(bufferWaveData[i]);
            }

            wr.Close();
            wr.Dispose();
        }

        /// <summary>
        /// Play button to send the play message to the dll
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Button event</param>
        private void button8_Click(object sender, EventArgs e)
        {
            playData();
        }


        /// <summary>
        /// Record button to send the record message to the dll
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Button event</param>
        private void button2_Click(object sender, EventArgs e)
        {
            recordData();
        }

        /// <summary>
        /// Stop Record button to send the stop record message to the dll
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Button event</param>
        private void button3_Click(object sender, EventArgs e)
        {
            stopRecordData();

            byte* pSaveBuffer = (byte*)getPSaveBuffer();
            uint sizePBuffer = getSizePSaveBuffer();
            byte[] buffer = new byte[sizePBuffer];
            for (int i = 0; i < sizePBuffer; i++)
            {
                buffer[i] = *(pSaveBuffer + i * sizeof(byte));
            }
            waveReader = new WavReader(0, 36 + buffer.Length, 0, 0, 16, 1, 1, 44100, 44100, 2, 16, 0, buffer.Length);

            short[] shortBuffer = new short[buffer.Length / waveReader.getBlockAlign()];
            for (int i = 0; i < shortBuffer.Length; i++)
            {
                shortBuffer[i] = BitConverter.ToInt16(buffer, waveReader.getBlockAlign() * i);
            }
            double[] newData = shortBuffer.Select(x => (double)(x)).ToArray();

            initWaveData(newData);
            initBufferData(buffer);

            setPlot(waveData);
            plotWaveform(waveData);
        }

        /// <summary>
        /// Pause button to send the pause message to the dll
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Button event</param>
        private void button9_Click(object sender, EventArgs e)
        {
            pauseData();
        }

        /// <summary>
        /// Stop playback button to send the stop play message to the dll
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Button event</param>
        private void button10_Click(object sender, EventArgs e)
        {
            stopPlayData();
        }

        /// <summary>
        /// Button to open a DFT in a seperate window
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Button event</param>
        private void button12_Click(object sender, EventArgs e)
        {
            int numberOfSamples = Highlighted.Length;
            double[] selectedSamples = new double[numberOfSamples];
            int startSelect = (int)x1;
            for (int i = startSelect; i < startSelect + numberOfSamples && i < waveData.Length; i++)
            {
                selectedSamples[i - startSelect] = waveData[i];
            }
            complex[] A = Fourier.DFT(selectedSamples, selectedSamples.Length);
            for (int i = 0; i < A.Length; i++)
            {
                selectedSamples[i] = Math.Sqrt((A[i].im * A[i].im) + (A[i].re * A[i].re));
            }
            initFreqPlot(selectedSamples, A);
        }

        /// <summary>
        /// Call Frequency Domain Form
        /// </summary>
        /// <param name="selectedSamples">The selected samples are passed to the frequency domain form to display the DFT</param>
        /// <param name="A">The DFT of the selected samples</param>
        private void initFreqPlot(double[] selectedSamples, complex[] A)
        {
            FrequencyDomain freqPlot = new FrequencyDomain();
            freqPlot.Show();
            freqPlot.dftFreqChart(selectedSamples, (int) waveReader.getSampleRate(), A, waveData, this);
        }

        /// <summary>
        /// Triangular Windowing function
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Button event</param>
        private void button11_Click(object sender, EventArgs e)
        {
            freqChart.ChartAreas[0].CursorX.IsUserEnabled = true;
            freqChart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            freqChart.ChartAreas[0].AxisX.ScaleView.Zoomable = false;
            int N = Highlighted.Length;
            double[] selectedSamples = new double[N];
            for (int i = 0; i < N; i++)
            {
                if (i<=N/2)
                {
                    selectedSamples[i] = i / Math.Truncate((double)N / 2);
                } else
                {
                    selectedSamples[i] = 2 - (i / Math.Truncate((double)N / 2));
                }
                selectedSamples[i] *= Highlighted[i];
            }
            windowDFT = Fourier.DFT(selectedSamples, selectedSamples.Length);
            freqChart.Series["Series1"].Points.Clear();
            freqChart.Series["Series2"].Points.Clear();
            double scale = 0;
            for (int i = 0; i < windowDFT.Length; i++)
            {
                selectedSamples[i] = Math.Sqrt((windowDFT[i].im * windowDFT[i].im) + (windowDFT[i].re * windowDFT[i].re));
                freqChart.Series["Series1"].Points.AddXY(i, selectedSamples[i]);
                if (scale < selectedSamples[i])
                {
                    scale = selectedSamples[i];
                }
            }
            freqChart.ChartAreas[0].AxisY.Maximum = scale * 2;
        }

        /// <summary>
        /// Low-Pass function to create a low-pass filter and perform convolution
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Button event</param>
        private void button13_Click(object sender, EventArgs e)
        {
            int sampleRate = (int)waveReader.getSampleRate();
            int filterSize = (int)x4;
            double fcut = x3 * sampleRate / filterSize;

            complex[] filter = Filtering.lowPassFilter(filterSize, fcut, sampleRate);
            double[] fw = Fourier.inverseDFT(filter, filterSize);
            for (int i = 0; i < fw.Length; i++)
            {
                fw[i] /= filterSize;
            }
            windowedData = Filtering.convolution(fw, waveData);
            freqChart.Series["Series2"].Points.Clear();
            for (int i = (int)x1; i <(int)x2; i++)
            {
                freqChart.Series["Series2"].Points.AddXY(i - x1, windowedData[i]);
            }
        }

        /// <summary>
        /// High-Pass function to create a high-pass filter and perform convolution
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Button event</param>
        private void button14_Click(object sender, EventArgs e)
        {
            int sampleRate = (int)waveReader.getSampleRate();
            int filterSize = (int)x4;
            double fcut = x3 * sampleRate / filterSize;

            complex[] filter = Filtering.highPassFilter(filterSize, fcut, sampleRate);
            double[] fw = Fourier.inverseDFT(filter, filterSize);
            for (int i = 0; i < fw.Length; i++)
            {
                fw[i] /= filterSize;
            }
            windowedData = Filtering.convolution(fw, waveData);
            freqChart.Series["Series2"].Points.Clear();
            for (int i = (int)x1; i < (int)x2; i++)
            {
                freqChart.Series["Series2"].Points.AddXY(i - x1, windowedData[i]);
            }
        }

        /// <summary>
        /// DFT Rectangle Windowing, to display on the main window and allow filtering
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Button event</param>
        private void button16_Click(object sender, EventArgs e)
        {
            freqChart.ChartAreas[0].CursorX.IsUserEnabled = true;
            freqChart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            freqChart.ChartAreas[0].AxisX.ScaleView.Zoomable = false;
            int N = Highlighted.Length;
            double[] selectedSamples = new double[N];
            for (int i = 0; i < N; i++)
            {
                selectedSamples[i] = Highlighted[i];
            }
            windowDFT = Fourier.DFT(selectedSamples, selectedSamples.Length);
            freqChart.Series["Series1"].Points.Clear();
            freqChart.Series["Series2"].Points.Clear();
            for (int i = 0; i < windowDFT.Length; i++)
            {
                selectedSamples[i] = Math.Sqrt((windowDFT[i].im * windowDFT[i].im) + (windowDFT[i].re * windowDFT[i].re));
                freqChart.Series["Series1"].Points.AddXY(i, selectedSamples[i]);
            }
        }

        /// <summary>
        /// Apply windowing filter to the wave data
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Button event</param>
        private void button15_Click(object sender, EventArgs e)
        {
            waveData = windowedData;
            setPlot(waveData);
            plotWaveform(waveData);

            if (waveReader.bitsPerSample > 16)
            {
                bufferWaveData = waveData.Select(x => Convert.ToInt32(x))
                                  .SelectMany(x => BitConverter.GetBytes(x))
                                  .ToArray();
            } else if (waveReader.bitsPerSample > 8)
            {
                bufferWaveData = waveData.Select(x => Convert.ToInt16(x))
                                  .SelectMany(x => BitConverter.GetBytes(x))
                                  .ToArray();
            } else
            {
                for (int i = 0; i < waveData.Length; i++)
                {
                    bufferWaveData[i] = (byte)(waveData[i] + 128);
                }
            }

            fixed (byte* array = bufferWaveData)
            {
                setPSaveBuffer(array, bufferWaveData.Length, (int)waveReader.getSamplesPerSecond(), (short)waveReader.getBlockAlign(), (short)waveReader.getBitsPerSample(), (short)waveReader.getNumChannels());
            }
        }

        /// <summary>
        /// Hann Windowing function
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Button event</param>
        private void button17_Click(object sender, EventArgs e)
        {
            freqChart.ChartAreas[0].CursorX.IsUserEnabled = true;
            freqChart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            freqChart.ChartAreas[0].AxisX.ScaleView.Zoomable = false;
            int N = Highlighted.Length;
            double[] selectedSamples = new double[N];
            for (int n = 0; n < N; n++)
            {
                selectedSamples[n] = 0.5 * (1 - Math.Cos(2 * Math.PI * n / N));
                selectedSamples[n] *= Highlighted[n];
            }
            windowDFT = Fourier.DFT(selectedSamples, selectedSamples.Length);
            freqChart.Series["Series1"].Points.Clear();
            freqChart.Series["Series2"].Points.Clear();
            double scale = 0; ;
            for (int i = 0; i < windowDFT.Length; i++)
            {
                selectedSamples[i] = Math.Sqrt((windowDFT[i].im * windowDFT[i].im) + (windowDFT[i].re * windowDFT[i].re));
                freqChart.Series["Series1"].Points.AddXY(i, selectedSamples[i]);
                if (scale<selectedSamples[i])
                {
                    scale = selectedSamples[i];
                }
            }
            freqChart.ChartAreas[0].AxisY.Maximum = scale*2;
        }
    }
}
