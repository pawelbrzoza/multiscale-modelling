namespace grain_growth.Models
{ 
    public class InclusionsProperties
    {
        public bool IsEnable { get; set; }

        public int Number { get; set; }

        public int Size { get; set; }

        public InclusionsType InclusionsType { get; set; }

        public InclusionsCreationTime CreationTime { get; set; }
    }
}