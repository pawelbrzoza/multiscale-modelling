
namespace grain_growth.Models
{
    public class Boundaries
    {
        public Range ClearBoundaries { get; set; }

        public Range BoundariesAll { get; set; }

        public Range BoundariesSelected { set; get; }

        public Range BoundariesSingleSelect { set; get; }
    }
}