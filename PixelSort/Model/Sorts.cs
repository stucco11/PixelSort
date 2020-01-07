using PixelSort.ViewModel;
using System;
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

    internal class SaturationSort : IComparer<Color>
    {
        /// <summary>
        /// Compare method for the brightness of each pixel
        /// </summary>
        /// <param name="x">Pixel x</param>
        /// <param name="y">Pixal y</param>
        /// <returns></returns>
        public int Compare(Color x, Color y)
        {
            if (x.GetSaturation() == 0 || y.GetSaturation() == 0)
            {
                return 0;
            }

            return x.GetSaturation().CompareTo(y.GetSaturation());
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
        /// Default method to be used for a new sorting instance
        /// </summary>
        /// <param name="path">Path of the image that will be processed</param>
        /// <param name="selectedSort">Enum of the selected sorting method to be used</param>
        /// <param name="lower">lower bounds for the brightness sort</param>
        /// <param name="upper">upper bounds for the brightness sort</param>
        /// <returns></returns>
        public Bitmap Sort(string path, SortingMethodsEnum selectedSort, double lower, double upper, int hP, int vP)
        {
            if (path == null || path.Equals(""))
            {
                return null;
            }
            Bitmap image = new Bitmap(@path);
            if (hP >= image.Width || vP >= image.Height)
            {
                switch (selectedSort)
                {
                    case SortingMethodsEnum.Brightness:
                        return SortByBrightness(image, lower, upper);

                    case SortingMethodsEnum.Hue:
                        return SortByHue(image);

                    case SortingMethodsEnum.Saturation:
                        return SortBySaturation(image);

                    default:
                        return image;
                }
            }
            int modx = image.Width % (1 + hP);
            int mody = image.Height % (1 + vP);
            List<Bitmap> partitions = PartitionImage(image, hP, vP, modx, mody);

            for (int i = 0; i < partitions.Count; ++i)
            {
                {
                    switch (selectedSort)
                    {
                        case SortingMethodsEnum.Brightness:
                            partitions[i] = SortByBrightness(partitions[i], lower, upper);
                            continue;

                        case SortingMethodsEnum.Hue:
                            partitions[i] = SortByHue(partitions[i]);
                            continue;

                        case SortingMethodsEnum.Saturation:
                            partitions[i] = SortBySaturation(partitions[i]);
                            continue;

                        default:
                            continue;
                    }
                }
            }

            image = Recombine(partitions, hP + 1);
            return image;
        }

        private Bitmap SortBySaturation(Bitmap toSort)
        {
            for (int i = 0; i < toSort.Height; ++i)
            {
                pixels.Clear();
                for (int j = 0; j < toSort.Width; ++j)
                {
                    pixels.Add(toSort.GetPixel(j, i));
                }
                pixels.Sort(new SaturationSort());

                for (int j = 0; j < pixels.Count; ++j)
                {
                    toSort.SetPixel(j, i, pixels[j]);
                }
            }

            return toSort;
        }

        /// <summary>
        /// Sorts the image by the Color.Brightness() float value. Can be filtered with the passed
        /// in lower and upper values
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
        /// Sorts the bitmap using the float value provided by Color.GetHue(). This usually goes in
        /// the order of red - orange - yellow - green - blue - purple - red
        /// </summary>
        /// <param name="toSort">Bitmap that needs to be sorted</param>
        /// <returns></returns>
        public Bitmap SortByHue(Bitmap toSort)
        {
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

        private Bitmap ExtractPixels(Bitmap image, int sX, int eX, int sY, int eY)
        {
            int a = 0;
            int b = 0;

            Bitmap part = new Bitmap(eX - sX, eY - sY);
            for (int j = sY; j < eY; ++j)
            {
                for (int i = sX; i < eX; ++i)
                {
                    part.SetPixel(a, b, image.GetPixel(i, j));
                    ++a;
                }
                a = 0;
                ++b;
            }
            return part;
        }

        private Bitmap MergedBitmaps(List<Bitmap> maps)
        {
            int height = maps[0].Height;
            int width = 0;
            foreach (Bitmap map in maps)
            {
                width += map.Width;
            }

            Bitmap result = new Bitmap(width, height);

            width = 0;
            using (Graphics g = Graphics.FromImage(result))
            {
                foreach (Bitmap map in maps)
                {
                    g.DrawImage(map, width, 0);
                    width += maps[0].Width;
                }
            }
            return result;
        }

        private List<Bitmap> PartitionImage(Bitmap image, int hP, int vP, int modx, int mody)
        {
            int startx = 0;
            int endx = 0;
            int starty = 0;
            int endy = 0;
            List<Bitmap> partitions = new List<Bitmap>();
            Bitmap part;
            if (hP + 1 > image.Width || vP + 1 > image.Height)
            {
                partitions.Add(image);
                return partitions;
            }

            for (int i = 0; i < (vP + 1); ++i)
            {
                for (int j = 0; j < (hP + 1); ++j)
                {
                    startx = j * (image.Width / (hP + 1));
                    endx = (j + 1) * (image.Width / (hP + 1));
                    if (j == hP)
                    {
                        endx = image.Width;
                    }
                    starty = i * (image.Height / (vP + 1));
                    endy = (i + 1) * (image.Height / (vP + 1));
                    if (i == vP)
                    {
                        endy = image.Height;
                    }
                    part = new Bitmap(ExtractPixels(image, startx, endx, starty, endy));
                    partitions.Add(part);
                }
            }

            return partitions;
        }

        private Bitmap Recombine(List<Bitmap> partitions, int hP)
        {
            List<Bitmap> toAdd = new List<Bitmap>();

            List<Bitmap> stackedPartitions = new List<Bitmap>();
            for (int i = 0; i < partitions.Count; ++i)
            {
                toAdd.Add(partitions[i]);
                if (toAdd.Count % hP == 0)
                {
                    stackedPartitions.Add(MergedBitmaps(toAdd));
                    toAdd.Clear();
                }
            }

            foreach (Bitmap part in stackedPartitions)
            {
                part.RotateFlip(RotateFlipType.Rotate270FlipNone);
            }

            Bitmap imageRecomb = MergedBitmaps(stackedPartitions);
            imageRecomb.RotateFlip(RotateFlipType.Rotate90FlipNone);

            return imageRecomb;
        }
    }
}