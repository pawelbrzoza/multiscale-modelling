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
                        if (range.GrainsArray[i, j].Color == range.StructureBitmap.GetPixel(point.X, point.Y) )
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
                    if(range.GrainsArray[i, j].Id == -2){
                        tempRange.GrainsArray[i, j] = new Grain
                        {
                            Id = -2,
                            Color = Color.Black
                        };
                        change = true;
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

        public static Range UpdateSubstructuresMC(Range range, MainProperties properties)
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
                            if (properties.SubstructuresType == SubstructuresType.Substructure)
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
                    if (range.GrainsArray[i, j].Id == -2)
                    {
                        tempRange.GrainsArray[i, j] = new Grain
                        {
                            Id = -2,
                            Color = Color.Black
                        };
                        change = true;
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

            Random random = new Random();
            InitStructure.grainArr = new Grain[properties.NumberOfGrains];

            //Point coordinates;
            // set random starting coordinates [x,y] and color for grains 
            for (int grainNumber = 1; grainNumber <= properties.NumberOfGrains; grainNumber++)
            {
                InitStructure.grainArr[grainNumber - 1] = new Grain()
                {
                    Color = Color.FromArgb(random.Next(10, 240), random.Next(10, 240), random.Next(2, 240)),
                    Id = grainNumber
                };
            }

            for (int i = 1; i < tempRange.Width - 1; i++)
            {
                for (int j = 1; j < tempRange.Height - 1; j++)
                {
                    if(tempRange.GrainsArray[i,j].Id != -1 && tempRange.GrainsArray[i, j].Id != -2 && tempRange.GrainsArray[i, j].Id != -3
                        && tempRange.GrainsArray[i, j].Id != -4 && tempRange.GrainsArray[i, j].Id != -5)
                    {
                        int r = random.Next(InitStructure.grainArr.Length);
                        tempRange.GrainsArray[i, j] = new Grain()
                        {
                            Id = InitStructure.grainArr[r].Id,
                            Color = InitStructure.grainArr[r].Color
                        };
                    }
                }
            }

            tempRange.StructureBitmap = new Bitmap(properties.RangeWidth, properties.RangeHeight);

            tempRange.IsFull = false;
            return tempRange;
        }
    }

    public enum SubstructuresType
    {
        Substructure = -3,
        DualPhase = -4
    }
}