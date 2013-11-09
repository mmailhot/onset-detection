using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


namespace Onset_Detection_Library
{
    public class OnsetData
    {
        public AudioData Audio { get; private set; }
        public int BlockSize { get; private set; }
        public double[] SpectralFlux { get; private set; }
        int _numberOfBlocks;
        const double _thresholdMultiplier = 1.5;
        const int _thresholdSize = 10;

        public OnsetData(AudioData audio, int blockSize)
        {
            Audio = audio;
            BlockSize = blockSize;
            _numberOfBlocks = Audio.Data.Length / BlockSize;
        }

        public void Process()
        {
            SpectralFlux = new double[_numberOfBlocks];
            Complex[] prevData = null;
            //Transform + Find Spectral Flux
            for (int i = 0; i < _numberOfBlocks; i++)
            {
                Complex[] fftData = FFT.FastTransform(Audio.Data, i * BlockSize,BlockSize);

                if (prevData != null)
                {
                    SpectralFlux[i] = CalculateSpectralFlux(prevData, fftData);
                }
                else
                {
                    SpectralFlux[i] = 0.0;
                }

                prevData = fftData;
            }
            //Calculate Thresholds
            var thresholds = CalculateThresholds(SpectralFlux);

            //Prune Values
            var pruned_flux = PruneValues(SpectralFlux, thresholds);
        }

        private double CalculateSpectralFlux(Complex[] oldData, Complex[] newData){
            double flux = 0.0;
            for (int i = 0; i < oldData.Length / 2; i++)
            {
                double freqFlux =  newData[i].Magnitude - oldData[i].Magnitude;
                if (freqFlux > 0)
                {
                    flux += freqFlux;
                }
            }
            return flux;
        }

        private double[] CalculateThresholds(double[] flux)
        {
            double[] thresholds = new double[flux.Length];

            for (int i = 0; i < thresholds.Length; i++)
            {
                //Calculate Max/Min indexes for this average
                int max = i + _thresholdSize < flux.Length ? i + _thresholdSize : flux.Length - 1;
                int min = i > _thresholdSize ? i - _thresholdSize : 0;
                int num_vals = max - min + 1;
                double total = 0;

                //Sum the flux values
                for (int j = min; j < max; j++)
                {
                    total += flux[j];
                }

                //Find the mean
                thresholds[i] = total / num_vals;
                thresholds[i] *= _thresholdMultiplier;
            }

            return thresholds;
        }

        private double[] PruneValues(double[] flux, double[] thresholds)
        {
            //TODO: Test whether it works better if I don't subtract the threshold
            double[] pruned = new double[flux.Length];

            for (int i = 0; i < flux.Length; i++)
            {
                if (flux[i] > thresholds[i])
                {
                    pruned[i] = flux[i] - thresholds[i];
                }
                else
                {
                    pruned[i] = 0.0;
                }
            }

            return pruned;
        }

    }
}
