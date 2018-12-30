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

        public MonteCarlo()
        {
            Random = new Random();
        }

        public Range Grow(Range tempRange, InitNucleons nucleons)
        {

            List<Grain> neighbourhood = new List<Grain>();
            List<Point> pointsCoordinates = new List<Point>();
            Grain centerGrain;
            int EnergyAfter, EnergyBefore, EnergyDelta, randomNumberOfList;

            if(nucleons.IsEnable == true)
            {
                for (int i = 1; i < tempRange.Width - 1; i++)
                    for (int j = 1; j < tempRange.Height - 1; j++)
                    {
                        neighbourhood = TakeMooreNeighbourhood(i, j, tempRange.GrainsArray);
                        if (neighbourhood.Where(g => g.Id == -5).Select(g => g.Id).Count() > 0)
                            pointsCoordinates.Add(new Point(i, j));
                    }

                pointsCoordinates.OrderBy(a => Random.Next());

                while (pointsCoordinates.Count > 0)
                {
                    EnergyAfter = 0; EnergyBefore = 0; EnergyDelta = 0;

                    //step 1
                    centerGrain = tempRange.GrainsArray[pointsCoordinates.First().X, pointsCoordinates.First().Y];

                    //step 2
                    neighbourhood = TakeMooreNeighbourhood(pointsCoordinates.First().X, pointsCoordinates.First().Y, tempRange.GrainsArray);

                    EnergyBefore = neighbourhood.Where(g => (!SpecialId.IsIdSpecial(g.Id) && g.Id != centerGrain.Id))
                                        .Select(g => g.Id).Count() + centerGrain.Energy_H;

                    //step 3
                    randomNumberOfList = Random.Next(InitStructures.AllGrainsTypes.Length);
                    centerGrain = InitStructures.AllGrainsTypes[randomNumberOfList];

                    EnergyAfter = neighbourhood.Where(g => (!SpecialId.IsIdSpecial(g.Id) && g.Id != centerGrain.Id))
                                            .Select(g => g.Id).Count();
                    //step 4
                    EnergyDelta = EnergyAfter - EnergyBefore;

                    //step 5
                    if (EnergyDelta <= 0 && !SpecialId.IsIdSpecial(tempRange.GrainsArray[pointsCoordinates.First().X, pointsCoordinates.First().Y].Id))
                        tempRange.GrainsArray[pointsCoordinates.First().X, pointsCoordinates.First().Y] = neighbourhood.Where(g=>g.Id == -5).First();
                    
                    pointsCoordinates.RemoveAt(0);
                }
            }
            else
            {
                for (int i = 1; i < tempRange.Width - 1; i++)
                    for (int j = 1; j < tempRange.Height - 1; j++)
                        pointsCoordinates.Add(new Point(i, j));

                pointsCoordinates.OrderBy(a => Random.Next());

                while (pointsCoordinates.Count > 0)
                {
                    EnergyAfter = 0; EnergyBefore = 0; EnergyDelta = 0;

                    //step 1
                    centerGrain = tempRange.GrainsArray[pointsCoordinates.First().X, pointsCoordinates.First().Y];
                   
                    //step 2
                    neighbourhood = TakeMooreNeighbourhood(pointsCoordinates.First().X, pointsCoordinates.First().Y, tempRange.GrainsArray);

                    EnergyBefore = neighbourhood.Where(g => (!SpecialId.IsIdSpecial(g.Id) && g.Id != centerGrain.Id))
                                            .Select(g => g.Id).Count();

                    //step 3
                    randomNumberOfList = Random.Next(InitStructures.AllGrainsTypes.Length);
                    centerGrain = InitStructures.AllGrainsTypes[randomNumberOfList];

                    EnergyAfter = neighbourhood.Where(g => (!SpecialId.IsIdSpecial(g.Id) && g.Id != centerGrain.Id))
                                            .Select(g => g.Id).Count();
                    //step 4
                    EnergyDelta = EnergyAfter - EnergyBefore;

                    //step 5
                    if (EnergyDelta <= 0 && !SpecialId.IsIdSpecial(tempRange.GrainsArray[pointsCoordinates.First().X, pointsCoordinates.First().Y].Id))
                        tempRange.GrainsArray[pointsCoordinates.First().X, pointsCoordinates.First().Y] = centerGrain;

                    pointsCoordinates.RemoveAt(0);
                }
            }
            
            UpdateBitmap(tempRange);
            return tempRange;
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
