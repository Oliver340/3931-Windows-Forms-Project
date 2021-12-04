using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3931_Project_windows_forms
{
    class Filtering
    {
        /// <summary>
        /// Filters out waves using convolution
        /// </summary>
        /// <param name="fw">An array of doubles to be applied to the original waveform as a filter</param>
        /// <param name="s">The original waveform</param>
        /// <returns>A filtered array of doubles</returns>
        public static double[] convolution(double[] fw, double[] s)
        {
            double sum;
            int sizeOfSamplesWithZero = s.Length + fw.Length - 1;
            double[] newSamples = new double[sizeOfSamplesWithZero];
            for (int i = 0; i < sizeOfSamplesWithZero; i++)
            {
                newSamples[i] = 0;
            }
            for (int i = 0; i < s.Length; i++)
            {
                newSamples[i] = s[i];
            }

            for (int i = 0; i < s.Length; i++)
            {
                sum = 0;
                for (int j = 0; j < fw.Length; j++)
                {
                    sum += newSamples[i + j] * fw[j];
                }
                s[i] = sum;
            }
            return s;
        }
        /// <summary>
        /// Generates a group of ones on the ends of a complex array
        /// </summary>
        /// <param name="filterSize">The size of the array</param>
        /// <param name="fcut">The value that determines how many ones will be included in the array</param>
        /// <param name="sampleRate">A value to keep the array from having an overflow error</param>
        /// <returns>An complex array of "real" ones on either end</returns>
        public static complex[] lowPassFilter(int filterSize, double fcut, int sampleRate)
        {
            int amountOfOnes = (int)Math.Ceiling((double)(fcut * filterSize / sampleRate));

            complex[] filter = new complex[filterSize];
            filter[0].re = 1;
            filter[0].im = 0;
            for (int i = 1; i < amountOfOnes + 1 && i < filterSize && i >= 0; i++)
            {
                filter[i].re = 1;
                filter[i].im = 0;
            }
            for (int i = filterSize - 1; i >= filterSize - amountOfOnes && i < filterSize && i >= 0; i--)
            {
                filter[i].re = 1;
                filter[i].im = 0;
            }
            return filter;
        }
        /// <summary>
        /// Generates a group of ones in the middle of a complex array
        /// </summary>
        /// <param name="filterSize">The size of the array</param>
        /// <param name="fcut">The value that determines how many ones will be included in the array</param>
        /// <param name="sampleRate">A value to keep the array from having an overflow error</param>
        /// <returns>An complex array of "real" ones in the center</returns>
        public static complex[] highPassFilter(int filterSize, double fcut, int sampleRate)
        {
            int amountOfOnes = (int)Math.Ceiling((double)(fcut * filterSize / sampleRate));

            complex[] filter = new complex[filterSize];
            filter[0].re = 1;
            filter[0].im = 0;
            for (int i = amountOfOnes + 1; i < filterSize - amountOfOnes && i < filterSize && i >= 0; i++)
            {
                filter[i].re = 1;
                filter[i].im = 0;
            }
            return filter;
        }
    }
}
