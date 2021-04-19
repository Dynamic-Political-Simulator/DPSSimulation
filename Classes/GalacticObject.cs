using System;
using System.Collections.Generic;
using System.Text;

namespace DPSSimulation.Classes
{
    public class GalacticObject
    {
      
        public int GalacticObjectGameId { get; set; }
        public float PosX { get; set; }
        public float PosY { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public List<Planet> Planets { get; set; } = new List<Planet>();
        public List<Hyperlane> Hyperlanes { get; set; } = new List<Hyperlane>();
        public Starbase Starbase { get; set; }
    }
}
