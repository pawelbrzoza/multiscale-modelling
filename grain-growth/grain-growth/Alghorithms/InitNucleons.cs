﻿using System;
using System.Drawing;
using grain_growth.Helpers;

namespace grain_growth.Models
{
    public class InitNucleons : Nucleons
    {
        private Random Random = new Random();

        public Range InitializeNucleons(Range tempRange, InitNucleons nucleons)
        {
            // randomize nucleon states
            for (int i = 0; i < nucleons.NucleonsStates.Length; i++)
                NucleonsStates[i] = Color.FromArgb(Random.Next(50, 200), 0, 0);

            // anywhere
            if (PositionDistribiution == PositionDistribiution.Anywhere)
            {
                Point coordinates;
                for (int i = 0; i < nucleons.AmountOfNucleons; i++)
                {
                    do
                    {
                        coordinates = RandomCoordinates.Get(tempRange.Width, tempRange.Height, Random);
                    }
                    while (tempRange.GrainsArray[coordinates.X, coordinates.Y].Id <= 0);

                    tempRange.GrainsArray[coordinates.X, coordinates.Y].Id = (int)SpecialId.Id.Nucleon;
                    tempRange.GrainsArray[coordinates.X, coordinates.Y].Color = NucleonsStates[Random.Next(nucleons.NucleonsStates.Length)];
                }
            }
            // on grain boundaries
            else
            {
                Point coordinates;
                for (int i = 0; i < nucleons.AmountOfNucleons; i++)
                {
                    do
                    {
                        coordinates = RandomCoordinates.Get(tempRange.Width, tempRange.Height, Random);
                    }
                    while (!InitBoundaries.IsOnGrainBoundaries(tempRange, coordinates) && !SpecialId.IsIdSpecial(tempRange.GrainsArray[coordinates.X, coordinates.Y].Id));

                    tempRange.GrainsArray[coordinates.X, coordinates.Y].Id = (int)SpecialId.Id.Nucleon;
                    tempRange.GrainsArray[coordinates.X, coordinates.Y].Color = NucleonsStates[Random.Next(nucleons.NucleonsStates.Length)];
                }
            }

            tempRange.IsFull = false;
            return tempRange;
        }

        public Range EnergyDistributor(Range tempRange, InitNucleons nucleons)
        {
            if (nucleons.EnergyDistribution == EnergyDistribution.Homogenous)
            {
                for (int i = 1; i < tempRange.Width - 1; i++)
                {
                    for (int j = 1; j < tempRange.Height - 1; j++)
                    {
                        tempRange.GrainsArray[i, j].Energy_H = nucleons.EnergyInside;
                    }
                }
            }
            else
            {
                for (int i = 1; i < tempRange.Width - 1; i++)
                {
                    for (int j = 1; j < tempRange.Height - 1; j++)
                    {
                        if (InitBoundaries.IsOnGrainBoundaries(tempRange, new Point(i, j)))
                        {
                            tempRange.GrainsArray[i, j].Energy_H = nucleons.EnergyOnEdges;
                        }
                        else
                        {
                            tempRange.GrainsArray[i, j].Energy_H = nucleons.EnergyInside;
                        }
                    }
                }
            }
            return tempRange;
        }

        public void EnergyVisualization(ref Range tempRange, InitNucleons nucleons)
        {
            for (int i = 1; i < tempRange.Width - 1; i++)
            {
                for (int j = 1; j < tempRange.Height - 1; j++)
                {
                    if(tempRange.GrainsArray[i,j].Energy_H == nucleons.EnergyInside)
                    {
                        tempRange.GrainsArray[i, j].Color = Color.Blue;
                    }
                    else
                    {
                        tempRange.GrainsArray[i, j].Color = Color.Green;
                    }

                    if(tempRange.GrainsArray[i,j].Id == (int)SpecialId.Id.Nucleon)
                    {
                        tempRange.GrainsArray[i, j].Color = Color.Red;
                    }
                }

            }
        }
    }
}