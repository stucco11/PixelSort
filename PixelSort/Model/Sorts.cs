using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace PixelSort.Model
{
    class BrightSort : IComparer<Color>
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
    class Sorts
    {
        List<Color> pixels = new List<Color>();
        Boolean added = false;

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
