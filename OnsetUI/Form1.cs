using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;
using Onset_Detection_Library;

namespace OnsetUI
{
    public partial class form1 : Form
    {
        string filename;
        OnsetData data;
        WaveFileReader waveFileReader;
        WaveChannel32 wc;
        DirectSoundOut audioOutput;
        Pen backgroundPen = new Pen(Color.SlateGray, 1);
        Pen mainPen = new Pen(Color.Red, 4);
        VisualizerData visData;

        public form1()
        {
            InitializeComponent();
        }


        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            DialogResult result = openDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                data = null;
                lblStatus.Text = "No Data Processed";
                filename = openDialog.FileName;
                lblFilename.Text = filename;
                btnProcess.Enabled = true;
            }
            btnPlay.Enabled = false;
        }

        private async void btnProcess_Click(object sender, EventArgs e)
        {
            Task<OnsetData> task = Task.Run(() => processAudio(filename));

            btnLoadFile.Enabled = false;
            btnProcess.Enabled = false;
            btnPlay.Enabled = false;

            data = await task;

            btnLoadFile.Enabled = true;
            btnProcess.Enabled = true;

            if (data != null)
            {
                lblStatus.Text = "Audio Processed";
                visData = new VisualizerData(data.PeakFrames);
                btnPlay.Enabled = true;
            }
            else
            {
                lblStatus.Text = "Processing Failed";
            }
        }

        private OnsetData processAudio(string filename)
        {
            try { 
                AudioData audio = AudioData.LoadFromWAV(filename);
                OnsetData data = new OnsetData(audio, 1024);
                data.Process();
                return data;
            }
            catch {
                return null;
            }

        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            btnLoadFile.Enabled = false;
            btnProcess.Enabled = false;
            btnPlay.Enabled = false;
            visData.Reset();
            waveFileReader = new WaveFileReader(filename);
            wc = new WaveChannel32(waveFileReader) { PadWithZeroes = false };
            audioOutput = new DirectSoundOut();
            audioOutput.Init(wc);
            audioOutput.Play();
            tmrRedraw.Enabled = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            audioOutput.Stop();
            audioOutput.Dispose();
            wc.Dispose();
            waveFileReader.Dispose();
            backgroundPen.Dispose();
            mainPen.Dispose();
        }

        private void PaintSurface(object sender, PaintEventArgs e)
        {
            if (audioOutput != null) {
                if (audioOutput.PlaybackState != PlaybackState.Stopped)
                {
                    e.Graphics.DrawLine(backgroundPen, 0, 100, 600, 100);
                    e.Graphics.DrawLine(backgroundPen, 300, 0, 300, 200);
                    double currentTime = wc.CurrentTime.TotalMilliseconds;
                    bool done = false;
                    int index = visData.startIndex;
                    while (!done && index < visData.indices.Count)
                    {
                        double indexTime = visData.indices[index] * 1000.0 * data.BlockSize / data.Audio.SampleRate;
                        int xPosition = position(currentTime, indexTime);
                        if (xPosition > 600)
                        {
                            visData.startIndex = index;
                        }else if(xPosition < 0){
                            done = true;
                        }
                        else
                        {
                            float strength = data.PeakStrength[index];
                            e.Graphics.DrawLine(mainPen, xPosition, 100 - (int)(150.0 * strength), xPosition, 100 + (int)(150.0 * strength));
                        }

                        index++;
                    }

                }
                else
                {
                    audioOutput.Stop();
                    tmrRedraw.Enabled = false;
                    btnLoadFile.Enabled = true;
                    btnProcess.Enabled = true;
                    btnPlay.Enabled = true;
                }
            }
            else
            {
                
            }
            
        }

        private void updateTimes()
        {
            int currentSeconds = wc.CurrentTime.Seconds;
            int currentMinutes = wc.CurrentTime.Minutes;

            int totalSeconds = wc.TotalTime.Seconds;
            int totalMinutes = wc.TotalTime.Minutes;

            lblTimecode.Text = String.Format("{0:D2}:{1:D2}/{2:D2}:{3:D2}", currentMinutes, currentSeconds, totalMinutes, totalSeconds);
        }

        private int position(double currentTime, double indexTime)
        {
            double difference = currentTime - indexTime ;
            return (int)(difference * 0.3 + 300.0);
        }

        private void tmrRedraw_Tick(object sender, EventArgs e)
        {
            drawingSurface.Refresh();
            updateTimes();
        }


    }
}
