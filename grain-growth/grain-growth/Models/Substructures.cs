using System;
using System.Collections.Generic;
using System.Drawing;

namespace grain_growth.Models
{
    public class Substructures
    {
        public Random Random = new Random();

        public static List<Point> SubStrucrtuePointsList;
    }

    public enum SubstructuresType
    {
        Substructure = -3,
        DualPhase = -4
    }
}
