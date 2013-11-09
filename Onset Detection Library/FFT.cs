using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.IntegralTransforms;

namespace Onset_Detection_Library
{
    public class FFT
    {
        public static double[] SlowTransform(double[] samples)
        {
            //Length in real values
            var n = samples.Length;

            //Check to make sure it is a power of two length
            if ((n & (n - 1)) != 0)
                throw new ArgumentException("Data is not in a power of two");
            
            //Copy array to one with blanks for the complex numbers
            var output = new double[n * 2];
            for (int i = 0; i < n; i++)
                output[2 * i] = samples[i];
            
            //Bit-Reversal Swap
            int j = 0;
            int m = 0;
            double temp;
            for (int i = 0; i < n; i += 2)
            {
                if (j > i)
                {
                    //Swap real part
                    temp = output[j];
                    output[j] = output[i];
                    output[i] = temp;

                    //Swap complex part
                    temp = output[j + 1];
                    output[j + 1] = output[i + 1];
                    output[i + 1] = temp;

                    if ((j / 2) < (n / 2))
                    {
                        //Swap real part
                        temp = output[(2 * n) - (i + 2)];
                        output[(2 * n) - (i + 2)] = output[(2 * n) - (j + 2)];
                        output[(2 * n) - (j + 2)] = temp;

                        //Swap complex part
                        temp = output[(2 * n) - (i + 2) + 1];
                        output[(2 * n) - (i + 2) + 1] = output[(2 * n) - (j + 2) + 1];
                        output[(2 * n) - (j + 2) + 1] = temp;
                    }
                }
                m = n;
                while (m >= 2 && j >= m)
                {
                    j -= m;
                    m = m / 2;
                }
                j += m;
            }

            //Danielson-Lanzcos routine AKA the actual FFT bit
            int mmax = 2;
            int istep;
            double theta,wtemp,wpr,wpi,wr,wi,tempr,tempi;
            while ((2 * n) > mmax)
            {
                istep = mmax << 1;
                theta = -(2 * Math.PI / mmax);
                wtemp = Math.Sin(0.5 * theta);
                wpr = -2.0 * wtemp * wtemp; //Why does C# not have a proper power operator
                wpi = Math.Sin(theta);
                wr = 1.0;
                wi = 0.0;
                for (m = 1; m < mmax; m += 2)
                {
                    for (int i = m; i <= (n * 2); i += istep)
                    {
                        j = i + mmax;
                        tempr = wr * output[j - 1] - wi * output[j];
                        tempi = wr * output[j] + wi * output[j-1];
                        output[j - 1] = output[i - 1] - tempr;
                        output[j] = output[i] - tempi;
                        output[i - 1] += tempr;
                        output[i] += tempi;
                    }
                    wr = (wtemp = wr) * wpr - wi * wpi + wr;
                    wi = wi * wpr + wtemp * wpi + wi;
                }
                mmax = istep;
            }

            return output;

        }

        public static double[] GenerateTestData(int length)
        {
            double[] output = new double[length];
            for (int i = 0; i < length; i++)
            {
                output[i] = Math.Sin(i);
            }
            return output;
        }

        public static Complex[] FastTransform(double[] samples,int offset, int length)
        {
            var complexSamples = new Complex[length];
            double[] window = FFTHelpers.HammingWindow(length);

            for (int i = 0; i < length; i++)
                complexSamples[i] = new Complex(samples[i+offset] * window[i], 0);

            Transform.FourierForward(complexSamples);
            return complexSamples;
        }
    }
}
