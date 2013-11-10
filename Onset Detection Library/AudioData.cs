using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onset_Detection_Library
{
    public class AudioData
    {

        public double[] Data { get; private set; }
        public int SampleRate { get; private set; }
        public double Duration { get; private set; }

        public AudioData(double[] data, int sampleRate)
        {
            Data = data;
            SampleRate = sampleRate;
            Duration = (double)data.Length / sampleRate;
        }

        public static AudioData LoadFromWAV(string filename)
        {
            byte[] wav = File.ReadAllBytes(filename);
            byte numChannels = wav[22];
            int sampleRate = BytesToShort(wav, 24);
            int samples = (wav.Length - 44) / 2 / numChannels;
            double[] data = new double[samples];

            for (int i = 0; i < samples; i++)
            {
                double audioTotal = 0;
                for (int j = 0; j < numChannels; j++)
                {
                    audioTotal += BytesToNormalizedFloat(wav, (i*2*numChannels) + (2 * j));
                }
                data[i] = audioTotal / numChannels;
            }

            return new AudioData(data, sampleRate);
        }
        private static int BytesToShort(byte[] array, int index)
        {
            return System.BitConverter.ToInt32(array, index);
        }
        private static float BytesToNormalizedFloat(byte[] array, int index)
        {
            short value = System.BitConverter.ToInt16(array, index);
            return value / 32767.0f;
        }
    }
}
