using System;
using System.Drawing;
using System.IO;

namespace PixelSort.Model
{
    internal class Save
    {
        // Default Constructor
        public Save()
        {
        }

        // Stored Path of the saved image
        public string SavedImagePath { get; set; }

        /*
         * When an image needs to be saved, SavedImagePath will be "the temp location \ a generated ID \ .png" 
         * to prevent JPG loss, however minimal that would be. This assumes image isn't null, which is checked for
         */
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