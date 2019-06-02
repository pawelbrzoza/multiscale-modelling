using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace grain_growth.Helpers
{
    class WriteableBitmapDrawer
    {
        WriteableBitmap writeableBitmap;
        IntPtr backBuffer;
        int W, H;
        public WriteableBitmapDrawer(WriteableBitmap writeableBitmap)
        {
            this.writeableBitmap = writeableBitmap;
            backBuffer = writeableBitmap.BackBuffer;
            W = (int)writeableBitmap.Width;
            H = (int)writeableBitmap.Height;
        }

        public unsafe void SetPixel(int x, int y, int color)
        {
            if ((x >= 0) && (y >= 0) && (x < W) && (y < H))
            {
                byte* p = (byte*)backBuffer + y * writeableBitmap.BackBufferStride + x * 4;
                *((int*)p) = color;
            }
        }

        private void CirclePoints(int x, int y, int x0, int y0, int color)
        {
            SetPixel(x + x0, y + y0, color);
            SetPixel(y + x0, x + y0, color);
            SetPixel(y + x0, -x + y0, color);
            SetPixel(x + x0, -y + y0, color);
            SetPixel(-y + x0, -x + y0, color);
            SetPixel(-x + x0, -y + y0, color);
            SetPixel(-y + x0, x + y0, color);
            SetPixel(-x + x0, y + y0, color);
        }


        /*
        * Algorytm rysowania okręgu; dla każdego punktyu sprawdzane jest czy jest w zakresie
        * bitmapy, co trochę spowalnia rysowanie
        */
        public void DrawCircle(int x0, int y0, int radius, int color)
        {
            int x, y, d;
            x = 0;
            y = radius;
            d = 5 - 4 * radius;
            CirclePoints(x, y, x0, y0, color);
            while (y > x)
            {
                if (d < 0)
                {                      /* Select E */
                    d += x * 8 + 12;
                    x++;
                }
                else
                {                          /* Select SE */
                    d += (x - y) * 8 + 20;
                    x++;
                    y--;
                }
                CirclePoints(x, y, x0, y0, color);
            }
        }

        private void p4(int x, int y, int px, int py, int color)
        {
            SetPixel(px + x, py + y, color);
            SetPixel(px + y, py - x, color);
            SetPixel(px - x, py - y, color);
            SetPixel(px - y, py + x, color);
        }

        /*
        * Maksymalnie prosta wersja algorytmu, podobny czas wykonania, minimalne róznice
        * sprawiuające że okrąg jest bardziej kanciasty, więc lepiej stosować DrawCircle
        */
        public void DrawCircleSimplified(int px, int py, int r, int color)
        {
            int x, y, d;
            x = 0; y = r; d = (r - 1) / 2;
            while (true)
            {
                p4(x, y, px, py, color);
                if (d >= 0)
                {
                    x++; d -= x;
                }
                if (d < 0)
                {
                    y--; d += y;
                }
                if (y <= 0) return;
            }
        }
    }
}
