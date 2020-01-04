using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelSort.Model
{
    class SimpleSort
    {
        Color[,] _toSort = new Color[0,0];
        public SimpleSort(Color[,] toSort)
        {
            _toSort = toSort;
        }

        public Image Sort()
        {
            if (_toSort == null)
            {
                return null;
            }
            return ConvertToBitmap(_toSort);
        }

        public Image ConvertToBitmap(Color[,] toConvert)
        {

            Bitmap bmp = new Bitmap(toConvert.GetLength(0), toConvert.GetLength(1));
            for (int i = 0; i < toConvert.GetLength(0); i++)
            {
                for (int j = 0; j < toConvert.GetLength(1); j++)
                {
                    bmp.SetPixel(i, j, Color.FromArgb(toConvert[i, j].ToArgb()));
                }
            }
            Image toReturn = bmp;
            return toReturn;
        }
    }
}
