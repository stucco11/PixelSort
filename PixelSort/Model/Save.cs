using System;
using System.Drawing;
using System.IO;

namespace PixelSort.Model
{
    internal class Save
    {
        public Save()
        {
        }

        public string SavedImagePath { get; set; }
        public bool SaveImage(Bitmap image)
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