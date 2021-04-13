using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetFramework_Algorithm_Chart_Start
{
    // stockObj class for holding the specific information of a ticker symbol when a pattern is found
    public class stockObj
    {
        public int id { get; set; }
        public string symbol { get; set; }
        public string startdate { get; set; }
        public string enddate { get; set; }
        public int interval { get; set; }
        public string name { get; set; }
        public string image { get; set; }
    }
}
