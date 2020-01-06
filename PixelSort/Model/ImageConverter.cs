using System;
using System.Drawing;
using System.IO;

namespace PixelSort.Model
{
    class ImageConverter
    {
        public string SavedImagePath { get; set; }
        public ImageConverter()
        {

        }

        public Bitmap ImageToBitmap(string image)
        {
            if (image.Length == 0)
            {
                return null;
            }
            Bitmap bitmapImg = new Bitmap(image);
            return bitmapImg;
        }

        public bool Save(Bitmap image)
        {
            if (image == null)
            {
                return false;
            }
            SavedImagePath = Path.GetTempPath() + @"\" + Guid.NewGuid() + ".png";
            image.Save(@SavedImagePath);
            return true;
        }

    }
}
