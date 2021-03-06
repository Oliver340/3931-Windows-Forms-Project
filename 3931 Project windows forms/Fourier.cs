using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3931_Project_windows_forms
{
    public struct complex
    {
        public double re;
        public double im;
    }

    public static class Fourier
    {
        /// <summary>
        /// Turn an array of doubles into an array of real and imaginary values
        /// </summary>
        /// <param name="s">The array to perform fourier on</param>
        /// <param name="N">The length of the array</param>
        /// <returns>An array of real and imaginary values</returns>
        public static complex[] DFT(double[] s, int N)
        {
            complex[] A = new complex[N];

            for (int f = 0; f < N; f++)
            {
                A[f] = new complex();
                A[f].im = 0;
                A[f].re = 0;
                for (int t = 0; t < N; t++)
                {
                    A[f].re += s[t] * Math.Cos(2.0 * Math.PI * t * f / N);
                    A[f].im -= s[t] * Math.Sin(2.0 * Math.PI * t * f / N);
                }
                A[f].re /= N;
                A[f].im /= N;
            }
            return A;
        }

        /// <summary>
        /// Turn an array of real and imaginary values into an array of doubles
        /// </summary>
        /// <param name="A">The array to perform inverse Fourier on</param>
        /// <param name="N">The length of the array</param>
        /// <returns>An array of doubles</returns>
        public static double[] inverseDFT(complex[] A, int N)
        {
            double[] s = new double[N];

            for (int t = 0; t < N; t++)
            {
                s[t] = 0;
                for (int f = 0; f < N; f++)
                {
                    s[t] += A[f].re * Math.Cos(2 * Math.PI * t * f / N);
                    s[t] -= A[f].im * Math.Sin(2 * Math.PI * t * f / N);
                }
            }
            return s;
        }
    }
}
