using PixelSort.ViewModel;
using System.Collections.Generic;
using System.Drawing;

namespace PixelSort.Model
{
    /// <summary>
    /// ICompare method for a custome sort to be used for the brightness of each pixel
    /// </summary>
    internal class BrightSort : IComparer<Color>
    {
        /// <summary>
        /// Compare method for the brightness of each pixel
        /// </summary>
        /// <param name="x">Pixel x</param>
        /// <param name="y">Pixal y</param>
        /// <returns></returns>
        public int Compare(Color x, Color y)
        {
            if (x.GetBrightness() == 0 || y.GetBrightness() == 0)
            {
                return 0;
            }

            return x.GetBrightness().CompareTo(y.GetBrightness());
        }
    }
    
    /// <summary>
    /// ICompare method for a custom sort to be used for the hue of each pixel
    /// </summary>
    internal class HueSort : IComparer<Color>
    {
        public int Compare(Color x, Color y)
        {
            if (x.GetHue() == 0 || y.GetHue() == 0)
            {
                return 0;
            }

            return x.GetHue().CompareTo(y.GetHue());
        }
    }
    /// <summary>
    /// Holds the sorting methods that are available for use
    /// </summary>
    internal class Sorts
    {
        private List<Color> pixels = new List<Color>();
        private List<Color> temp = new List<Color>();

        /// <summary>
        /// Sorts the image by the Color.Brightness() float value. Can be filtered with the passed in lower and upper values
        /// </summary>
        /// <param name="toSort">Bitmap that needs to be sorted</param>
        /// <param name="lower">Lower bound for the brightness values</param>
        /// <param name="upper">Upper bound for the brightness values</param>
        /// <returns></returns>
        public Bitmap SortByBrightness(Bitmap toSort, double lower, double upper)
        {
            if (upper < lower)
            {
                double mid = upper;
                upper = lower;
                lower = mid;
            }
            if (upper > 1.0)
            {
                upper = 1.0;
            }
            if (lower < 0.0)
            {
                lower = 0.0;
            }

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

                int a = 0;
                bool first = false;
                int b = 0;
                for (int j = 0; j < pixels.Count; ++j)
                {
                    float bright = pixels[j].GetBrightness();
                    if (bright <= upper && bright >= lower)
                    {
                        if (!first)
                        {
                            a = j;
                            first = true;
                            continue;
                        }
                        b = j;
                    }
                    else
                    {
                        if (b > a)
                        {
                            temp = new List<Color>(pixels.GetRange(a, b - a + 1));
                            temp.Sort(new BrightSort());
                            for (int c = 0; c < b - a; ++c)
                            {
                                pixels[a] = temp[c];
                                ++a;
                            }
                        }
                        b = 0;
                        a = 0;
                        first = false;
                    }
                }

                if (b > a)
                {
                    temp = new List<Color>(pixels.GetRange(a, b - a + 1));
                    temp.Sort(new BrightSort());
                    int d = b - a;
                    for (int c = 0; c < d; ++c)
                    {
                        pixels[a] = temp[c];
                        ++a;
                    }
                }

                for (int j = 0; j < pixels.Count; ++j)
                {
                    toSort.SetPixel(j, i, pixels[j]);
                }
            }

            return toSort;
        }

        /// <summary>
        /// Sorts the bitmap using the float value provided by Color.GetHue(). This usually goes in the order of red - orange - yellow - green - blue - purple - red
        /// </summary>
        /// <param name="toSort">Bitmap that needs to be sorted</param>
        /// <returns></returns>
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

        /// <summary>
        /// Default method to be used for a new sorting instance
        /// </summary>
        /// <param name="path">Path of the image that will be processed</param>
        /// <param name="selectedSort">Enum of the selected sorting method to be used</param>
        /// <param name="lower">lower bounds for the brightness sort</param>
        /// <param name="upper">upper bounds for the brightness sort</param>
        /// <returns></returns>
        public Bitmap Sort(string path, SortingMethodsEnum selectedSort, double lower, double upper)
        {
            Bitmap image = new Bitmap(@path);
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