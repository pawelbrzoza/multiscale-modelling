using System;
using System.Drawing;

namespace grain_growth.Helpers
{
    public class RandomCoordinates
    {
        public static Point Get(int width, int height, Random random)
        {
            return new Point(random.Next(1, width - 1), random.Next(1, height - 1));
        }
    }
}
