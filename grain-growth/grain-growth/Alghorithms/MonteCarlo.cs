using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

using grain_growth.Helpers;
using grain_growth.Models;

namespace grain_growth.Alghorithms
{
    class MonteCarlo
    {
        private Random Random { get; set; }

        static public int MCS_local;

        public MonteCarlo()
        {
            this.Random = new Random();
        }

        public Range Grow(Range prevRange, MainProperties properties)
        {
            var currRange = new Range(prevRange.Width, prevRange.Height, true);
            InitStructure.AddBlackBorder(currRange);
            List<Grain> neighbourhood = new List<Grain>();
            int afterE, beforeE, deltaE;

            for (int i = 1; i < prevRange.Width - 1; i++)
                for (int j = 1; j < prevRange.Height - 1; j++)
                    currRange.GrainsArray[i, j] = prevRange.GrainsArray[i, j];


            for (int i = 1; i < prevRange.Width - 1; i++)
                for (int j = 1; j < prevRange.Height - 1; j++)
                    for (int k = 0; k < properties.MCS; k++)
                    { 
                        afterE = 0; beforeE = 0; deltaE = 0;
            
                        Point randPoint = InitStructure.RandomCoordinates(properties.RangeWidth, properties.RangeHeight, Random);

                        neighbourhood = TakeMooreNeighbourhood(randPoint.X, randPoint.Y, prevRange.GrainsArray);

                        Grain centerGrain = currRange.GrainsArray[randPoint.X, randPoint.Y];

                        var most = neighbourhood.Where(g => (!InitStructure.IsIdSpecial(g.Id)))
                                                    .GroupBy(g => g.Id);

                        for (int l = 0; l < neighbourhood.Count; l++)
                            if (centerGrain.Id != neighbourhood[l].Id)
                                beforeE++;

                        int r = Random.Next(InitStructure.grainArr.Length);
                        centerGrain = InitStructure.grainArr[r];

                        for (int l = 0; l < neighbourhood.Count; l++)
                            if (centerGrain.Id != neighbourhood[l].Id)
                                afterE++;

                        deltaE = afterE - beforeE;

                        //currRange.GrainsArray

                        if (most.Any() && deltaE <= 0)
                        {
                            // assign grain which are the most in the list of neighborhoods 
                            currRange.GrainsArray[randPoint.X, randPoint.Y] = most.OrderByDescending(g => g.Count())
                                                                                  .Select(g => g.First()).First();
                        }
                
                    }
            
            UpdateBitmap(currRange);


            return currRange;
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

        public static void UpdateBitmap(Range range)
        {
            // setting bitmap colors form grains array
            for (int i = 0; i < range.Width; i++)
                for (int j = 0; j < range.Height; j++)
                    range.StructureBitmap.SetPixel(i, j, range.GrainsArray[i, j].Color);
        }

        public static void UpdateGrainsArray(Range range)
        {
            range.GrainsArray = new Grain[range.Width, range.Height];

            Dictionary<Color, int> grainIds = new Dictionary<Color, int>
            {
                { Color.FromArgb(0, 0, 0), -1 },
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
                            Id = ChooseGrainId(grainIds, color)
                        };
                    }
                }
            }
        }

        public static int ChooseGrainId(Dictionary<Color, int> grainIds, Color color)
        {
            int nextId = grainIds.Values.Max() + 1;

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
