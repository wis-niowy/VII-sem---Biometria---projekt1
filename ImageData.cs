using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace V_sem___GK___projekt3
{
    public class ImageData
    {
        private Color[,] PictureColors { get; set; }

        public int[] RedValues
        {
            get
            {
                int[] toReturn = new int[256];
                foreach (var pixel in PictureColors)
                {
                    toReturn[pixel.R]++;
                }
                return toReturn;
            }
        }
        public int[] GreenValues
        {
            get
            {
                int[] toReturn = new int[256];
                foreach (var pixel in PictureColors)
                {
                    toReturn[pixel.G]++;
                }
                return toReturn;
            }
        }
        public int[] BlueValues
        {
            get
            {
                int[] toReturn = new int[256];
                foreach (var pixel in PictureColors)
                {
                    toReturn[pixel.B]++;
                }
                return toReturn;
            }
        }
        public int[] LuminanceValues
        {
            get
            {
                int[,] convertedPixels = new int[PictureColors.GetLength(0), PictureColors.GetLength(1)];
                for (int i = 0; i < PictureColors.GetLength(0); ++i)
                    for (int j = 0; j < PictureColors.GetLength(1); ++j)
                    {
                        convertedPixels[i, j] = (int)(0.30 * PictureColors[i, j].R + 0.60 * PictureColors[i, j].G + 0.11 * PictureColors[i, j].B);
                    }
                int[] toReturn = new int[256];
                foreach (var pixel in convertedPixels)
                {
                    toReturn[pixel % 256]++;
                }
                return toReturn;
            }
        }

        public ImageData(Color[,] pC)
        {
            PictureColors = pC;
        }


    }
}
