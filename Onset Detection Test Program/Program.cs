using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Onset_Detection_Library;

namespace Onset_Detection_Test_Program
{
    class Program
    {
        static void Main(string[] args)
        {
            AudioData audio = AudioData.LoadFromWAV(@"D:\440Hz.wav");

            Complex[] temp2 = Onset_Detection_Library.FFT.FastTransform(audio.Data.Take(1024).ToArray());
            double largest2 = 0;
            int largestIndex2 = 0;
            for (int i = 0; i < temp2.Length / 2; i++)
            {
                double current = temp2[i].Magnitude;
                if (current > largest2)
                {
                    largest2 = current;
                    largestIndex2 = i;
                }
            }
        }
    }
}
