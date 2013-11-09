using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Stopwatch timer = new Stopwatch();
            timer.Start();
            AudioData audio = AudioData.LoadFromWAV(@"D:\DaylightMono.wav");
            OnsetData onsets = new OnsetData(audio,1024);
            onsets.Process();
            timer.Stop();
        }
    }
}
