using grain_growth.Helpers;
using System.Collections.Generic;
using System.Drawing;

namespace grain_growth.Models
{
    public class Boundaries
    {
        public Range ClearBoundaries { get; set; }

        public Range BoundariesWithBackground { get; set; }

        public Range BoundariesAllSelected { set; get; }

        public Range BoundariesSingleSelect { set; get; }

        public Boundaries(MainProperties properties) {

            ClearBoundaries = new Range(properties.RangeWidth, properties.RangeHeight);
            BoundariesWithBackground = new Range(properties.RangeWidth, properties.RangeHeight);
            BoundariesAllSelected = new Range(properties.RangeWidth, properties.RangeHeight);
            BoundariesSingleSelect = new Range(properties.RangeWidth, properties.RangeHeight);

            InitStructure.AddBlackBorder(ClearBoundaries);
            InitStructure.AddBlackBorder(BoundariesWithBackground);
            InitStructure.AddBlackBorder(BoundariesAllSelected);
            InitStructure.AddBlackBorder(BoundariesSingleSelect);

            // init grains array by default values
            for (int i = 1; i < properties.RangeWidth - 1; i++)
            {
                for (int j = 1; j < properties.RangeHeight - 1; j++)
                {
                    ClearBoundaries.GrainsArray[i, j] = new Grain()
                    {
                        Id = 0,
                        Color = Color.White
                    };
                    BoundariesWithBackground.GrainsArray[i, j] = new Grain()
                    {
                        Id = 0,
                        Color = Color.White
                    };
                    BoundariesAllSelected.GrainsArray[i, j] = new Grain()
                    {
                        Id = 0,
                        Color = Color.White
                    };
                    BoundariesSingleSelect.GrainsArray[i, j] = new Grain()
                    {
                        Id = 0,
                        Color = Color.White
                    };
                }
            }
        }

        public static bool IsOnGrainBoundaries(Range range, Point point)
        {
            var centerId = range.GrainsArray[point.X, point.Y].Id;
            var neighboursIds = new List<int>
            {
                range.GrainsArray[point.X - 1, point.Y].Id,
                range.GrainsArray[point.X + 1, point.Y].Id,
                range.GrainsArray[point.X, point.Y - 1].Id,
                range.GrainsArray[point.X, point.Y + 1].Id,
                range.GrainsArray[point.X - 1, point.Y - 1].Id,
                range.GrainsArray[point.X - 1, point.Y + 1].Id,
                range.GrainsArray[point.X + 1, point.Y - 1].Id,
                range.GrainsArray[point.X + 1, point.Y + 1].Id
            };

            foreach (var neighbourId in neighboursIds)
            {
                if (centerId != neighbourId && !InitStructure.IsIdSpecial(neighbourId))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsOnGrainBoundariesColor(Range range, Point point, Color color)
        {
            var neighbours = new List<Grain>
            {
                range.GrainsArray[point.X - 1, point.Y],
                range.GrainsArray[point.X + 1, point.Y],
                range.GrainsArray[point.X, point.Y - 1],
                range.GrainsArray[point.X, point.Y + 1],
                range.GrainsArray[point.X - 1, point.Y - 1],
                range.GrainsArray[point.X - 1, point.Y + 1],
                range.GrainsArray[point.X + 1, point.Y - 1],
                range.GrainsArray[point.X + 1, point.Y + 1]
            };

            foreach (var neighbour in neighbours)
            {
                if (neighbour.Color == color && neighbour.Id != 0 && neighbour.Id != -1)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
