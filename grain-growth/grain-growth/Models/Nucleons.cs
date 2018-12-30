
namespace grain_growth.Models
{
    public class Nucleons
    {
        public bool IsEnable { get; set; }

        public int AmountOfNucleons { get; set; }

        public int EnergyInside { get; set; }

        public int EnergyOnEdges { get; set; }

        public EnergyDistribution EnergyDistribution { get; set; }

        public TypeOfNucleonsCreation TypeOfcreation { get; set; }
    }

    public enum TypeOfNucleonsCreation
    {
        Constant = 0,
        Increasing = 1,
        AtTheBeginning = 2
    }

    public enum EnergyDistribution
    {
        Homogenous = 0,
        Heterogenous = 1
    }
}
