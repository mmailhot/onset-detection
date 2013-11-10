using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Onset_Detection_Library;
using NAudio;
using NAudio.Wave;
using System.Threading;

namespace Onset_Detection_Test_Program
{
    class Program
    {
        static void Main(string[] args)
        {
            const string filename = @"D:\DaylightShort.wav";

            Stopwatch timer = new Stopwatch();
            timer.Start();
            AudioData audio = AudioData.LoadFromWAV(filename);
            OnsetData onsets = new OnsetData(audio, 1024);
            onsets.Process();
            timer.Stop();

            List<int> temp = new List<int>(onsets.PeakFrames);

            using(var waveFileReader = new ByteTrackingWaveReader(filename))
            using(WaveChannel32 wc = new WaveChannel32(waveFileReader){PadWithZeroes = false})
            using(var audioOutput = new DirectSoundOut())
            {
                audioOutput.Init(wc);
                audioOutput.Play();
                
                
                while(audioOutput.PlaybackState != PlaybackState.Stopped){

                    int position = (int)(wc.CurrentTime.TotalMilliseconds / 1000 / 1024 * onsets.Audio.SampleRate);
                    if (temp.Count > 0 && position > temp[0])
                    {
                        Console.Out.WriteLine("Beat at index " + temp[0]);
                        temp.Remove(temp[0]);
                    }

                    Thread.Sleep(1);
                }

                audioOutput.Stop();
            }

        }

    }
}
