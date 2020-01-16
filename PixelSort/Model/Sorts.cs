using PixelSort.ViewModel;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace PixelSort.Model
{
    /// <summary>
    /// ICompare method of a custom sort to be used for sorting pixels by their blue values
    /// </summary>
    internal class BlueSort : IComparer<Color>
    {
        /// <summary> Compare method for the brightness of each pixel </summary> <param
        /// name="x">Pixel x</param> <param name="y">Pixel y</param> <returns>-1, 0, or 1 (-1 means
        /// x < y, 0 means x == y, 1 means x > y)</returns>
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
        /// <summary> Compare method for the brightness of each pixel </summary> <param
        /// name="x">Pixel x</param> <param name="y">Pixel y</param> <returns>-1, 0, or 1 (-1 means
        /// x < y, 0 means x == y, 1 means x > y)</returns>
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
    /// ICompare method of a custom sort to be used for sorting pixels by their green values
    /// </summary>
    internal class GreenSort : IComparer<Color>
    {
        /// <summary> Compare method for the brightness of each pixel </summary> <param
        /// name="x">Pixel x</param> <param name="y">Pixel y</param> <returns>-1, 0, or 1 (-1 means
        /// x < y, 0 means x == y, 1 means x > y)</returns>
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
        /// <summary> Compare method for the hue of each pixel </summary> <param name="x"></param>
        /// <param name="y"></param> <returns>-1, 0, or 1 (-1 means x < y, 0 means x == y, 1 means x
        /// > y)</returns>
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
    /// ICompare method of a custom sort to be used for sorting pixels by their red values
    /// </summary>
    internal class RedSort : IComparer<Color>
    {
        /// <summary> Compare method for the brightness of each pixel </summary> <param
        /// name="x">Pixel x</param> <param name="y">Pixel y</param> <returns>-1, 0, or 1 (-1 means
        /// x < y, 0 means x == y, 1 means x > y)</returns>
        public int Compare(Color x, Color y)
        {
            if (x.R == 0 || y.R == 0)
            {
                return 0;
            }

            return x.R.CompareTo(y.R);
        }
    }

    /// <summary>
    /// ICompare method for a custom sort to be used for the saturation of each pixel
    /// </summary>
    internal class SaturationSort : IComparer<Color>
    {
        /// <summary> Compare method for the brightness of each pixel </summary> <param
        /// name="x">Pixel x</param> <param name="y">Pixel y</param> <returns>-1, 0, or 1 (-1 means
        /// x < y, 0 means x == y, 1 means x > y)</returns>
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
        public Bitmap Sort(string path, SortingMethodsEnum selectedSort, double lower, double upper, int hP, int vP, RGBEnum colorChecked, AdditionalOptionsEnum addOps, DirectionEnum directionChecked)
        {
            // if path is null or "", return null
            if (path == null || path.Equals(""))
            {
                return null;
            }

            // creates two Bitmaps of the same image, handy for keeping the original width/height
            // and other data
            Bitmap image = new Bitmap(@path);
            Bitmap origImage = new Bitmap(@path);

            image = RotateImage(image, directionChecked);
            origImage = RotateImage(origImage, directionChecked);

            // if Extend is clicked, ExtendSort the image
            if (addOps == AdditionalOptionsEnum.Extend)
            {
                image = ExtendSort(image);
            }

            // If the number of horizontal or vertical partitions is greater than or equal to the
            // width or height of the image, don't partition the image and sort with the whole image instead
            if (hP >= image.Width || vP >= image.Height)
            {
                // Switch statement for using the correct sort based on the selected method
                switch (selectedSort)
                {
                    case SortingMethodsEnum.Brightness:
                        image = SortByBrightness(image, lower, upper, addOps, selectedSort);
                        break;

                    case SortingMethodsEnum.Hue:
                        image = SortByHue(image, addOps, selectedSort, lower, upper);
                        break;

                    case SortingMethodsEnum.Saturation:
                        image = SortBySaturation(image, addOps, selectedSort, lower, upper);
                        break;

                    case SortingMethodsEnum.RGB:
                        image = SortByRGB(image, colorChecked, addOps, selectedSort, lower, upper);
                        break;

                    default:
                        break;
                }
            }
            // If the number of partitions wanted is within the image's width/height, split the
            // image into (horizontal partitions + 1) * (vertical partitions + 1), sort the
            // paritions, and then stitch them back together
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
                                partitions[i] = SortByHue(partitions[i], addOps, selectedSort, lower, upper);
                                continue;

                            case SortingMethodsEnum.Saturation:
                                partitions[i] = SortBySaturation(partitions[i], addOps, selectedSort, lower, upper);
                                continue;

                            case SortingMethodsEnum.RGB:
                                partitions[i] = SortByRGB(partitions[i], colorChecked, addOps, selectedSort, lower, upper);
                                continue;

                            default:
                                continue;
                        }
                    }
                }

                image = Recombine(partitions, hP + 1);
            }

            // if Extending was selected, then put the image back together so it's no longer a single line of pixels
            if (addOps == AdditionalOptionsEnum.Extend)
            {
                image = UnExtendSort(image, origImage);
            }

            image = RotateImageBack(image, directionChecked);

            // return the image to the user
            return image;
        }

        private Bitmap RotateImage(Bitmap image, DirectionEnum directionChecked)
        {
            switch (directionChecked)
            {
                case DirectionEnum.Up:
                    image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    return image;

                case DirectionEnum.Down:
                    image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    return image;

                case DirectionEnum.Left:
                    image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    return image;

                default:
                    return image;
            }
        }

        private Bitmap RotateImageBack(Bitmap image, DirectionEnum directionChecked)
        {
            switch (directionChecked)
            {
                case DirectionEnum.Up:
                    image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    return image;

                case DirectionEnum.Down:
                    image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    return image;

                case DirectionEnum.Left:
                    image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    return image;

                default:
                    return image;
            }
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
            // Ensures that upper > lower
            if (upper < lower)
            {
                double mid = upper;
                upper = lower;
                lower = mid;
            }

            // Sets upper and lower so that neither is outside of the bounds of 0.0 and 1.0
            if (upper > 1.0)
            {
                upper = 1.0;
            }
            if (lower < 0.0)
            {
                lower = 0.0;
            }

            // If spiral wasn't selected, then run the following code
            if (addOps != AdditionalOptionsEnum.Spiral)
            {
                // loops through the height of the image
                for (int i = 0; i < toSort.Height; ++i)
                {
                    // clears pixels every time
                    pixels.Clear();

                    // Adds each row to pixels
                    for (int j = 0; j < toSort.Width; ++j)
                    {
                        pixels.Add(toSort.GetPixel(j, i));
                    }

                    // Sort pixels with respect to bounds
                    pixels = BrightSortWithBounds(pixels, lower, upper);

                    // place each pixel from pixels into the correct row of the image
                    for (int j = 0; j < pixels.Count; ++j)
                    {
                        toSort.SetPixel(j, i, pixels[j]);
                    }
                }
            }

            // if spiral was selected, then run SortBySpiral
            else
            {
                toSort = SortBySpiral(toSort, selectedSort, lower, upper, RGBEnum.Blue);
            }

            // return the image
            return toSort;
        }

        /// <summary>
        /// Sorts the bitmap using the float value provided by Color.GetHue(). This usually goes in
        /// the order of red - orange - yellow - green - blue - purple - red
        /// </summary>
        /// <param name="toSort">Bitmap that needs to be sorted</param>
        /// <returns></returns>
        public Bitmap SortByHue(Bitmap toSort, AdditionalOptionsEnum addOps, SortingMethodsEnum selectedSort, double lower, double upper)
        {
            // If spiral wasn't selected, then run the following code
            if (addOps != AdditionalOptionsEnum.Spiral)
            {
                // Iterate through each row of the image, sort that row according to HueSort, and then replace the unsorted row with the sorted row
                for (int i = 0; i < toSort.Height; ++i)
                {
                    pixels.Clear();
                    for (int j = 0; j < toSort.Width; ++j)
                    {
                        pixels.Add(toSort.GetPixel(j, i));
                    }
                    pixels = HueSortWithBounds(pixels, lower, upper);

                    for (int j = 0; j < pixels.Count; ++j)
                    {
                        toSort.SetPixel(j, i, pixels[j]);
                    }
                }
            }

            // if spiral was selected, then run SortBySpiral
            else
            {
                toSort = SortBySpiral(toSort, selectedSort, lower, upper, RGBEnum.Blue);
            }

            return toSort;
        }

        /// <summary>
        /// Transforms an image of x by y into an image of 1 by x * y
        /// </summary>
        /// <param name="image">The image to transform into a line</param>
        /// <returns>Returns the image that is now 1 by width * height</returns>
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
                    newImage.SetPixel(a, 0, image.GetPixel(j, i));
                    ++a;
                }
            }

            return newImage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image">The image to pull the pixel data from</param>
        /// <param name="sX">Starting value for x</param>
        /// <param name="eX">Ending value for x</param>
        /// <param name="sY">Starting value for y</param>
        /// <param name="eY">Ending value for y</param>
        /// <returns>Returns a bitmaps that is image from sx to ex and sy to ey</returns>
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

        /// <summary>
        /// Merges bitmaps into one bitmap from left to right
        /// </summary>
        /// <param name="maps">The list of bitmaps to merge together</param>
        /// <returns>Returns a Bitmap that is merged from the passed in maps</returns>
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

        /// <summary>
        /// Takes the image and creates partitions of the image that are returned in a list
        /// </summary>
        /// <param name="image">Image to partitions</param>
        /// <param name="hP">Number of horizontal partitions</param>
        /// <param name="vP">Number of vertical partitions</param>
        /// <param name="modx">Remaining number of pixels in width</param>
        /// <param name="mody">Remaining number of pixels in height</param>
        /// <returns></returns>
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

        /// <summary>
        /// Fuses the list of Bitmaps back into the image that is the correct height and width based on the number of horizontal partitions
        /// </summary>
        /// <param name="partitions">List of Bitmaps that will be combined</param>
        /// <param name="hP">Number of horizontal paritions</param>
        /// <returns>Returns a combined bitmap</returns>
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

        /// <summary>
        /// Sorts the image by the selected RGB value (either Red, Green, or Blue)
        /// </summary>
        /// <param name="toSort">Image that is to be sorted</param>
        /// <param name="colorChecked">The color that is selected</param>
        /// <param name="addOps">Additional operations that are selected</param>
        /// <param name="selectedSort">The selected sorting method</param>
        /// <returns>Returns the image that is sorted by Reds, Greens, or Blues</returns>
        private Bitmap SortByRGB(Bitmap toSort, RGBEnum colorChecked, AdditionalOptionsEnum addOps, SortingMethodsEnum selectedSort, double lower, double upper)
        {
            // If spiral wasn't selected, then run the following code
            if (addOps != AdditionalOptionsEnum.Spiral)
            {
                for (int i = 0; i < toSort.Height; ++i)
                {
                    pixels.Clear();
                    for (int j = 0; j < toSort.Width; ++j)
                    {
                        pixels.Add(toSort.GetPixel(j, i));
                    }

                    pixels = RGBSortWithBounds(pixels, lower, upper, colorChecked);
                    
                    for (int j = 0; j < pixels.Count; ++j)
                    {
                        toSort.SetPixel(j, i, pixels[j]);
                    }
                }
            }

            // if spiral was selected, then run SortBySpiral
            else
            {
                toSort = SortBySpiral(toSort, selectedSort, lower, upper, colorChecked);
            }

            // returns the sorted image
            //toSort.SetResolution(72.0F, 72.0F);
            return toSort;
        }

        /// <summary>
        /// Sorts the image by saturation
        /// </summary>
        /// <param name="toSort">Image that is to be sorted</param>
        /// <param name="addOps">Additional operations that are selected</param>
        /// <param name="selectedSort">The selected sorting method</param>
        /// <returns></returns>
        private Bitmap SortBySaturation(Bitmap toSort, AdditionalOptionsEnum addOps, SortingMethodsEnum selectedSort, double lower, double upper)
        {
            // If spiral wasn't selected, then run the following code
            if (addOps != AdditionalOptionsEnum.Spiral)
            {
                for (int i = 0; i < toSort.Height; ++i)
                {
                    pixels.Clear();
                    for (int j = 0; j < toSort.Width; ++j)
                    {
                        pixels.Add(toSort.GetPixel(j, i));
                    }
                    pixels = SaturationSortWithBounds(pixels, lower, upper);

                    for (int j = 0; j < pixels.Count; ++j)
                    {
                        toSort.SetPixel(j, i, pixels[j]);
                    }
                }
            }

            // if spiral was selected, then run SortBySpiral
            else
            {
                toSort = SortBySpiral(toSort, selectedSort, lower, upper, RGBEnum.Blue);
            }

            // returns the sorted image
            return toSort;
        }

        private List<Color> SaturationSortWithBounds(List<Color> pixelList, double lower, double upper)
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

            for (int j = 0; j < pixelList.Count; ++j)
            {
                float bright = pixelList[j].GetSaturation();
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
                        temp = new List<Color>(pixelList.GetRange(a, b - a + 1));
                        temp.Sort(new SaturationSort());
                        for (int c = 0; c < temp.Count; ++c)
                        {
                            pixelList[a] = temp[c];
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
                temp = new List<Color>(pixelList.GetRange(a, b - a + 1));
                temp.Sort(new SaturationSort());
                int d = b - a;
                for (int c = 0; c < d; ++c)
                {
                    pixelList[a] = temp[c];
                    ++a;
                }
            }

            return pixelList;
        }

        /// <summary>
        /// This will sort the passed in image by moving from the exterior perimeter to the interior and sorting the perimeter as though it was one singular line
        /// </summary>
        /// <param name="image">The image to be sorted</param>
        /// <param name="selectedSort">The selected sorting method</param>
        /// <param name="lower">The lower bound (used for brightness sorting)</param>
        /// <param name="upper">The upper bound (used for brightness sorting)</param>
        /// <param name="color">The selected color (used for RGB sorting)</param>
        /// <returns>Returns a sorted image</returns>
        private Bitmap SortBySpiral(Bitmap image, SortingMethodsEnum selectedSort, double lower, double upper, RGBEnum color)
        {
            //Sets the intial boundaries for the min and max values of Width and Height
            int endWidth = image.Width - 1;
            int endHeight = image.Height - 1;
            int startWidth = 0;
            int startHeight = 0;

            // Iteratively moves in from the exterior to the center and sorts the collected pixels from the perimeter
            while (endWidth >= startWidth && endHeight >= startHeight)
            {
                // Clear pixels and add the pixels from the perimeter to it
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

                // Sort pixels with respect to bounds by Brightness
                if (selectedSort == SortingMethodsEnum.Brightness)
                {
                    pixels = BrightSortWithBounds(pixels, lower, upper);
                }

                // Sort pixels by Hue
                if (selectedSort == SortingMethodsEnum.Hue)
                {
                    pixels = HueSortWithBounds(pixels, lower, upper);
                }

                // Sort pixels by Saturation
                if (selectedSort == SortingMethodsEnum.Saturation)
                {
                    pixels = SaturationSortWithBounds(pixels, lower, upper);
                }

                // Sort pixels with respect to RGB
                if (selectedSort == SortingMethodsEnum.RGB)
                {
                    pixels = RGBSortWithBounds(pixels, lower, upper, color);
                }

                // Put the pixels that were sorted back into the perimeter of the image
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

            // image is returned
            return image;
        }

        private List<Color> HueSortWithBounds(List<Color> pixelList, double lower, double upper)
        {
            if (upper < lower)
            {
                double mid = upper;
                upper = lower;
                lower = mid;
            }
            if (upper > 360.0)
            {
                upper = 360.0;
            }
            if (lower < 0.0)
            {
                lower = 0.0;
            }

            int a = 0;
            bool first = false;
            int b = 0;

            for (int j = 0; j < pixelList.Count; ++j)
            {
                float hue = pixelList[j].GetHue();
                if (hue <= upper && hue >= lower)
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
                        temp = new List<Color>(pixelList.GetRange(a, b - a + 1));
                        temp.Sort(new HueSort());
                        for (int c = 0; c < temp.Count; ++c)
                        {
                            pixelList[a] = temp[c];
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
                temp = new List<Color>(pixelList.GetRange(a, b - a + 1));
                temp.Sort(new HueSort());
                int d = b - a;
                for (int c = 0; c < d; ++c)
                {
                    pixelList[a] = temp[c];
                    ++a;
                }
            }

            return pixelList;
        }

        /// <summary>
        /// Sorts the passed in list by brightness with respect to boundaries
        /// </summary>
        /// <param name="pixelList">List of pixels to be sorted</param>
        /// <param name="lower">Lower boundary</param>
        /// <param name="upper">Upper boundary</param>
        /// <returns>Sorted list of pixels</returns>
        private List<Color> BrightSortWithBounds(List<Color> pixelList, double lower, double upper)
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

            for (int j = 0; j < pixelList.Count; ++j)
            {
                float bright = pixelList[j].GetBrightness();
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
                        temp = new List<Color>(pixelList.GetRange(a, b - a + 1));
                        temp.Sort(new BrightSort());
                        for (int c = 0; c < temp.Count; ++c)
                        {
                            pixelList[a] = temp[c];
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
                temp = new List<Color>(pixelList.GetRange(a, b - a + 1));
                temp.Sort(new BrightSort());
                int d = b - a;
                for (int c = 0; c < d; ++c)
                {
                    pixelList[a] = temp[c];
                    ++a;
                }
            }

            return pixelList;
        }

        private List<Color> RGBSortWithBounds(List<Color> pixelList, double lower, double upper, RGBEnum sortBy)
        {
            if (upper < lower)
            {
                double mid = upper;
                upper = lower;
                lower = mid;
            }
            if (upper > 255.0)
            {
                upper = 255.0;
            }
            if (lower < 0.0)
            {
                lower = 0.0;
            }

            int a = 0;
            bool first = false;
            int b = 0;

            if (sortBy == RGBEnum.Blue)
            {
                for (int j = 0; j < pixelList.Count; ++j)
                {
                    double color = pixelList[j].B;
                    if (color <= upper && color >= lower)
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
                            temp = new List<Color>(pixelList.GetRange(a, b - a + 1));
                            temp.Sort(new BlueSort());
                            for (int c = 0; c < temp.Count; ++c)
                            {
                                pixelList[a] = temp[c];
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
                    temp = new List<Color>(pixelList.GetRange(a, b - a + 1));
                    temp.Sort(new BlueSort());
                    int d = b - a;
                    for (int c = 0; c < d; ++c)
                    {
                        pixelList[a] = temp[c];
                        ++a;
                    }
                }
            } 
            else if (sortBy == RGBEnum.Green)
            {
                for (int j = 0; j < pixelList.Count; ++j)
                {
                    double color = pixelList[j].G;
                    if (color <= upper && color >= lower)
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
                            temp = new List<Color>(pixelList.GetRange(a, b - a + 1));
                            temp.Sort(new GreenSort());
                            for (int c = 0; c < temp.Count; ++c)
                            {
                                pixelList[a] = temp[c];
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
                    temp = new List<Color>(pixelList.GetRange(a, b - a + 1));
                    temp.Sort(new GreenSort());
                    int d = b - a;
                    for (int c = 0; c < d; ++c)
                    {
                        pixelList[a] = temp[c];
                        ++a;
                    }
                }
            } 
            else
            {
                for (int j = 0; j < pixelList.Count; ++j)
                {
                    double color = pixelList[j].R;
                    if (color <= upper && color >= lower)
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
                            temp = new List<Color>(pixelList.GetRange(a, b - a + 1));
                            temp.Sort(new RedSort());
                            for (int c = 0; c < temp.Count; ++c)
                            {
                                pixelList[a] = temp[c];
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
                    temp = new List<Color>(pixelList.GetRange(a, b - a + 1));
                    temp.Sort(new RedSort());
                    int d = b - a;
                    for (int c = 0; c < d; ++c)
                    {
                        pixelList[a] = temp[c];
                        ++a;
                    }
                }
            }

            return pixelList;
        }

        /// <summary>
        /// Puts the extended list of pixels back into an image of the appropriate size
        /// </summary>
        /// <param name="image">Extended image</param>
        /// <param name="origImage">Original image (used for sizing)</param>
        /// <returns>Returns an image of the correct size</returns>
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
    }
}