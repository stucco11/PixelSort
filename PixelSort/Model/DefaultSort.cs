using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PixelSort.Model
{
    class DefaultSort
    {
        private Image _image;
        private Image _sortedImage;
        public DefaultSort(Image image)
        {
            _image = image;
        }

        private Image SortedImage()
        {
            System.Drawing.Bitmap img = new System.Drawing.Bitmap(@_image.Source.ToString());
            System.Drawing.Color[,] bitmapArray = new System.Drawing.Color[img.Width,img.Height];
            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    System.Drawing.Color pixel = img.GetPixel(i, j);

                    float bright = pixel.GetBrightness();
                    if (bitmapArray.GetLength(i) == 0)
                    {
                        bitmapArray[i, j] = pixel;
                        continue;
                    }
                    for (int h = 0; h < bitmapArray.GetLength(i); h++)
                    {
                        
                    }
                }
            }

            return _sortedImage;
        }
    }
}
