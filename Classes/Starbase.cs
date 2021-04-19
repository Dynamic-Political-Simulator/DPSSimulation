using System;
using System.Collections.Generic;
using System.Text;

namespace DPSSimulation.Classes
{
    public class Starbase
    {
        public int StarbaseId { get; set; }
        public int Owner { get; set; }
        public string Level { get; set; }
        public List<string> Modules { get; set; } = new List<string>();
        public List<string> Buildings { get; set; } = new List<string>();
        public Fleet StarbaseFleet { get; set; }

        
    }
}
