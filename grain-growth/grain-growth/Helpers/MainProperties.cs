
namespace grain_growth.Models
{
    public class MainProperties
    {

        public int RangeWidth { get; set; }

        public int RangeHeight { get; set; }

        // Monte Carlo
        public int MCS { get; set; }

        // Cellular Automata
        public int AmountOfGrains { get; set; }

        public int GrowthProbability { get; set; }

        public NeighbourhoodType NeighbourhoodType { get; set; }

        public SubstructuresType SubstructuresType { get; set; }

        public MethodType MethodType { get; set; }
    }

    public enum MethodType {
        CellularAutomata = 0,
        MonteCarlo = 1
    }
}
