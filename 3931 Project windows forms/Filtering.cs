using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3931_Project_windows_forms
{
    class Filtering
    {
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
                //s[i] = sum / fw.Length;
            }
            return s;
        }

        public static complex[] lowPassFilter(int filterSize, double fcut, int sampleRate)
        {
            int amountOfOnes = (int)Math.Ceiling((double)(fcut * filterSize / sampleRate));

            complex[] filter = new complex[filterSize];
            filter[0].re = 1;
            for (int i = 1; i < amountOfOnes + 1 && i < filterSize; i++)
            {
                filter[i].re = 1;
                filter[i].im = 0;
            }
            for (int i = filterSize - 1; i >= filterSize - amountOfOnes; i--)
            {
                filter[i].re = 1;
                filter[i].im = 0;
            }
            return filter;
        }

        public static complex[] highPassFilter(int filterSize, double fcut, int sampleRate)
        {
            int amountOfOnes = (int)Math.Ceiling((double)(fcut * filterSize / sampleRate));

            complex[] filter = new complex[filterSize];
            filter[0].re = 1;
            for (int i = amountOfOnes + 1; i < filterSize - amountOfOnes && i < filterSize; i++)
            {
                filter[i].re = 1;
            }
            return filter;
        }
    }
}
