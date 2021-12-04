using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _3931_Project_windows_forms
{
    internal class CopyPaste
    {
        ///<summary>
        ///Inserts the copied array into the original waveform, overwriting the areas between x1 and x2
        ///</summary>
        ///<param name="original">The initial array of doubles, representing the waveform</param>
        ///<param name="copied">The array of doubles, copied from the original array</param>
        ///<param name="x1">The point in the array that you start pasting to</param>
        ///<param name="x2">The point in the array that you stop pasting to</param>
        ///<returns>The new array to be charted</returns>
        public static double[] Paste(double[] original, double[] copied, double x1, double x2)
        {
            if (copied==null)
            {
                return original;
            }
            
            double[] newWave = new double[original.Length + copied.Length];

            for (int i = 0; i < newWave.Length; i++)
            {
                if (i<(int)x1)
                {
                    newWave[i] = original[i];
                } else if (i<(int)x1+copied.Length)
                {
                    newWave[i] = copied[i-(int)x1];
                } if (i >= x2 && i < original.Length)
                {
                    newWave[i+copied.Length-(int)(x2-x1)] = original[i];
                }
            }
            return newWave;
        }
        ///<summary>
        ///Removes a section of the original waveform between x1 and x2
        ///</summary>
        ///<param name="original">The initial array of doubles, representing the waveform</param>
        ///<param name="selection">The selected array of doubles from the initial waveform</param>
        ///<param name="x1">The point in the array that you start removing</param>
        ///<param name="x2">The point in the array that you stop removing</param>
        ///<returns>The new array to be charted</returns>
        public static double[] Cut(double[] original, double[] selection, double x1, double x2)
        {
            double[] newWave = new double[original.Length - selection.Length];
            for (int i = 0; i < x1; i++)
            {
                newWave[i] = original[i];
            }
            for (int i = 0; i < original.Length - x2; i++)
            {
                newWave[(int)x1 + i] = original[(int)x2 + i];
            }
            return newWave;
        }
    }
}
