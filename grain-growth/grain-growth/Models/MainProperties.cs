using grain_growth.Helpers;

namespace grain_growth.Models
{
    public class MainProperties
    {
        public int NumberOfGrains { get; set; }

        public int RangeWidth { get; set; }

        public int RangeHeight { get; set; }

        public int GrowthProbability { get; set; }

        public InitInclusions Inclusions { get; set; }

        public NeighbourhoodType NeighbourhoodType { get; set; }

        public SubstructuresType SubstructuresType { get; set; }
    }
}
