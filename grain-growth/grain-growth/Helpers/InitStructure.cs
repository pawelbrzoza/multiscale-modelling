﻿using System;
using System.Drawing;

using grain_growth.Models;

namespace grain_growth.Helpers
{
    public class InitStructure
    {
        public static Range InitializeStructure(MainProperties properties)
        {
            Random random = new Random();

            // sizes
            Range tempRange = new Range(properties.RangeWidth, properties.RangeHeight);
  
            // border
            AddBlackBorder(tempRange);

            // init grains array by default values
            for (int i = 1; i < tempRange.Width - 1; i++)
            {
                for (int j = 1; j < tempRange.Height - 1; j++)
                {
                    if (tempRange.GrainsArray[i, j] == null)
                    {
                        tempRange.GrainsArray[i, j] = new Grain()
                        {
                            Id = 0,
                            Color = Color.White
                        };
                    }
                }
            }

            tempRange.StructureBitmap = new Bitmap(properties.RangeWidth, properties.RangeHeight);

            // inclusions at the beginning
            if (properties.Inclusions.AreEnable && (properties.Inclusions.CreationTime == InclusionsCreationTime.Begin))
            {
                InitInclusions inclusions = new InitInclusions(properties.Inclusions);
                tempRange = inclusions.AddInclusionsAtTheBegining(tempRange);
            }

            Point coordinates;
            // set random starting coordinates [x,y] and color for grains 
            for (int grainNumber = 1; grainNumber <= properties.NumberOfGrains; grainNumber++)
            {
                do
                {
                    coordinates = RandomCoordinates(tempRange.Width, tempRange.Height, random);
                }
                while (tempRange.GrainsArray[coordinates.X, coordinates.Y].Id != 0);

                tempRange.GrainsArray[coordinates.X, coordinates.Y].Color = Color.FromArgb(random.Next(10, 240), random.Next(10, 240), random.Next(2, 240));
                tempRange.GrainsArray[coordinates.X, coordinates.Y].Id = grainNumber;
                
            }
            return tempRange;
        }

        public static void AddBlackBorder(Range range)
        {
            for (int i = 0; i < range.Height; i++)
            {
                range.GrainsArray[0, i] = new Grain()
                {
                    Id = -1,
                    Color = Color.Black
                };
                range.GrainsArray[range.Width - 1, i] = new Grain()
                {
                    Id = -1,
                    Color = Color.Black
                };
            }

            for (int i = 0; i < range.Width; i++)
            {
                range.GrainsArray[i, 0] = new Grain()
                {
                    Id = -1,
                    Color = Color.Black
                };
                range.GrainsArray[i, range.Height - 1] = new Grain()
                {
                    Id = -1,
                    Color = Color.Black
                };
            }
        }

        public static Point RandomCoordinates(int width, int height, Random random)
        {
            return new Point(random.Next(1, width - 1), random.Next(1, height - 1));
        }

        public static bool IsIdSpecial(int id)
        {
            return Enum.IsDefined(typeof(SpecialId), id);
        }
    }
}