using System.Collections.Generic;
using System.Drawing;

namespace PixelSort.Model
{
    internal class BrightSort : IComparer<Color>
    {
        public int Compare(Color x, Color y)
        {
            if (x.GetBrightness() == 0 || y.GetBrightness() == 0)
            {
                return 0;
            }

            // CompareTo() method
            return x.GetBrightness().CompareTo(y.GetBrightness());
        }
    }

    internal class Sorts
    {
        private List<Color> pixels = new List<Color>();

        public Bitmap SortByBrightness(Bitmap toSort)
        {
            if (toSort == null)
            {
                return null;
            }
            for (int i = 0; i < toSort.Height; ++i)
            {
                pixels.Clear();
                for (int j = 0; j < toSort.Width; ++j)
                {
                    pixels.Add(toSort.GetPixel(j, i));
                }
                pixels.Sort(new BrightSort());

                for (int j = 0; j < pixels.Count; ++j)
                {
                    toSort.SetPixel(j, i, pixels[j]);
                }
            }

            return toSort;
        }
    }
}