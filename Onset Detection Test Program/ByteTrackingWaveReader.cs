using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onset_Detection_Test_Program
{
    public class ByteTrackingWaveReader:NAudio.Wave.WaveFileReader
    {
        public long BytesRead { get; private set; }

        public ByteTrackingWaveReader(string filename) : base(filename) {
            BytesRead = 0;
        }

        public override int Read(byte[] array, int offset, int count)
        {
            BytesRead += count;
            return base.Read(array, offset, count);
        }
    }
}
