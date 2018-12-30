
namespace grain_growth.Models
{ 
    public class Inclusions
    {
        public bool IsEnable { get; set; }

        public int AmountOfInclusions { get; set; }

        public int Size { get; set; }

        public InclusionsType InclusionsType { get; set; }

        public InclusionsCreationTime CreationTime { get; set; }
    }

    public enum InclusionsCreationTime
    {
        Begin = 0,
        After = 1
    }

    public enum InclusionsType
    {
        Square = 0,
        Circular = 1
    }
}