using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelSort.Model
{
    class ImageToColor
    {
        public ImageToColor()
        {

        }

        public Color[,] ImageToColorArray(string image)
        {
            if (image.Length == 0)
            {
                return null;
            }
            Bitmap bitmapImg = new Bitmap(image);
            Color[,] colorArray = new Color[bitmapImg.Width, bitmapImg.Height];
            for (int i = 0; i < bitmapImg.Width; i++)
            {
                for (int j = 0; j < bitmapImg.Height; j++)
                {
                    Color pixel = bitmapImg.GetPixel(i, j);

                    colorArray[i, j] = pixel;
                }
            }

            return colorArray;
        }
    }
}
