using grain_growth.Helpers;
using System;
using System.Drawing;

namespace grain_growth.Models
{
    public class InitSubstructures : Substructures
    {
        public Range UpdateSubstructuresCA(Range tempRange, MainProperties properties)
        {
            bool change;
            for (int i = 1; i < tempRange.Width - 1; i++)
            {
                for (int j = 1; j < tempRange.Height - 1; j++)
                {
                    change = false;
                    foreach (var point in SubStrucrtuePointsList)
                    {
                        if (tempRange.GrainsArray[i, j].Color == tempRange.StructureBitmap.GetPixel(point.X, point.Y) )
                        {
                            if(properties.SubstructuresType == SubstructuresType.Substructure)
                            {
                                tempRange.GrainsArray[i, j] = new Grain
                                {
                                    Id = (int)SpecialId.Id.Substructure,
                                    Color = tempRange.GrainsArray[i, j].Color
                                };
                            }
                            else
                            {
                                tempRange.GrainsArray[i, j] = new Grain
                                {
                                    Id = (int)SpecialId.Id.DualPhase,
                                    Color = Color.Pink
                                };
                            }

                            change = true;
                        }
                    }
                    if(tempRange.GrainsArray[i, j].Id == -2){
                        tempRange.GrainsArray[i, j] = new Grain
                        {
                            Id = (int)SpecialId.Id.Inclusion,
                            Color = Color.Black
                        };
                        change = true;
                    }
                    if (change == false)
                    {
                        tempRange.GrainsArray[i, j] = new Grain
                        {
                            Id = (int)SpecialId.Id.Empty,
                            Color = Color.White
                        };
                    }
                }
            }

            Point coordinates;
            for (int i = 1; i <= properties.AmountOfGrains; i++)
            {
                while (true)
                {
                    coordinates = RandomCoordinates.Get(tempRange.Width, tempRange.Height, Random);

                    if (tempRange.GrainsArray[coordinates.X, coordinates.Y].Id == 0)
                    {
                        tempRange.GrainsArray[coordinates.X, coordinates.Y].Color = Color.FromArgb(Random.Next(10, 240), Random.Next(10, 240), Random.Next(10, 240));
                        tempRange.GrainsArray[coordinates.X, coordinates.Y].Id = Random.Next(1, 1000000);
                        break;
                    }
                }
            }

            tempRange.IsFull = false;
            return tempRange;
        }

        public Range UpdateSubstructuresMC(Range tempRange, MainProperties properties)
        {
            bool change;
            for (int i = 1; i < tempRange.Width - 1; i++)
            {
                for (int j = 1; j < tempRange.Height - 1; j++)
                {
                    change = false;
                    foreach (var point in SubStrucrtuePointsList)
                    {
                        if (tempRange.GrainsArray[i, j].Color == tempRange.StructureBitmap.GetPixel(point.X, point.Y))
                        {
                            if (properties.SubstructuresType == SubstructuresType.Substructure)
                            {
                                tempRange.GrainsArray[i, j] = new Grain
                                {
                                    Id = (int)SpecialId.Id.Substructure,
                                    Color = tempRange.GrainsArray[i, j].Color
                                };
                            }
                            else
                            {
                                tempRange.GrainsArray[i, j] = new Grain
                                {
                                    Id = (int)SpecialId.Id.DualPhase,
                                    Color = Color.Pink
                                };
                            }

                            change = true;
                        }
                    }
                    if (tempRange.GrainsArray[i, j].Id == -2)
                    {
                        tempRange.GrainsArray[i, j] = new Grain
                        {
                            Id = (int)SpecialId.Id.Inclusion,
                            Color = Color.Black
                        };
                        change = true;
                    }
                    if (change == false)
                    {
                        tempRange.GrainsArray[i, j] = new Grain
                        {
                            Id = (int)SpecialId.Id.Empty,
                            Color = Color.White
                        };
                    }
                }
            }

            InitStructures.AllGrainsTypes = new Grain[properties.AmountOfGrains];

            // set random starting coordinates [x,y] and color for grains 
            for (int grainNumber = 1; grainNumber <= properties.AmountOfGrains; grainNumber++)
            {
                InitStructures.AllGrainsTypes[grainNumber - 1] = new Grain()
                {
                    Color = Color.FromArgb(Random.Next(10, 240), Random.Next(10, 240), Random.Next(10, 240)),
                    Id = grainNumber
                };
            }

            for (int i = 1; i < tempRange.Width - 1; i++)
            {
                for (int j = 1; j < tempRange.Height - 1; j++)
                {
                    if(tempRange.GrainsArray[i,j].Id >= 0)
                    {
                        int r = Random.Next(InitStructures.AllGrainsTypes.Length);
                        tempRange.GrainsArray[i, j] = new Grain()
                        {
                            Id = InitStructures.AllGrainsTypes[r].Id,
                            Color = InitStructures.AllGrainsTypes[r].Color,
                            Energy_H = InitStructures.AllGrainsTypes[r].Energy_H
                        };
                    }
                }
            }

            tempRange.IsFull = false;
            return tempRange;
        }
    }
}