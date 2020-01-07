using PixelSort.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace PixelSort.Model 
{   
    internal class BrightSort : IComparer<Color>
    {
        private double lower;
        private double upper;

        public BrightSort(double lower, double upper)
        {
            this.lower = lower;
            this.upper = upper;
        }

        public int Compare(Color x, Color y)
        {
            if (x.GetBrightness() == 0 || y.GetBrightness() == 0)
            {
                return 0;
            }
            /*
            if (lower < 0)
            {
                lower = 0.0;
            }
            if (upper > 1)
            {
                upper = 1.0;
            }

            if (x.GetBrightness() > upper || x.GetBrightness() < lower || y.GetBrightness() > upper || y.GetBrightness() < lower)
            {
                return 0;
            }
            */
            // CompareTo() method
            return x.GetBrightness().CompareTo(y.GetBrightness());
        }
    }

    internal class HueSort : IComparer<Color>
    {
        public int Compare(Color x, Color y)
        {
            if (x.GetHue() == 0 || y.GetHue() == 0)
            {
                return 0;
            }

            // CompareTo() method
            return x.GetHue().CompareTo(y.GetHue());
        }
    }

    internal class Sorts
    {

        private List<Color> pixels = new List<Color>();

        public Bitmap SortByBrightness(Bitmap toSort, double lower, double upper)
        {
            BrightSort brightSort = new BrightSort(lower, upper);
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
                /*
                int a = 0;
                int b = 0;
                for (int j = 0; j < pixels.Count; ++j)
                {
                    float bright = pixels[j].GetBrightness();
                    if (bright <= upper && bright >= lower)
                    {
                        if (a == 0)
                        {
                            a = j;
                            continue;
                        }
                        b = j;
                    } else
                    {
                        if ( b > a)
                        {
                            pixels.GetRange(a, b - a).Sort(brightSort);
                        }
                        b = 0;
                        a = 0;
                    }
                }
                if (b > a)
                {
                    pixels.GetRange(a, b - a).Sort(brightSort);
                }
                */
                pixels.Sort(brightSort);

                for (int j = 0; j < pixels.Count; ++j)
                {
                    toSort.SetPixel(j, i, pixels[j]);
                }
            }

            return toSort;
        }

        public Bitmap SortByHue(Bitmap toSort)
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
                pixels.Sort(new HueSort());

                for (int j = 0; j < pixels.Count; ++j)
                {
                    toSort.SetPixel(j, i, pixels[j]);
                }
            }

            return toSort;
        }

        public Bitmap Sort(Bitmap image, SortingMethodsEnum selectedSort, double lower, double upper)
        {
            switch (selectedSort)
            {
                case SortingMethodsEnum.Brightness:
                    return SortByBrightness(image, lower, upper);
                case SortingMethodsEnum.Hue:
                    return SortByHue(image);
                default:
                    return image;
            }
        }
    }
}