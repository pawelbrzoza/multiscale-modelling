using grain_growth.MainMethods;
using grain_growth.Helpers;
using System.Collections.Generic;
using System.Drawing;

namespace grain_growth.Models
{
    public class InitBoundaries : Boundaries
    {
        public InitBoundaries(MainProperties properties)
        {

            ClearBoundaries = new Range(properties.RangeWidth, properties.RangeHeight);
            BoundariesAll = new Range(properties.RangeWidth, properties.RangeHeight);
            BoundariesSelected = new Range(properties.RangeWidth, properties.RangeHeight);
            BoundariesSingleSelect = new Range(properties.RangeWidth, properties.RangeHeight);

            InitStructures.AddBlackBorder(ClearBoundaries);
            InitStructures.AddBlackBorder(BoundariesAll);
            InitStructures.AddBlackBorder(BoundariesSelected);
            InitStructures.AddBlackBorder(BoundariesSingleSelect);

            // init grains array by default values
            for (int i = 1; i < properties.RangeWidth - 1; i++)
            {
                for (int j = 1; j < properties.RangeHeight - 1; j++)
                {
                    ClearBoundaries.GrainsArray[i, j] = new Grain()
                    {
                        Id = 0,
                        Color = Color.White,
                        Energy_H = 0
                    };
                    BoundariesAll.GrainsArray[i, j] = new Grain()
                    {
                        Id = 0,
                        Color = Color.White,
                        Energy_H = 0
                    };
                    BoundariesSelected.GrainsArray[i, j] = new Grain()
                    {
                        Id = 0,
                        Color = Color.White,
                        Energy_H = 0
                    };
                    BoundariesSingleSelect.GrainsArray[i, j] = new Grain()
                    {
                        Id = 0,
                        Color = Color.White,
                        Energy_H = 0
                    };
                }
            }
        }

        public static bool IsOnGrainBoundaries(Range range, Point point)
        {
            var centerId = range.GrainsArray[point.X, point.Y].Id;
            var neighboursIds = new List<int>
            {
                range.GrainsArray[point.X - 1, point.Y].Id,
                range.GrainsArray[point.X + 1, point.Y].Id,
                range.GrainsArray[point.X, point.Y - 1].Id,
                range.GrainsArray[point.X, point.Y + 1].Id,
                range.GrainsArray[point.X - 1, point.Y - 1].Id,
                range.GrainsArray[point.X - 1, point.Y + 1].Id,
                range.GrainsArray[point.X + 1, point.Y - 1].Id,
                range.GrainsArray[point.X + 1, point.Y + 1].Id
            };

            foreach (var neighbourId in neighboursIds)
            {
                if (centerId != neighbourId && !SpecialId.IsIdSpecial(neighbourId))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsOnGrainBoundariesOutside(Range range, Point point, Color color)
        {
            var neighbours = new List<Grain>
            {
                range.GrainsArray[point.X - 1, point.Y],
                range.GrainsArray[point.X + 1, point.Y],
                range.GrainsArray[point.X, point.Y - 1],
                range.GrainsArray[point.X, point.Y + 1],
                range.GrainsArray[point.X - 1, point.Y - 1],
                range.GrainsArray[point.X - 1, point.Y + 1],
                range.GrainsArray[point.X + 1, point.Y - 1],
                range.GrainsArray[point.X + 1, point.Y + 1]
            };

            foreach (var neighbour in neighbours)
            {
                if (neighbour.Color == color && neighbour.Id != 0 && neighbour.Id != -1)
                {
                    return true;
                }
            }
            return false;
        }

        public void DrawSingleSelect(Range tempRange, Color color)
        {
            for (int i = 1; i < tempRange.Width - 1; i++)
            {
                for (int j = 1; j < tempRange.Height - 1; j++)
                {
                    if (IsOnGrainBoundariesOutside(tempRange, new System.Drawing.Point(i, j), color) &&
                        tempRange.GrainsArray[i, j].Color != color)
                    {
                        BoundariesSingleSelect.GrainsArray[i, j].Color = Color.Black;
                        BoundariesSingleSelect.GrainsArray[i, j].Id = -1;
                    }
                    else
                    {
                        BoundariesSingleSelect.GrainsArray[i, j].Color = tempRange.GrainsArray[i, j].Color;
                        BoundariesSingleSelect.GrainsArray[i, j].Id = tempRange.GrainsArray[i, j].Id;
                    }
                }
            }
            CellularAutomata.UpdateBitmap(BoundariesSingleSelect);
        }

        public void GenerateBoundariesAll(Range tempRange)
        {
            for (int i = 1; i < tempRange.Width - 1; i++)
            {
                for (int j = 1; j < tempRange.Height - 1; j++)
                {
                    if (IsOnGrainBoundaries(tempRange, new Point(i, j)))
                    {
                        BoundariesAll.GrainsArray[i, j].Color = Color.Black;
                        BoundariesAll.GrainsArray[i, j].Id = -1;
                        ClearBoundaries.GrainsArray[i, j] = BoundariesAll.GrainsArray[i, j];
                    }
                    else
                    {
                        BoundariesAll.GrainsArray[i, j].Color = tempRange.GrainsArray[i, j].Color;
                        BoundariesAll.GrainsArray[i, j].Id = tempRange.GrainsArray[i, j].Id;
                    }
                }
            }
            CellularAutomata.UpdateBitmap(BoundariesAll);
        }

        public void GenerateBoundariesSelected(Range tempRange)
        {
            BoundariesSelected = tempRange;
            foreach (var point in Substructures.SubStrucrtuePointsList)
            {
                var color = tempRange.StructureBitmap.GetPixel(point.X, point.Y);

                DrawSingleSelect(tempRange, color);

                for (int i = 1; i < BoundariesSingleSelect.Width - 1; i++)
                {
                    for (int j = 1; j < BoundariesSingleSelect.Height - 1; j++)
                    {
                        if (BoundariesSingleSelect.GrainsArray[i, j].Color == Color.Black)
                        {
                            BoundariesSelected.GrainsArray[i, j] = BoundariesSingleSelect.GrainsArray[i, j];
                            ClearBoundaries.GrainsArray[i, j] = BoundariesSingleSelect.GrainsArray[i, j];
                        }
                    }
                }
            }
            CellularAutomata.UpdateBitmap(BoundariesSelected);
        }
    }
}
