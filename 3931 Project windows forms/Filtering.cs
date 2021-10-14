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
            int difference = s.Length % fw.Length;
            int sizeOfSamplesWithZero = s.Length + difference + 1;
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
    }
}
