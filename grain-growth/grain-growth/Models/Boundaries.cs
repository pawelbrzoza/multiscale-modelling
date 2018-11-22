using grain_growth.Helpers;
using System.Collections.Generic;
using System.Drawing;

namespace grain_growth.Models
{
    public class Boundaries
    {
        public Range ClearBoundaries { get; set; }

        public Range BoundariesWithBackground { get; set; }

        public Range BoundariesSelected { set; get; }

        public Range BoundariesSingleSelect { set; get; }

        public Boundaries(MainProperties properties) {

            ClearBoundaries = new Range(properties.RangeWidth, properties.RangeHeight);
            BoundariesWithBackground = new Range(properties.RangeWidth, properties.RangeHeight);
            BoundariesSelected = new Range(properties.RangeWidth, properties.RangeHeight);
            BoundariesSingleSelect = new Range(properties.RangeWidth, properties.RangeHeight);

            // border
            InitStructure.AddBlackBorder(ClearBoundaries);
            InitStructure.AddBlackBorder(BoundariesWithBackground);
            InitStructure.AddBlackBorder(BoundariesSelected);
            InitStructure.AddBlackBorder(BoundariesSingleSelect);

            // init grains array by default values
            for (int i = 1; i < properties.RangeWidth - 1; i++)
            {
                for (int j = 1; j < properties.RangeHeight - 1; j++)
                {
                    if (ClearBoundaries.GrainsArray[i, j] == null)
                    {
                        ClearBoundaries.GrainsArray[i, j] = new Grain()
                        {
                            Id = 0,
                            Color = Color.White
                        };
                    }
                    if (BoundariesWithBackground.GrainsArray[i, j] == null)
                    {
                        BoundariesWithBackground.GrainsArray[i, j] = new Grain()
                        {
                            Id = 0,
                            Color = Color.White
                        };
                    }
                    if (BoundariesSelected.GrainsArray[i, j] == null)
                    {
                        BoundariesSelected.GrainsArray[i, j] = new Grain()
                        {
                            Id = 0,
                            Color = Color.White
                        };
                    }
                    if (BoundariesSingleSelect.GrainsArray[i, j] == null)
                    {
                        BoundariesSingleSelect.GrainsArray[i, j] = new Grain()
                        {
                            Id = 0,
                            Color = Color.White
                        };
                    }
                }
            }
        }

        public bool IsCoordinateOnGrainBoundaries(Range range, Point coordinates)
        {
            var centerId = range.GrainsArray[coordinates.X, coordinates.Y].Id;
            var neighboursIds = new List<int>
            {
                range.GrainsArray[coordinates.X - 1, coordinates.Y].Id,
                range.GrainsArray[coordinates.X + 1, coordinates.Y].Id,
                range.GrainsArray[coordinates.X, coordinates.Y - 1].Id,
                range.GrainsArray[coordinates.X, coordinates.Y + 1].Id,
                range.GrainsArray[coordinates.X - 1, coordinates.Y - 1].Id,
                range.GrainsArray[coordinates.X - 1, coordinates.Y + 1].Id,
                range.GrainsArray[coordinates.X + 1, coordinates.Y - 1].Id,
                range.GrainsArray[coordinates.X + 1, coordinates.Y + 1].Id
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

        public bool IsOnGrainBoundariesColor(Range currRange, Point point, Color color)
        {
            var neighbours = new List<Grain>
            {
                currRange.GrainsArray[point.X - 1, point.Y],
                currRange.GrainsArray[point.X + 1, point.Y],
                currRange.GrainsArray[point.X, point.Y - 1],
                currRange.GrainsArray[point.X, point.Y + 1],
                currRange.GrainsArray[point.X - 1, point.Y - 1],
                currRange.GrainsArray[point.X - 1, point.Y + 1],
                currRange.GrainsArray[point.X + 1, point.Y - 1],
                currRange.GrainsArray[point.X + 1, point.Y + 1]
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
