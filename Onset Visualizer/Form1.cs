using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;
using Onset_Detection_Library;

namespace Onset_Visualizer
{
    public partial class Form1 : Form
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Message
        {
            public IntPtr hWnd;
            public System.Windows.Forms.Message msg;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public System.Drawing.Point p;
        }

        [System.Security.SuppressUnmanagedCodeSecurity] // We won't use this maliciously
        [DllImport("User32.dll", CharSet=CharSet.Auto)]
        public static extern bool PeekMessage(out Message msg, IntPtr hWnd, uint messageFilterMin, uint messageFilterMax, uint flags);

        WaveFileReader waveFileReader ;
        WaveChannel32 wc;
        DirectSoundOut audioOutput;
        OnsetData onsets;

        public Form1()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            const string filename = @"D:\DaylightShort.wav";

            AudioData audio = AudioData.LoadFromWAV(filename);
            onsets = new OnsetData(audio, 1024);
            onsets.Process();

            waveFileReader = new WaveFileReader(filename);
            wc = new WaveChannel32(waveFileReader) { PadWithZeroes = false };
            audioOutput = new DirectSoundOut();

            audioOutput.PlaybackStopped += audioOutput_PlaybackStopped;
            audioOutput.Init(wc);
            audioOutput.Play();
            System.Windows.Forms.Application.Idle += new EventHandler(OnApplicationIdle);
        }

        private void audioOutput_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            audioOutput.Stop();
        }

        private void OnApplicationIdle(object sender, EventArgs e)
        {
            while (AppStillIdle)
            {
                int position = (int)(wc.CurrentTime.TotalMilliseconds / 1000 / 1024 * onsets.Audio.SampleRate);
                int index = -1;
                for (int i = 0; i < onsets.PeakFrames.Count; i++)
                {
                    if (onsets.PeakFrames[i] > position)
                    {
                        break;
                    }
                    index = i;
                }
                if (index > -1)
                {
                    int delay = position - onsets.PeakFrames[index];
                    Console.WriteLine(delay);
                    if (delay < 20)
                    {
                        this.BackColor = System.Drawing.Color.FromArgb(0, 255 - (10 * delay), 0);
                    }
                    else
                    {
                        this.BackColor = System.Drawing.Color.FromArgb(0, 0, 0);
                    }
                    
                }
            }
        }

        private bool AppStillIdle
        {
            get
            {
                Message msg;
                return !PeekMessage(out msg, IntPtr.Zero, 0, 0, 0);
            }
        }
    }
}
