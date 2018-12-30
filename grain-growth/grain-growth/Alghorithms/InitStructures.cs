using System;
using System.Drawing;
using grain_growth.Models;

namespace grain_growth.Helpers
{
    public class InitStructures
    {
        private static Random Random = new Random();

        public static Grain[] AllGrainsTypes;

        public static Range InitCellularAutomata(MainProperties properties)
        {
            AllGrainsTypes = new Grain[properties.AmountOfGrains];

            Range tempRange = new Range(properties.RangeWidth, properties.RangeHeight);
            tempRange.IsFull = false;
  
            // border
            AddBlackBorder(tempRange);

            // init grains array by white color (default)
            for (int i = 1; i < tempRange.Width - 1; i++)
                for (int j = 1; j < tempRange.Height - 1; j++)
                    if (tempRange.GrainsArray[i, j] == null)
                        tempRange.GrainsArray[i, j] = new Grain()
                        {
                            Id = 0,
                            Color = Color.White,
                        };

            tempRange.StructureBitmap = new Bitmap(properties.RangeWidth, properties.RangeHeight);

            // set random starting coordinates [x,y] and color for grains 
            Point coordinates;
            for (int grainNumber = 1; grainNumber <= properties.AmountOfGrains; grainNumber++)
            {
                do
                {
                    coordinates = RandomCoordinates.Get(tempRange.Width, tempRange.Height, Random);
                }
                while (tempRange.GrainsArray[coordinates.X, coordinates.Y].Id != 0);

                AllGrainsTypes[grainNumber - 1] = new Grain()
                {
                    Color = Color.FromArgb(Random.Next(10, 240), Random.Next(10, 240), Random.Next(2, 240)),
                    Id = grainNumber,
                };

                tempRange.GrainsArray[coordinates.X, coordinates.Y].Color = AllGrainsTypes[grainNumber - 1].Color;
                tempRange.GrainsArray[coordinates.X, coordinates.Y].Id = AllGrainsTypes[grainNumber - 1].Id;
            }
            return tempRange;
        }

        public static Range InitMonteCarlo(MainProperties properties)
        {
            AllGrainsTypes = new Grain[properties.AmountOfGrains];
            Range tempRange = new Range(properties.RangeWidth, properties.RangeHeight);
            tempRange.IsFull = false;

            // border
            AddBlackBorder(tempRange);

            // set random starting coordinates [x,y] and color for grains 
            for (int grainNumber = 1; grainNumber <= properties.AmountOfGrains; grainNumber++)
            {
                AllGrainsTypes[grainNumber - 1] = new Grain()
                {
                    Color = Color.FromArgb(Random.Next(10, 240), Random.Next(10, 240), Random.Next(2, 240)),
                    Id = grainNumber,
                }; 
            }

            for (int i = 1; i < tempRange.Width - 1; i++)
            {
                for (int j = 1; j < tempRange.Height - 1; j++)
                {
                    int r = Random.Next(AllGrainsTypes.Length);
                    tempRange.GrainsArray[i, j] = new Grain()
                    {
                        Id = AllGrainsTypes[r].Id,
                        Color = AllGrainsTypes[r].Color,
                    };

                }
            }

            tempRange.StructureBitmap = new Bitmap(properties.RangeWidth, properties.RangeHeight);

            return tempRange;
        }

        public static void AddBlackBorder(Range tempRange)
        {
            for (int i = 0; i < tempRange.Height; i++)
            {
                tempRange.GrainsArray[0, i] = new Grain()
                {
                    Id = -1,
                    Color = Color.Black,
                    Energy_H = 0
                };
                tempRange.GrainsArray[tempRange.Width - 1, i] = new Grain()
                {
                    Id = -1,
                    Color = Color.Black,
                    Energy_H = 0
                };
            }

            for (int i = 0; i < tempRange.Width; i++)
            {
                tempRange.GrainsArray[i, 0] = new Grain()
                {
                    Id = -1,
                    Color = Color.Black,
                    Energy_H = 0
                };
                tempRange.GrainsArray[i, tempRange.Height - 1] = new Grain()
                {
                    Id = -1,
                    Color = Color.Black,
                    Energy_H = 0
                };
            }
        }

    }
}