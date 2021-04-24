using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemyUczace
{
    public class GainRatioMax
    {
        public GainRatioMax(int index = 0, double gainRatio = 0)
        {
            Index = index;
            GainRatio = gainRatio;
        }

        public int Index { get; set; }
        public double  GainRatio { get; set; }
    }
}
