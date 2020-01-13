using PixelSort.ViewModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Authentication.ExtendedProtection;

namespace PixelSort.Model
{
    internal class BlueSort : IComparer<Color>
    {
        /// <summary>
        /// Compare method for the brightness of each pixel
        /// </summary>
        /// <param name="x">Pixel x</param>
        /// <param name="y">Pixal y</param>
        /// <returns></returns>
        public int Compare(Color x, Color y)
        {
            if (x.B == 0 || y.B == 0)
            {
                return 0;
            }

            return x.B.CompareTo(y.B);
        }
    }

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

    internal class GreenSort : IComparer<Color>
    {
        /// <summary>
        /// Compare method for the brightness of each pixel
        /// </summary>
        /// <param name="x">Pixel x</param>
        /// <param name="y">Pixal y</param>
        /// <returns></returns>
        public int Compare(Color x, Color y)
        {
            if (x.G == 0 || y.G == 0)
            {
                return 0;
            }

            return x.G.CompareTo(y.G);
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

    internal class RedSort : IComparer<Color>
    {
        /// <summary>
        /// Compare method for the brightness of each pixel
        /// </summary>
        /// <param name="x">Pixel x</param>
        /// <param name="y">Pixal y</param>
        /// <returns></returns>
        public int Compare(Color x, Color y)
        {
            if (x.R == 0 || y.R == 0)
            {
                return 0;
            }

            return x.R.CompareTo(y.R);
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
        public Bitmap Sort(string path, SortingMethodsEnum selectedSort, double lower, double upper, int hP, int vP, int rotationValue, RGBEnum colorChecked, AdditionalOptionsEnum addOps)
        {
            if (path == null || path.Equals(""))
            {
                return null;
            }
            Bitmap image = new Bitmap(@path);
            Bitmap origImage = new Bitmap(@path);
            if (addOps == AdditionalOptionsEnum.Extend)
            {
                image = ExtendSort(image);
            }

            if (hP >= image.Width || vP >= image.Height)
            {
                switch (selectedSort)
                {
                    case SortingMethodsEnum.Brightness:
                        image = SortByBrightness(image, lower, upper, addOps, selectedSort);
                        break;

                    case SortingMethodsEnum.Hue:
                        image = SortByHue(image, addOps, selectedSort);
                        break;

                    case SortingMethodsEnum.Saturation:
                        image = SortBySaturation(image, addOps, selectedSort);
                        break;

                    case SortingMethodsEnum.RGB:
                        image = SortByRGB(image, colorChecked, addOps, selectedSort);
                        break;

                    default:
                        break;
                }
            }
            else
            {
                int modx = image.Width % (1 + hP);
                int mody = image.Height % (1 + vP);
                List<Bitmap> partitions = PartitionImage(image, hP, vP, modx, mody);

                for (int i = 0; i < partitions.Count; ++i)
                {
                    {
                        switch (selectedSort)
                        {
                            case SortingMethodsEnum.Brightness:
                                partitions[i] = SortByBrightness(partitions[i], lower, upper, addOps, selectedSort);
                                continue;

                            case SortingMethodsEnum.Hue:
                                partitions[i] = SortByHue(partitions[i], addOps, selectedSort);
                                continue;

                            case SortingMethodsEnum.Saturation:
                                partitions[i] = SortBySaturation(partitions[i], addOps, selectedSort);
                                continue;

                            case SortingMethodsEnum.RGB:
                                partitions[i] = SortByRGB(partitions[i], colorChecked, addOps, selectedSort);
                                continue;

                            default:
                                continue;
                        }
                    }
                }

                image = Recombine(partitions, hP + 1);
            }
            if (addOps == AdditionalOptionsEnum.Extend)
            {
                image = UnExtendSort(image, origImage);
            }
            return image;
        }

        private Bitmap SortBySpiral(Bitmap image, SortingMethodsEnum selectedSort, double lower, double upper, RGBEnum color)
        {
            int endWidth = image.Width - 1;
            int endHeight = image.Height - 1;
            int startWidth = 0;
            int startHeight = 0;

            while (endWidth >= startWidth && endHeight >= startHeight)
            {
                pixels.Clear();
                for (int i = startWidth; i < endWidth; ++i)
                {
                    pixels.Add(image.GetPixel(i, startHeight));
                }
                for (int i = startHeight; i < endHeight; ++i)
                {
                    pixels.Add(image.GetPixel(endWidth, i));
                }
                for (int i = endWidth; i > startWidth; --i)
                {
                    pixels.Add(image.GetPixel(i, endHeight));
                }
                for (int i = endHeight; i > startHeight; --i)
                {
                    pixels.Add(image.GetPixel(startWidth, i));
                }

                if (selectedSort == SortingMethodsEnum.Brightness)
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

                    int a = 0;
                    bool first = false;
                    int b = 0;

                    for (int j = 0; j < pixels.Count; ++j)
                    {
                        float bright = pixels[j].GetBrightness();
                        if (pixels[j].A == 0)
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

                    pixels.Sort(new BrightSort());
                }

                if (selectedSort == SortingMethodsEnum.Hue)
                {
                    pixels.Sort(new HueSort());
                }

                if (selectedSort == SortingMethodsEnum.Saturation)
                {
                    pixels.Sort(new SaturationSort());
                }
                if (selectedSort == SortingMethodsEnum.RGB)
                {
                    switch (color)
                    {
                        case RGBEnum.Blue:
                            pixels.Sort(new BlueSort());
                            break;

                        case RGBEnum.Green:
                            pixels.Sort(new GreenSort());
                            break;

                        default:
                            pixels.Sort(new RedSort());
                            break;
                    }
                }

                int count = 0;

                for (int i = startWidth; i < endWidth; ++i)
                {
                    image.SetPixel(i, startHeight, pixels[count]);
                    count++;
                }
                for (int i = startHeight; i < endHeight; ++i)
                {
                    pixels.Add(image.GetPixel(endWidth, i));
                    image.SetPixel(endWidth, i, pixels[count]);
                    count++;
                }
                for (int i = endWidth; i > startWidth; --i)
                {
                    image.SetPixel(i, endHeight, pixels[count]);
                    count++;
                }
                for (int i = endHeight; i > startHeight; --i)
                {
                    pixels.Add(image.GetPixel(startWidth, i));
                    image.SetPixel(startWidth, i, pixels[count]);
                    count++;
                }

                ++startWidth;
                ++startHeight;
                --endWidth;
                --endHeight;
            }
            return image;
        }

        private Bitmap UnExtendSort(Bitmap image, Bitmap origImage)
        {
            int width = origImage.Width;
            int height = origImage.Height;
            int a = 0;
            Bitmap newImage = new Bitmap(width, height);

            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    newImage.SetPixel(j, i, image.GetPixel(a, 0));
                    ++a;
                }
            }

            return newImage;
        }

        private Bitmap ExtendSort(Bitmap image)
        {
            int width = image.Width;
            int height = image.Height;
            Bitmap newImage = new Bitmap(width * height, 1);
            int a = 0;
            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    newImage.SetPixel(a, 0, image.GetPixel(j,i));
                    ++a;
                }
            }

            return newImage;
        }

        /// <summary>
        /// Sorts the image by the Color.Brightness() float value. Can be filtered with the passed
        /// in lower and upper values
        /// </summary>
        /// <param name="toSort">Bitmap that needs to be sorted</param>
        /// <param name="lower">Lower bound for the brightness values</param>
        /// <param name="upper">Upper bound for the brightness values</param>
        /// <returns></returns>
        public Bitmap SortByBrightness(Bitmap toSort, double lower, double upper, AdditionalOptionsEnum addOps, SortingMethodsEnum selectedSort)
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

            if (addOps != AdditionalOptionsEnum.Spiral)
            {
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
                        if (pixels[j].A == 0)
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
            } else
            {
                toSort = SortBySpiral(toSort, selectedSort, lower, upper, RGBEnum.Blue);
            }
            return toSort;
        }

        /// <summary>
        /// Sorts the bitmap using the float value provided by Color.GetHue(). This usually goes in
        /// the order of red - orange - yellow - green - blue - purple - red
        /// </summary>
        /// <param name="toSort">Bitmap that needs to be sorted</param>
        /// <returns></returns>
        public Bitmap SortByHue(Bitmap toSort, AdditionalOptionsEnum addOps, SortingMethodsEnum selectedSort)
        {
            if (addOps != AdditionalOptionsEnum.Spiral)
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
            } else
            {
                toSort = SortBySpiral(toSort, selectedSort, 0, 0, RGBEnum.Blue);
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

        private Point[] FindRotationPoints(Bitmap image, int rotationValue)
        {
            Point[] points = {
                new Point(0,0), //Upper left
                new Point(image.Width,0), //Upper right
                new Point(0,image.Height) //Lower left
            };

            for (int i = 0; i < points.Length; ++i)
            {
                int x = points[i].X;
                int y = points[i].Y;

                points[i].X = Convert.ToInt32((x * Math.Cos(rotationValue * (Math.PI / 180.0))) - (y * Math.Sin(rotationValue * (Math.PI / 180.0))));
                points[i].Y = Convert.ToInt32((x * Math.Sin(rotationValue * (Math.PI / 180.0))) + (y * Math.Cos(rotationValue * (Math.PI / 180.0))));
            }

            return points;
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

        private Bitmap RotateImage(Bitmap image, Point[] points, int rotationValue)
        {
            return image;
            int maxX = 0;
            int maxY = 0;
            int minX = 0;
            int minY = 0;

            for (int i = 0; i < points.Length; ++i)
            {
                if (points[i].X > maxX)
                {
                    maxX = points[i].X;
                }
                if (points[i].X < minX)
                {
                    minX = points[i].X;
                }
                if (points[i].Y > maxY)
                {
                    maxY = points[i].Y;
                }
                if (points[i].Y < minY)
                {
                    minY = points[i].Y;
                }
            }

            if (Convert.ToInt32((image.Width * Math.Sin(rotationValue * (Math.PI / 180.0))) + (image.Width * Math.Cos(rotationValue * (Math.PI / 180.0)))) < minY)
            {
                minY = Convert.ToInt32((image.Width * Math.Sin(rotationValue * (Math.PI / 180.0))) + (image.Height * Math.Cos(rotationValue * (Math.PI / 180.0))));
            }
            if (Convert.ToInt32((image.Width * Math.Sin(rotationValue * (Math.PI / 180.0))) + (image.Width * Math.Cos(rotationValue * (Math.PI / 180.0)))) > maxY)
            {
                maxY = Convert.ToInt32((image.Width * Math.Sin(rotationValue * (Math.PI / 180.0))) + (image.Height * Math.Cos(rotationValue * (Math.PI / 180.0))));
            }
            if (Convert.ToInt32((image.Width * Math.Cos(rotationValue * (Math.PI / 180.0))) - (image.Height * Math.Sin(rotationValue * (Math.PI / 180.0)))) < minX)
            {
                minX = Convert.ToInt32((image.Width * Math.Cos(rotationValue * (Math.PI / 180.0))) - (image.Height * Math.Sin(rotationValue * (Math.PI / 180.0))));
            }
            if (Convert.ToInt32((image.Width * Math.Cos(rotationValue * (Math.PI / 180.0))) - (image.Height * Math.Sin(rotationValue * (Math.PI / 180.0)))) > maxX)
            {
                maxX = Convert.ToInt32((image.Width * Math.Cos(rotationValue * (Math.PI / 180.0))) - (image.Height * Math.Sin(rotationValue * (Math.PI / 180.0))));
            }

            int newX = maxX - minX;
            int newY = maxY - minY;

            Bitmap blankImage = new Bitmap(newX, newY);

            for (int i = 0; i < points.Length; ++i)
            {
                points[i].X += Math.Abs(minX);
                points[i].Y += Math.Abs(minY);
            }

            // Draw the image unaltered with its upper-left corner at (0, 0).
            Graphics graphic = Graphics.FromImage(blankImage);
            graphic.DrawImage(image, points);
            graphic.Dispose();

            // Draw the image mapped to the parallelogram.

            return blankImage;
        }

        private Bitmap RotateImageUndo(Bitmap image, Point[] points, int rotationValue, Bitmap toRotate)
        {
            
            int minX = 0;
            int minY = 0;

            for (int i = 0; i < points.Length; ++i)
            {
                if (points[i].X < minX)
                {
                    minX = points[i].X;
                }
                if (points[i].Y < minY)
                {
                    minY = points[i].Y;
                }
            }
            if (Convert.ToInt32((image.Width * Math.Sin(rotationValue * (Math.PI / 180.0))) + (image.Width * Math.Cos(rotationValue * (Math.PI / 180.0)))) < minY)
            {
                minY = Convert.ToInt32((image.Width * Math.Sin(rotationValue * (Math.PI / 180.0))) + (image.Height * Math.Cos(rotationValue * (Math.PI / 180.0))));
            }
            if (Convert.ToInt32((image.Width * Math.Cos(rotationValue * (Math.PI / 180.0))) - (image.Height * Math.Sin(rotationValue * (Math.PI / 180.0)))) < minX)
            {
                minX = Convert.ToInt32((image.Width * Math.Cos(rotationValue * (Math.PI / 180.0))) - (image.Height * Math.Sin(rotationValue * (Math.PI / 180.0))));
            }

            Bitmap blankImage = new Bitmap(toRotate.Width, toRotate.Height);

            for (int i = 0; i < points.Length; ++i)
            {
                points[i].X -= minX;
                points[i].Y -= minY;
            }

            Graphics graphics = Graphics.FromImage(blankImage);
            graphics.DrawImage(toRotate, points);
            graphics.Dispose();
            Bitmap toReturn = new Bitmap(image.Width, image.Height);
            int b = 0;
            for (int i = blankImage.Height - image.Height; i < blankImage.Height; ++i)
            {
                int a = 0;
                for (int j = blankImage.Width - image.Width; j < blankImage.Width; ++j)
                {
                    toReturn.SetPixel(b, a, blankImage.GetPixel(i, j));
                    a++;
                }
                b++;
            }
            return toReturn;
        }

        private Bitmap SortByRGB(Bitmap toSort, RGBEnum colorChecked, AdditionalOptionsEnum addOps, SortingMethodsEnum selectedSort)
        {
            if (addOps != AdditionalOptionsEnum.Spiral)
            {
                for (int i = 0; i < toSort.Height; ++i)
                {
                    pixels.Clear();
                    for (int j = 0; j < toSort.Width; ++j)
                    {
                        pixels.Add(toSort.GetPixel(j, i));
                    }

                    switch (colorChecked)
                    {
                        case RGBEnum.Red:
                            pixels.Sort(new RedSort());
                            break;

                        case RGBEnum.Blue:
                            pixels.Sort(new BlueSort());
                            break;

                        default:
                            pixels.Sort(new GreenSort());
                            break;
                    }

                    for (int j = 0; j < pixels.Count; ++j)
                    {
                        toSort.SetPixel(j, i, pixels[j]);
                    }
                }
            } else
            {
                toSort = SortBySpiral(toSort, selectedSort, 0, 0, colorChecked);
            }
                

            return toSort;
        }
        private Bitmap SortBySaturation(Bitmap toSort, AdditionalOptionsEnum addOps, SortingMethodsEnum selectedSort)
        {
            if (addOps != AdditionalOptionsEnum.Spiral)
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
            }else
            {
                toSort = SortBySpiral(toSort, selectedSort, 0, 0, RGBEnum.Blue);
            }

            return toSort;
        }
    }
}