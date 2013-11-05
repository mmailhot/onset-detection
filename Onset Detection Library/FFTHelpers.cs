using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onset_Detection_Library
{
    internal sealed class FFTHelpers
    {
        //Hamming Window Code
        private static double[] _hammingWindow;

        internal static double[] HammingWindow(int length)
        {
            if (_hammingWindow == null || _hammingWindow.Length != length)
            {
                _hammingWindow = GenerateHammingWindow(length);
            }
            return _hammingWindow;
        }

        private static double[] GenerateHammingWindow(int length)
        {
            double[] window = new double[length];
            for(int i = 0; i < length; i++){
                window[i] = 0.54 - 0.46 * Math.Cos((2 * Math.PI * i)/(length - 1));
            }
            return window;
        }
    }
}
