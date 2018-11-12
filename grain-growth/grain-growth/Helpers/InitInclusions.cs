using System;
using System.Collections.Generic;
using System.Drawing;

using grain_growth.Models;

namespace grain_growth.Helpers
{
    public class InitInclusions
    {
        private InclusionsProperties inclusionsProperties;

        private Random random;

        public InitInclusions(InclusionsProperties inclusionsProperties)
        {
            this.inclusionsProperties = inclusionsProperties;
            this.random = new Random();
        }

        public Range AddInclusionsAtTheBegining(Range range)
        {
            Point coordinates;
            for (int i = 0; i < inclusionsProperties.Number; i++)
            {
                do
                {
                    coordinates = InitStructure.RandomCoordinates(range.Width, range.Height, random);
                }
                while (range.GrainsArray[coordinates.X, coordinates.Y].Id != 0);

                switch (inclusionsProperties.InclusionsType)
                {
                    case InclusionsType.Square:
                        AddSquareInclusion(range, coordinates);
                        break;
                    case InclusionsType.Circular:
                        AddCirularInclusion(range, coordinates);
                        break;
                }
            }
            return range;
        }

        public Range AddInclusionsAfterGrainGrowth(Range range)
        {
            Point coordinates;
            for (int inclusionNumber = 0; inclusionNumber < inclusionsProperties.Number; inclusionNumber++)
            {
                do
                {
                    coordinates = InitStructure.RandomCoordinates(range.Width, range.Height, random);
                }
                while (!IsCoordinateOnGrainBoundaries(range, coordinates));

                switch (inclusionsProperties.InclusionsType)
                {
                    case InclusionsType.Square:
                        AddSquareInclusion(range, coordinates);
                        break;
                    case InclusionsType.Circular:
                        AddCirularInclusion(range, coordinates);
                        break;
                }
            }
            return range;
        }

        private void AddSquareInclusion(Range range, Point coordinates)
        {
            int a = (int)(inclusionsProperties.Size / Math.Sqrt(2));
            int halfA = (a / 2);
            for (int x = coordinates.X - halfA; (x <= coordinates.X + halfA && x < range.Width && x > 0); x++)
            {
                for (int y = coordinates.Y - halfA; (y <= coordinates.Y + halfA && y < range.Height && y > 0); y++)
                {
                    if (!InitStructure.IsIdSpecial(range.GrainsArray[x, y].Id) || range.GrainsArray[x, y].Id == 0)
                    {
                        range.GrainsArray[x, y].Color = Color.Black;
                        range.GrainsArray[x, y].Id = Convert.ToInt32(SpecialId.Inclusion);
                    }
                }
            }
        }

        private void AddCirularInclusion(Range range, Point coordinates)
        {
            var pointsInside = GetPointsInsideCircle(inclusionsProperties.Size, coordinates);
            foreach (var point in pointsInside)
            {
                if (point.X < range.Width && point.X > 0 && point.Y < range.Height && point.Y > 0)
                {
                    if (!InitStructure.IsIdSpecial(range.GrainsArray[point.X, point.Y].Id) || range.GrainsArray[point.X, point.Y].Id == 0)
                    {
                        range.GrainsArray[point.X, point.Y].Color = Color.Black;
                        range.GrainsArray[point.X, point.Y].Id = Convert.ToInt32(SpecialId.Inclusion);
                    }
                }
            }
        }

        private List<Point> GetPointsInsideCircle(int radius, Point center)
        {
            List<Point> pointsInside = new List<Point>();

            for (int x = center.X - radius; x < center.X + radius; x++)
            {
                for (int y = center.Y - radius; y < center.Y + radius; y++)
                {
                    if ((x - center.X) * (x - center.X) + (y - center.Y) * (y - center.Y) <= radius * radius)
                    {
                        pointsInside.Add(new Point(x, y));
                    }
                }
            }
            return pointsInside;
        }

        private bool IsCoordinateOnGrainBoundaries(Range range, Point coordinates)
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
    }
}
