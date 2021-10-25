using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3931_Project_windows_forms
{
    internal class CopyPaste
    {
        public static double[] Copy(complex[] a, int start, int end)
        {
            if (start > end)
            {
                int temp = start;
                start = end;
                end = temp;
            }
            complex[] complices= new complex[end-start];
            for (int i = start; i < end; i++)
            {
                complices[i-start] = a[i];
            }
            return Fourier.inverseDFT(complices, complices.Length);
        }
        public static complex[] Paste(complex[] a, double[] s, int location)
        {
            complex[] clipboard = Fourier.DFT(s, s.Length);
            complex[] newWave = new complex[a.Length + clipboard.Length];
            for (int i = 0; i < newWave.Length; i++)
            {
                if (i < location)
                {
                    newWave[i] = a[i];
                }
                else if (i < location + clipboard.Length)
                {
                    newWave[i] = clipboard[i - location];
                }
                else
                {
                    newWave[i] = a[i - location];
                }
            }
            return newWave;
        }

        public static double[] Cut(complex[] a, int start, int end)
        {
            double[] clipboard = Copy(a, start, end);
            complex[] newA = new complex[a.Length - end + start];
            for (int i = 0; i < a.Length; i++)
            {
                while (i > start && i < end)
                {
                    i++;
                }
                if (i < end)
                {
                    newA[i] = a[i];
                } else
                {
                    newA[i-end+start] = a[i];
                }
            }
            a = newA;
            return clipboard;
        }
    }
}
