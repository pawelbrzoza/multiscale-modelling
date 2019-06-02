using System;
using System.Collections.Generic;
using System.Drawing;

using grain_growth.Models;

namespace grain_growth.Helpers
{
    public class InitInclusions : Inclusions
    {
        private readonly Random Random = new Random();

        public Range AddInclusionsAtTheBegining(Range tempRange)
        {
            Point coordinates;
            for (int i = 0; i < AmountOfInclusions; i++)
            {
                do
                {
                    coordinates = RandomCoordinates.Get(tempRange.Width, tempRange.Height, Random);
                }
                while (tempRange.GrainsArray[coordinates.X, coordinates.Y].Id != 0);

                switch (InclusionsType)
                {
                    case InclusionsType.Square:
                        AddSquareInclusion(tempRange, coordinates);
                        break;
                    case InclusionsType.Circular:
                        AddCirularInclusion(tempRange, coordinates);
                        break;
                }
            }
            return tempRange;
        }

        public Range AddInclusionsAfter(Range tempRange)
        {
            Point coordinates;
            for (int inclusionNumber = 0; inclusionNumber < AmountOfInclusions; inclusionNumber++)
            {
                do
                {
                    coordinates = RandomCoordinates.Get(tempRange.Width, tempRange.Height, Random);
                }
                while (!InitBoundaries.IsOnGrainBoundaries(tempRange, coordinates));

                switch (InclusionsType)
                {
                    case InclusionsType.Square:
                        AddSquareInclusion(tempRange, coordinates);
                        break;
                    case InclusionsType.Circular:
                        AddCirularInclusion(tempRange, coordinates);
                        break;
                }
            }
            return tempRange;
        }

        private void AddSquareInclusion(Range tempRange, Point center)
        {
            int halfA = (Size / 2);
            for (int x = center.X - halfA; (x <= center.X + halfA && x < tempRange.Width && x > 0); x++)
            {
                for (int y = center.Y - halfA; (y <= center.Y + halfA && y < tempRange.Height && y > 0); y++)
                {
                    if (!SpecialId.IsIdSpecial(tempRange.GrainsArray[x, y].Id) || tempRange.GrainsArray[x, y].Id == 0)
                    {
                        tempRange.GrainsArray[x, y].Color = Color.Black;
                        tempRange.GrainsArray[x, y].Id = Convert.ToInt32(SpecialId.Id.Inclusion);
                    }
                }
            }
        }

        private void AddCirularInclusion(Range tempRange, Point center)
        {
            var pointsInside = GetPointsInsideCircle(Size / 2, center);
            foreach (var point in pointsInside)
            {
                if (point.X < tempRange.Width && point.X > 0 && point.Y < tempRange.Height && point.Y > 0)
                {
                    if (!SpecialId.IsIdSpecial(tempRange.GrainsArray[point.X, point.Y].Id) || tempRange.GrainsArray[point.X, point.Y].Id == 0)
                    {
                        tempRange.GrainsArray[point.X, point.Y].Color = Color.Black;
                        tempRange.GrainsArray[point.X, point.Y].Id = Convert.ToInt32(SpecialId.Id.Inclusion);
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
