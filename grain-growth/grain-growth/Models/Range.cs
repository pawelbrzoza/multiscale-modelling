using System.Drawing;

namespace grain_growth.Models
{
    public class Range
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public bool IsFull { get; set; }

        public Bitmap StructureBitmap { get; set; }

        public Grain[,] GrainsArray { get; set; }

        public Range()
        {
            IsFull = false;
        }

        public Range(int width, int height)
        {
            Width = width;
            Height = height;
            GrainsArray = new Grain[width, height];
            StructureBitmap = new Bitmap(width, height);
        }

        public Range(int width, int height, bool state)
        {
            IsFull = state;
            Width = width;
            Height = height;
            GrainsArray = new Grain[width, height];
            StructureBitmap = new Bitmap(width, height);
        }
    }
}