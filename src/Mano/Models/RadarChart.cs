using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mano.Models
{
    public class RadarChartDataSet
    {
        public string fillColor { get; set; }
        public string strokeColor { get; set; }
        public string pointColor { get; set; }
        public string pointStrokeColor { get; set; }
        public List<long> data { get; set; } = new List<long>();
    }

    public class RadarChart
    {
        public List<string> labels { get; set; } = new List<string>();
        public List<RadarChartDataSet> datasets { get; set; } = new List<RadarChartDataSet>();
    }
}
