using System;
using System.Collections.Generic;
using System.Text;

namespace DPSSimulation.Classes
{
    public class PoliticalAlignment
    {
        public int Federalism { get; set; }
        public int Democracy { get; set; }
        public int Globalism { get; set; }
        public int Militarism { get; set; }
        public int Security { get; set; }
        public int Cooperation { get; set; }
        public int Secularism { get; set; }
        public int Progressivism { get; set; }
        public int Monoculturalism { get; set; }
        public int[,] Alignement { get; set; } = new int[9,2];
    }
}
