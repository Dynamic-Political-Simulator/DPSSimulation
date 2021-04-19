using System;
using System.Collections.Generic;
using System.Text;

namespace DPSSimulation.Classes
{
    public class Building
    {
        public int BuildingId { get; set; }
        public int BuildingGameId { get; set; }
        public string Type { get; set; }
        public bool ruined { get; set; }
    }
}
