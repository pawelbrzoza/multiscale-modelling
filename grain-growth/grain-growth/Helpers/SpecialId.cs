using System;

namespace grain_growth.Models
{
    public class SpecialId
    {
        public static bool IsIdSpecial(int id)
        {
            return Enum.IsDefined(typeof(Id), id);
        }

        public enum Id
        {
            Empty = 0,
            Border = -1,
            Inclusion = -2,
            Substructure = -3,
            DualPhase = -4,
            //Nucleon = -5
        }
    }
}
