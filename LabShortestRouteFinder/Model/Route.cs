using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LabShortestRouteFinder.Model
{
    public class Route
    {
        private double cost;
        public required CityNode Start {  get; set; }
        public required CityNode Destination { get; set; }
        public int DrivingDistance { get; set; }
        public double StraightLineDistance { get; set; }
        public double Cost { 
            get
            {
                return Convert.ToDouble((this.DrivingDistance / 100) * 15);
            }
            set
            {
                cost = value;

            }
        }
        // TODO add method for DrivingDistance change event to recalculate cost
        // TODO add serializer to save Route changes to json file
    }
}
