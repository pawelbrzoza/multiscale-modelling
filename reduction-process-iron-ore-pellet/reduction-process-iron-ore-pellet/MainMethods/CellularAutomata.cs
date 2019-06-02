using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using grain_growth.Helpers;
using grain_growth.Models;
using FastBitmapLib;

namespace grain_growth.MainMethods
{
    public class CellularAutomata
    {
        private Random Random = new Random();
        
        public Range Grow(Range prevRange, MainProperties properties)
        {
            var currRange = new Range(prevRange.Width, prevRange.Height, true);

            InitStructures.AddBlackBorder(currRange);
            InitStructures.DrawBlackCircle(currRange, new Point(150,150), 148);
            
            var isGrowthMoore2 = properties.NeighbourhoodType == NeighbourhoodType.Moore2 ? true : false;

            List<Grain> neighbourhood = new List<Grain>();

            for (int i = 1; i < prevRange.Width - 1; i++)
            {
                for (int j = 1; j < prevRange.Height - 1; j++)
                {
                    if (prevRange.GrainsArray[i, j].Id != (int)SpecialId.Id.Empty)
                    {
                        // just init if there is already some color (not white)
                        currRange.GrainsArray[i, j] = prevRange.GrainsArray[i, j];
                    }
                    else
                    {
                        if (!isGrowthMoore2)
                        {
                            // ordinary types of growth - list of Moore or Neuman neighbourhood
                            switch (properties.NeighbourhoodType)
                            {
                                case NeighbourhoodType.Moore:
                                    neighbourhood = TakeMooreNeighbourhood(i, j, prevRange.GrainsArray);
                                    break;
                                case NeighbourhoodType.Neumann:
                                    neighbourhood = TakeNeumannNeighbourhood(i, j, prevRange.GrainsArray);
                                    break;
                            }

                            var most = neighbourhood.Where(g => (!SpecialId.IsIdSpecial(g.Id)))
                                                    .GroupBy(g => g.Id);

                            if (most.Any())
                            {
                                // assign grain which are the most in the list of neighborhoods 
                                currRange.GrainsArray[i, j] = most.OrderByDescending(g => g.Count())
                                                                  .Select(g => g.First()).First();
                            }
                            else
                            {
                                currRange.GrainsArray[i, j] = new Grain()
                                {
                                    Id = (int)SpecialId.Id.Empty,
                                    Color = Color.White
                                };
                                currRange.IsFull = false;
                            }
                        }
                        else
                        {
                            // MOORE 2

                            var grainGrowth = false;

                            // rule 1 - ordinary moore
                            neighbourhood = TakeMooreNeighbourhood(i, j, prevRange.GrainsArray);

                            var most = neighbourhood.Where(g => (!SpecialId.IsIdSpecial(g.Id)))
                                                    .GroupBy(g => g.Id);

                            if (most.Any())
                            {
                                most = most.OrderByDescending(g => g.Count());

                                if (most.First().Count() >= 5 && most.First().Count() <= 8)
                                {
                                    currRange.GrainsArray[i, j] = most.Select(g => g.First()).First();
                                    grainGrowth = true;
                                }
                                else
                                {
                                    // rule 2 - nearest moore
                                    neighbourhood = TakeNearestMooreNeighbourhood(i, j, prevRange.GrainsArray);

                                    most = neighbourhood.Where(g => (!SpecialId.IsIdSpecial(g.Id)))
                                                        .GroupBy(g => g.Id);

                                    if (most.Any())
                                    {
                                        most = most.OrderByDescending(g => g.Count());
                                        if (most.First().Count() == 3)
                                        {
                                            currRange.GrainsArray[i, j] = most.Select(g => g.First()).First();
                                            grainGrowth = true;
                                        }
                                    }
                                    if (!grainGrowth)
                                    {
                                        // rule 3 - further moore
                                        neighbourhood = TakeFurtherMooreNeighbourhood(i, j, prevRange.GrainsArray);

                                        most = neighbourhood.Where(g => (!SpecialId.IsIdSpecial(g.Id)))
                                                            .GroupBy(g => g.Id);

                                        if (most.Any())
                                        {
                                            most = most.OrderByDescending(g => g.Count());
                                            if (most.First().Count() == 3)
                                            {
                                                currRange.GrainsArray[i, j] = most.Select(g => g.First()).First();
                                                grainGrowth = true;
                                            }
                                        }
                                    }
                                    if (!grainGrowth)
                                    {
                                        // rule 4 - ordinary moore with probability
                                        neighbourhood = TakeMooreNeighbourhood(i, j, prevRange.GrainsArray);

                                        most = neighbourhood.Where(g => (!SpecialId.IsIdSpecial(g.Id)))
                                                            .GroupBy(g => g.Id);

                                        var randomProbability = Random.Next(0, 100);
                                        if (most.Any() && (randomProbability <= properties.GrowthProbability))
                                        {
                                            currRange.GrainsArray[i, j] = most.OrderByDescending(g => g.Count())
                                                                                .Select(g => g.First()).First();
                                            grainGrowth = true;
                                        }
                                    }
                                }
                            }
                            if (!grainGrowth)
                            {
                                // if grain not exist
                                currRange.GrainsArray[i, j] = new Grain()
                                {
                                    Id = (int)SpecialId.Id.Empty,
                                    Color = Color.White
                                };
                                currRange.IsFull = false;
                            }
                        }
                    }
                }
            }
            UpdateBitmap(currRange);
            return currRange;
        }

        private List<Grain> TakeNeumannNeighbourhood(int i, int j, Grain[,] structureArray)
        {
            var neighbourhood = new List<Grain>
            {
                structureArray[i, j + 1],
                structureArray[i + 1, j],
                structureArray[i - 1, j],
                structureArray[i, j - 1],
                
            };
            return neighbourhood;
        }

        private List<Grain> TakeMooreNeighbourhood(int i, int j, Grain[,] structureArray)
        {
            var neighbourhood = new List<Grain>
            {
                structureArray[i - 1, j],
                structureArray[i + 1, j],
                structureArray[i, j - 1],
                structureArray[i, j + 1],
                structureArray[i - 1, j - 1],
                structureArray[i - 1, j + 1],
                structureArray[i + 1, j - 1],
                structureArray[i + 1, j + 1]
            };
            return neighbourhood;
        }

        private List<Grain> TakeNearestMooreNeighbourhood(int i, int j, Grain[,] structureArray)
        {
            var neighbourhood = new List<Grain>
            {
                structureArray[i - 1, j],
                structureArray[i + 1, j],
                structureArray[i, j - 1],
                structureArray[i, j + 1]
            };
            return neighbourhood;
        }

        private List<Grain> TakeFurtherMooreNeighbourhood(int i, int j, Grain[,] structureArray)
        {
            var neighbourhood = new List<Grain>
            {
                structureArray[i - 1, j - 1],
                structureArray[i - 1, j + 1],
                structureArray[i + 1, j - 1],
                structureArray[i + 1, j + 1]
            };
            return neighbourhood;
        }

        public static void UpdateBitmap(Range range)
        {
            //Stopwatch sw = Stopwatch.StartNew();
            //for (int i = 0; i < range.Width; i++)
            //    for (int j = 0; j < range.Height; j++)
            //        range.StructureBitmap.SetPixel(i, j, range.GrainsArray[i, j].Color);
            //Console.WriteLine("Serial: {0:f2} s", sw.Elapsed.TotalSeconds);

            //Stopwatch sw = Stopwatch.StartNew();
            using (var fastBitmap = range.StructureBitmap.FastLock())
            {
                for (int i = 0; i < range.Width; i++)
                    for (int j = 0; j < range.Height; j++)
                        fastBitmap.SetPixel(i, j, range.GrainsArray[i, j].Color);
            }
            //Console.WriteLine("Serial: {0:f2} s", sw.Elapsed.TotalSeconds);
        }

        public static void UpdateGrainsArray(Range range)
        {
            range.GrainsArray = new Grain[range.Width, range.Height];

            Dictionary<Color, int> grainIds = new Dictionary<Color, int>
            {
                { Color.FromArgb(0, 0, 0), 0 },
            };

            if (range.StructureBitmap != null)
            {
                for (int i = 0; i < range.Width; i++)
                {
                    for (int j = 0; j < range.Height; j++)
                    {
                        var color = range.StructureBitmap.GetPixel(i, j);
                        range.GrainsArray[i, j] = new Grain()
                        {
                            Color = color,
                            Id = ChooseGrainId(grainIds, color),
                        };
                    }
                }
            }
        }

        public static int ChooseGrainId(Dictionary<Color, int> grainIds, Color color)
        {
            int nextId = grainIds.Values.Max()+1;

            if (grainIds.ContainsKey(color))
            {
                if (!grainIds.TryGetValue(color, out int id))
                {
                    grainIds[color] = nextId;
                    return nextId;
                }
                return id;
            }
            else
            {
                grainIds.Add(color, nextId);
                return nextId;
            }
        }
    }
}
