using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace V_sem___GK___projekt3
{
    class Helpers
    {
        private static void AmendCoords(ref int x, ref int y, int xMax, int yMax)
        {
            x = x < 0 ? 0 : x >= xMax ? xMax - 1 : x;
            y = y < 0 ? 0 : y >= yMax ? yMax - 1 : y;
        }

        public static void CheckAndAmendChannels(ref int r, ref int g, ref int b)
        {
            r = r > 255 ? 255 : r < 0 ? 0 : r;
            g = g > 255 ? 255 : g < 0 ? 0 : g;
            b = b > 255 ? 255 : b < 0 ? 0 : b;
        }

        public static double[,] GetAveragingMatrix()
        {
            //return new double[,]
            //{
            //    { 1.0/9.0, 1.0/9.0, 1.0/9.0 },
            //    { 1.0/9.0, 1.0/9.0, 1.0/9.0 },
            //    { 1.0/9.0, 1.0/9.0, 1.0/9.0 }
            //};
            return new double[,]
            {
                { 1.0/16.0, 2.0/16.0, 1.0/16.0 },
                { 2.0/16.0, 4.0/16.0, 2.0/16.0 },
                { 1.0/16.0, 2.0/16.0, 1.0/16.0 }
            };
        }

        public static double[,] GetGaussianFilterMatrix(double b)
        {
            return NormalizeMatrix(new double[,]
            {
                { 1.0, b  , 1.0 },
                { b  , b*b, b   },
                { 1.0, b  , 1.0 }
            });
        }

        public static double[,] GetLaplacianFilterMatrix()
        {
            return new double[,]
            {
                {  0.0, -1.0,  0.0 },
                { -1.0,  4.0, -1.0 },
                {  0.0, -1.0,  0.0 }
            };
        }

        public static double[,] GetLaplacianFilterMatrixDerivativesOnDiag()
        {
            return new double[,]
            {
                { -1.0, -1.0, -1.0 },
                { -1.0,  8.0, -1.0 },
                { -1.0, -1.0, -1.0 }
            };
        }

        public static double[,] GetLaplacianFilterMatrixThreeParallelLines()
        {
            return new double[,]
            {
                { -2.0,  1.0, -2.0 },
                {  1.0,  4.0,  1.0 },
                { -2.0,  1.0, -2.0 }
            };
        }

        public static double[,] GetRobertsCrossX()
        {
            return new double[,]
            {
                { 0.0,  0.0,  0.0 },
                { 0.0,  1.0,  0.0 },
                { 0.0,  0.0, -1.0 }
            };
        }

        public static double[,] GetRobertsCrossY()
        {
            return new double[,]
            {
                { 0.0,  0.0,  0.0 },
                { 0.0,  0.0,  1.0 },
                { 0.0, -1.0,  0.0 }
            };
        }

        /// <summary>
        /// Returns pixel colors for RGB channels transformed by a filter matrix
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name=""></param>
        /// <returns>Array: [R, G, B]</returns>
        public static int[] PerformFilterOnPixelColor(int x, int y, Color[,] PictureColors, double[,] FilterMatrix)
        {
            FilterMatrix = NormalizeMatrix(FilterMatrix);
            double red = 0.0, green = 0.0, blue = 0.0;
            int matrixSize = FilterMatrix.GetLength(0);
            int matrixHalf = matrixSize / 2;
            for (int i = x - matrixHalf, ii = 0; i <= x + matrixHalf; ++i, ++ii)
            {
                for (int j = y - matrixHalf, jj = 0; j <= y + matrixHalf; j++, ++jj)
                {
                    int iTemp = i, jTemp = j;
                    AmendCoords(ref iTemp, ref jTemp, PictureColors.GetLength(0), PictureColors.GetLength(1));
                    red   += PictureColors[iTemp, jTemp].R * FilterMatrix[ii, jj];
                    green += PictureColors[iTemp, jTemp].G * FilterMatrix[ii, jj];
                    blue  += PictureColors[iTemp, jTemp].B * FilterMatrix[ii, jj];
                }
            }

            return new int[] { (int)red, (int)green, (int)blue };
        }

        //public static double[,] Calculate1DSampleKernel(double deviation, int size)
        //{
        //    double[,] ret = new double[size, 1];
        //    double sum = 0;
        //    int half = size / 2;
        //    for (int i = 0; i < size; i++)
        //    {
        //        ret[i, 0] = 1 / (Math.Sqrt(2 * Math.PI) * deviation) * Math.Exp(-(i - half) * (i - half) / (2 * deviation * deviation));
        //        sum += ret[i, 0];
        //    }
        //    return ret;
        //}
        //public static double[,] Calculate1DSampleKernel(double deviation)
        //{
        //    int size = (int)Math.Ceiling(deviation * 3) * 2 + 1;
        //    return Calculate1DSampleKernel(deviation, size);
        //}
        //public static double[,] CalculateNormalized1DSampleKernel(double deviation)
        //{
        //    return NormalizeMatrix(Calculate1DSampleKernel(deviation));
        //}
        public static double[,] NormalizeMatrix(double[,] matrix)
        {
            double[,] ret = new double[matrix.GetLength(0), matrix.GetLength(1)];
            double sum = 0;
            double lowest = matrix.Cast<double>().ToArray().Min();
            double greatest = matrix.Cast<double>().ToArray().Max();

            if (lowest < 0)
            {
                //for (int i = 0; i < ret.GetLength(0); i++)
                //{
                //    for (int j = 0; j < ret.GetLength(1); j++)
                //    {
                //        ret[i, j] = (matrix[i, j] - lowest) / (greatest - lowest);
                //    }
                //}
                ret = matrix;
            }
            else
            {
                for (int i = 0; i < ret.GetLength(0); i++)
                {
                    for (int j = 0; j < ret.GetLength(1); j++)
                    {
                        sum += matrix[i, j];
                        if (matrix[i, j] < lowest)
                            lowest = matrix[i, j];
                    }
                }
                if (sum != 0)
                {
                    for (int i = 0; i < ret.GetLength(0); i++)
                    {
                        for (int j = 0; j < ret.GetLength(1); j++)
                        {
                            ret[i, j] = matrix[i, j] / sum;
                        }
                    }
                }
            }
            
            return ret;
        }

        //public static double[,] GaussianConvolution(double[,] matrix, double deviation)
        //{
        //    double[,] kernel = CalculateNormalized1DSampleKernel(deviation);
        //    double[,] res1 = new double[matrix.GetLength(0), matrix.GetLength(1)];
        //    double[,] res2 = new double[matrix.GetLength(0), matrix.GetLength(1)];
        //    //x-direction
        //    for (int i = 0; i < matrix.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < matrix.GetLength(1); j++)
        //            res1[i, j] = processPoint(matrix, i, j, kernel, 0);
        //    }
        //    //y-direction
        //    for (int i = 0; i < matrix.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < matrix.GetLength(1); j++)
        //            res2[i, j] = processPoint(res1, i, j, kernel, 1);
        //    }
        //    return res2;
        //}
        //private static double processPoint(double[,] matrix, int x, int y, double[,] kernel, int direction)
        //{
        //    double res = 0;
        //    int half = kernel.GetLength(0) / 2;
        //    for (int i = 0; i < kernel.GetLength(0); i++)
        //    {
        //        int cox = direction == 0 ? x + i - half : x;
        //        int coy = direction == 1 ? y + i - half : y;
        //        if (cox >= 0 && cox < matrix.GetLength(0) && coy >= 0 && coy < matrix.GetLength(1))
        //        {
        //            res += matrix[cox, coy] * kernel[i, 0];
        //        }
        //    }
        //    return res;
        //}
    }
}

//public class GaussianFilter : Filter
//{
//    Filter inputFilter;
//    double deviation;

//    public GaussianFilter(Color[,] pictureColors, double deviation, Filter filter = null) : base(pictureColors)
//    {
//        inputFilter = filter;
//        this.deviation = deviation;
//    }

//    public override Bitmap ApplyFilter(Bitmap inputBitmap, FilterCanalArgs args, Point brushCenter = default(Point), int brushRadius = 0, bool[,] alreadyFilteredPixels = null)
//    {
//        Bitmap outputBitmap = inputBitmap;
//        if (inputFilter != null)
//            outputBitmap = inputFilter.ApplyFilter(inputBitmap, args, brushCenter, brushRadius);
//        int bitmapWidth = inputBitmap.Width, bitmapHeight = inputBitmap.Height;

//        double[,] redMatrix = new double[bitmapWidth, bitmapHeight];
//        double[,] greenMatrix = new double[bitmapWidth, bitmapHeight];
//        double[,] blueMatrix = new double[bitmapWidth, bitmapHeight];

//        for (int i = 0; i < bitmapWidth; i++)
//        {
//            for (int j = 0; j < bitmapHeight; j++)
//            {
//                redMatrix[i, j] = base.PictureColors[i, j].R;
//                greenMatrix[i, j] = base.PictureColors[i, j].G;
//                blueMatrix[i, j] = base.PictureColors[i, j].B;
//            }
//        }

//        redMatrix = Helpers.GaussianConvolution(redMatrix, deviation);
//        greenMatrix = Helpers.GaussianConvolution(greenMatrix, deviation);
//        blueMatrix = Helpers.GaussianConvolution(blueMatrix, deviation);

//        int forWidth, forHeight;
//        forWidth = (brushRadius == 0) ? bitmapWidth : brushRadius + brushCenter.X;
//        forHeight = (brushRadius == 0) ? bitmapHeight : brushRadius + brushCenter.Y;
//        for (int i = (brushRadius == 0) ? 0 : brushCenter.X - brushRadius; i < forWidth; i++) // if brush - iterate only inside rectangle containing that brush
//        {
//            for (int j = (brushRadius == 0) ? 0 : brushCenter.Y - brushRadius; j < forHeight; j++)
//            {
//                if (i < 0 || j < 0 || i >= bitmapWidth || j >= bitmapHeight) continue;
//                if (alreadyFilteredPixels == null || !alreadyFilteredPixels[i, j])
//                {
//                    if ((brushCenter == default(Point) && brushRadius == 0) || (CountDistance(brushCenter, new Point(i, j))) <= brushRadius)
//                    {
//                        int newRed = 0, newGreen = 0, newBlue = 0;
//                        if (args.Red)
//                        {
//                            newRed = (int)Math.Min(255, redMatrix[i, j]);
//                        }
//                        if (args.Green)
//                        {
//                            newGreen = (int)Math.Min(255, greenMatrix[i, j]);
//                        }
//                        if (args.Blue)
//                        {
//                            newBlue = (int)Math.Min(255, blueMatrix[i, j]);
//                        }

//                        Color newColor = Color.FromArgb(newRed, newGreen, newBlue);
//                        outputBitmap.SetPixel(i, j, newColor);
//                        if (alreadyFilteredPixels != null)
//                            alreadyFilteredPixels[i, j] = true;
//                    }
//                }
//            }
//        }

//        return outputBitmap;
//    }
//}