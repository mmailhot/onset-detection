using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnsetUI
{
    class VisualizerData
    {
        public int startIndex { get; set; }
        public List<int> indices { get; private set; }

        public VisualizerData(List<int> data)
        {
            indices = data;
            startIndex = 0;
        }

        public void Reset()
        {
            startIndex = 0;
        }
    }
}
