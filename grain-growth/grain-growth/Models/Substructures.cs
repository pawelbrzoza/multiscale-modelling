using grain_growth.Alghorithms;
using grain_growth.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace grain_growth.Models
{
    public class Substructures
    {
        public static List<Point> SubstrListPoints;

        public static List<Grain> SubstrGrainList { get; set; }

        public static Range UpdateSubstructures(Range range, MainProperties properties)
        {
            var tempRange = new Range(range.Width, range.Height);
            InitStructure.AddBlackBorder(tempRange);

            bool change;
            for (int i = 1; i < range.Width - 1; i++)
            {
                for (int j = 1; j < range.Height - 1; j++)
                {
                    change = false;
                    foreach (var point in SubstrListPoints)
                    {
                        if (range.GrainsArray[i, j].Color == range.StructureBitmap.GetPixel(point.X, point.Y))
                        {
                            if(properties.SubstructuresType == SubstructuresType.Substructure)
                            {
                                tempRange.GrainsArray[i, j] = new Grain
                                {
                                    Id = -3,
                                    Color = range.GrainsArray[i, j].Color
                                };
                            }
                            else
                            {
                                tempRange.GrainsArray[i, j] = new Grain
                                {
                                    Id = -4,
                                    Color = Color.Pink
                                };
                            }

                            change = true;
                        }
                    }
                    if (change == false)
                    {
                        tempRange.GrainsArray[i, j] = new Grain
                        {
                            Id = 0,
                            Color = Color.White
                        };
                    }
                }
            }

            Point coordinates;
            Random random = new Random();

            for (int i = 1; i <= properties.NumberOfGrains; i++)
            {
                while (true)
                {
                    coordinates = InitStructure.RandomCoordinates(range.Width, range.Height, random);

                    if (tempRange.GrainsArray[coordinates.X, coordinates.Y].Id == 0)
                    {
                        tempRange.GrainsArray[coordinates.X, coordinates.Y].Color = Color.FromArgb(random.Next(10, 240), random.Next(10, 240), random.Next(10, 240));
                        tempRange.GrainsArray[coordinates.X, coordinates.Y].Id = random.Next(1, 1000000);
                        break;
                    }
                }
            }

            tempRange.IsFull = false;
            return tempRange;
        }
    }
}