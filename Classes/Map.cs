using System;
using System.Collections.Generic;

namespace DPSSimulation.Classes
{
    public class Map
    {
        public List<Building> Buildings { get; set; } = new List<Building>();
        public List<District> Districts { get; set; } = new List<District>();
        public List<Design> Designs { get; set; } = new List<Design>();
        public List<Empire> Empires { get; set; } = new List<Empire>();
       
        
        public Parser Parser { get; set; }

        public Map(string path)
        {
            Parser = new Parser(this);
            Parser.parseSave(path);

        }

        

    }
}
