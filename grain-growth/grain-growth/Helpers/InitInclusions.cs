using System;
using System.Collections.Generic;
using System.Drawing;

using grain_growth.Models;

namespace grain_growth.Helpers
{
    public class InitInclusions : Inclusions
    {
        //private Inclusions inclusionsProperties;

        private readonly Random random;

        public InitInclusions()
        {
            random = new Random();
        }

        public Range AddInclusionsAtTheBegining(Range range)
        {
            Point coordinates;
            for (int i = 0; i < Number; i++)
            {
                do
                {
                    coordinates = InitStructure.RandomCoordinates(range.Width, range.Height, random);
                }
                while (range.GrainsArray[coordinates.X, coordinates.Y].Id != 0);

                switch (InclusionsType)
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
            for (int inclusionNumber = 0; inclusionNumber < Number; inclusionNumber++)
            {
                do
                {
                    coordinates = InitStructure.RandomCoordinates(range.Width, range.Height, random);
                }
                while (!Boundaries.IsOnGrainBoundaries(range, coordinates));

                switch (InclusionsType)
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

        private void AddSquareInclusion(Range range, Point center)
        {
            int a = (int)Size;
            int halfA = (a / 2);
            for (int x = center.X - halfA; (x <= center.X + halfA && x < range.Width && x > 0); x++)
            {
                for (int y = center.Y - halfA; (y <= center.Y + halfA && y < range.Height && y > 0); y++)
                {
                    if (!InitStructure.IsIdSpecial(range.GrainsArray[x, y].Id) || range.GrainsArray[x, y].Id == 0)
                    {
                        range.GrainsArray[x, y].Color = Color.Black;
                        range.GrainsArray[x, y].Id = Convert.ToInt32(SpecialId.Inclusion);
                    }
                }
            }
        }

        private void AddCirularInclusion(Range range, Point center)
        {
            var pointsInside = GetPointsInsideCircle(Size / 2, center);
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

    }
}
