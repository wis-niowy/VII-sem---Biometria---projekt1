using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace V_sem___GK___projekt3
{
    public abstract class Filter
    {
        protected Color[,] PictureColors { get; set; }

        public Filter(Color[,] pC)
        {
            PictureColors = pC;
        }

        public abstract Bitmap ApplyFilter(Bitmap inputBitmap, FilterCanalArgs args, Point brushCenter = default(Point), int brushRadius = 0, bool[,] alreadyFilteredPixels = null);
        public static double CountDistance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }
    }

    public class NegativeFilter: Filter
    {
        Filter inputFilter;

        public NegativeFilter(Color[,] pictureColors, Filter filter = null): base(pictureColors)
        {
            inputFilter = filter;
        }

        public override Bitmap ApplyFilter(Bitmap inputBitmap, FilterCanalArgs args, Point brushCenter = default(Point), int brushRadius = 0, bool[,] alreadyFilteredPixels = null)
        {
            Stopwatch sw = new Stopwatch();

            sw.Start();

            Bitmap outputBitmap = inputBitmap;
            if (inputFilter != null)
                outputBitmap = inputFilter.ApplyFilter(inputBitmap, args, brushCenter, brushRadius);
            int bitmapWidth = inputBitmap.Width, bitmapHeight = inputBitmap.Height;

            int forWidth, forHeight;
            forWidth = (brushRadius == 0) ? bitmapWidth : brushRadius + brushCenter.X;
            forHeight = (brushRadius == 0) ? bitmapHeight : brushRadius + brushCenter.Y;
            for (int i = (brushRadius == 0) ? 0 : brushCenter.X - brushRadius; i < forWidth; i++) // if brush - iterate only inside rectangle containing that brush
            {
                for (int j = (brushRadius == 0) ? 0 : brushCenter.Y - brushRadius; j < forHeight; j++)
                {
                    if (i < 0 || j < 0 || i >= bitmapWidth || j >= bitmapHeight) continue;
                    if (alreadyFilteredPixels == null || !alreadyFilteredPixels[i, j])
                    {
                        if ((brushCenter == default(Point) && brushRadius == 0) || (CountDistance(brushCenter, new Point(i, j))) <= brushRadius)
                        {
                            int newRed = args.Red ? 255 - base.PictureColors[i, j].R : base.PictureColors[i, j].R;
                            int newGrenn = args.Green ? 255 - base.PictureColors[i, j].G : base.PictureColors[i, j].G;
                            int newBlue = args.Blue ? 255 - base.PictureColors[i, j].B : base.PictureColors[i, j].B;

                            Color newColor = Color.FromArgb(newRed, newGrenn, newBlue);
                            outputBitmap.SetPixel(i, j, newColor);
                            if (alreadyFilteredPixels != null)
                                alreadyFilteredPixels[i, j] = true;
                        }
                    }
                }
            }

            //unsafe
            //{
            //    BitmapData bitmapData = outputBitmap.LockBits(new Rectangle(0, 0, outputBitmap.Width, outputBitmap.Height), ImageLockMode.ReadWrite, outputBitmap.PixelFormat);

            //    int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(outputBitmap.PixelFormat) / 8;
            //    int heightInPixels = (brushRadius == 0) ? bitmapHeight : brushRadius + brushCenter.Y;
            //    int widthInPixels = (brushRadius == 0) ? bitmapWidth : brushRadius + brushCenter.X;
            //    int widthInBytes = widthInPixels * bytesPerPixel;
            //    byte* PtrFirstPixel = (byte*)bitmapData.Scan0;

            //    Parallel.For((brushRadius == 0) ? 0 : brushCenter.Y - brushRadius, heightInPixels, y =>
            //    {
            //        byte* currentLine = PtrFirstPixel + (y * bitmapData.Stride);
            //        for (int x = (brushRadius == 0) ? 0 : brushCenter.X - brushRadius; x < widthInBytes; x = x + bytesPerPixel)
            //        {
            //            int i = x / bytesPerPixel;
            //            int j = y;
            //            if (alreadyFilteredPixels == null || !alreadyFilteredPixels[i, j])
            //            {
            //                if ((brushCenter == default(Point) && brushRadius == 0) || (CountDistance(brushCenter, new Point(i, j))) <= brushRadius)
            //                {
            //                    int oldBlue = currentLine[x];
            //                    int oldGreen = currentLine[x + 1];
            //                    int oldRed = currentLine[x + 2];

            //                    currentLine[x] = (byte)(255 - oldBlue);
            //                    currentLine[x + 1] = (byte)(255 - oldGreen);
            //                    currentLine[x + 2] = (byte)(255 - oldRed);

            //                    if (alreadyFilteredPixels != null)
            //                        alreadyFilteredPixels[i, j] = true;
            //                }
            //            }
            //        }
            //    });
            //    outputBitmap.UnlockBits(bitmapData);
            //}

            //BitmapData bitmapData = outputBitmap.LockBits(new Rectangle(0, 0, outputBitmap.Width, outputBitmap.Height), ImageLockMode.ReadWrite, outputBitmap.PixelFormat);

            //int bytesPerPixel = Bitmap.GetPixelFormatSize(outputBitmap.PixelFormat) / 8;
            //int byteCount = bitmapData.Stride * outputBitmap.Height;
            //byte[] pixels = new byte[byteCount];
            //IntPtr ptrFirstPixel = bitmapData.Scan0;
            //Marshal.Copy(ptrFirstPixel, pixels, 0, pixels.Length);
            //int heightInPixels = (brushRadius == 0) ? bitmapHeight : brushRadius + brushCenter.Y;
            //int widthInPixels = (brushRadius == 0) ? bitmapWidth : brushRadius + brushCenter.X;
            //int widthInBytes = widthInPixels * bytesPerPixel;

            //for (int y = (brushRadius == 0) ? 0 : brushCenter.Y - brushRadius; y < heightInPixels; y++)
            //{
            //    int currentLine = y * bitmapData.Stride;
            //    for (int x = (brushRadius == 0) ? 0 : brushCenter.X - brushRadius; x < widthInBytes; x = x + bytesPerPixel)
            //    {
            //        int i = x / bytesPerPixel;
            //        int j = y;
            //        if (i < 0 || j < 0 || i >= bitmapWidth || j >= bitmapHeight) continue;
            //        if (alreadyFilteredPixels == null || !alreadyFilteredPixels[i, j])
            //        {
            //            if ((brushCenter == default(Point) && brushRadius == 0) || CountDistance(brushCenter, new Point(i, j)) <= brushRadius)
            //            {
            //                int oldBlue = pixels[currentLine + x];
            //                int oldGreen = pixels[currentLine + x + 1];
            //                int oldRed = pixels[currentLine + x + 2];

            //                pixels[currentLine + x] = (byte)(255 - oldBlue);
            //                pixels[currentLine + x + 1] = (byte)(255 - oldGreen);
            //                pixels[currentLine + x + 2] = (byte)(255 - oldRed);

            //                if (alreadyFilteredPixels != null)
            //                    alreadyFilteredPixels[i, j] = true;
            //            }
            //        }
            //    }
            //}

            //// copy modified bytes back
            //Marshal.Copy(pixels, 0, ptrFirstPixel, pixels.Length);
            //outputBitmap.UnlockBits(bitmapData);

            sw.Stop();
            return outputBitmap;
        }

    }

    public class BrightnessFilter : Filter
    {
        Filter inputFilter;
        int delta;

        public BrightnessFilter(Color[,] pictureColors, int delta, Filter filter = null): base(pictureColors)
        {
            inputFilter = filter;
            this.delta = delta;
        }

        public override Bitmap ApplyFilter(Bitmap inputBitmap, FilterCanalArgs args, Point brushCenter = default(Point), int brushRadius = 0, bool[,] alreadyFilteredPixels = null)
        {
            Bitmap outputBitmap = inputBitmap;
            if (inputFilter != null)
                outputBitmap = inputFilter.ApplyFilter(inputBitmap, args, brushCenter, brushRadius);
            int bitmapWidth = inputBitmap.Width, bitmapHeight = inputBitmap.Height;

            int forWidth, forHeight;
            forWidth = (brushRadius == 0) ? bitmapWidth : brushRadius + brushCenter.X;
            forHeight = (brushRadius == 0) ? bitmapHeight : brushRadius + brushCenter.Y;
            for (int i = (brushRadius == 0) ? 0 : brushCenter.X - brushRadius; i < forWidth; i++) // if brush - iterate only inside rectangle containing that brush
            {
                for (int j = (brushRadius == 0) ? 0 : brushCenter.Y - brushRadius; j < forHeight; j++)
                {
                    if (i < 0 || j < 0 || i >= bitmapWidth || j >= bitmapHeight) continue;
                    if (alreadyFilteredPixels == null || !alreadyFilteredPixels[i, j])
                    {
                        if ((brushCenter == default(Point) && brushRadius == 0) || (CountDistance(brushCenter, new Point(i, j))) <= brushRadius)
                        {
                            int newRed = base.PictureColors[i, j].R;
                            int newGreen = base.PictureColors[i, j].G;
                            int newBlue = base.PictureColors[i, j].B;

                            if (args.Red)
                            {
                                newRed = base.PictureColors[i, j].R + delta;
                                newRed = newRed > 255 ? 255 : newRed < 0 ? 0 : newRed;
                            }
                            if (args.Green)
                            {
                                newGreen = base.PictureColors[i, j].G + delta;
                                newGreen = newGreen > 255 ? 255 : newGreen < 0 ? 0 : newGreen;
                            }
                            if (args.Blue)
                            {
                                newBlue = base.PictureColors[i, j].B + delta;
                                newBlue = newBlue > 255 ? 255 : newBlue < 0 ? 0 : newBlue;
                            }

                            Color newColor = Color.FromArgb(newRed, newGreen, newBlue);
                            outputBitmap.SetPixel(i, j, newColor);
                            if (alreadyFilteredPixels != null)
                                alreadyFilteredPixels[i, j] = true;
                        }
                    }
                }
            }

            //unsafe
            //{
            //    BitmapData bitmapData = outputBitmap.LockBits(new Rectangle(0, 0, outputBitmap.Width, outputBitmap.Height), ImageLockMode.ReadWrite, outputBitmap.PixelFormat);

            //    int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(outputBitmap.PixelFormat) / 8;
            //    int heightInPixels = (brushRadius == 0) ? bitmapHeight : brushRadius + brushCenter.Y;
            //    int widthInPixels = (brushRadius == 0) ? bitmapWidth : brushRadius + brushCenter.X;
            //    int widthInBytes = widthInPixels * bytesPerPixel;
            //    byte* PtrFirstPixel = (byte*)bitmapData.Scan0;

            //    Parallel.For((brushRadius == 0) ? 0 : brushCenter.Y - brushRadius, heightInPixels, y =>
            //    {
            //        byte* currentLine = PtrFirstPixel + (y * bitmapData.Stride);
            //        for (int x = (brushRadius == 0) ? 0 : brushCenter.X - brushRadius; x < widthInBytes; x = x + bytesPerPixel)
            //        {
            //            int i = x / bytesPerPixel;
            //            int j = y;
            //            if (alreadyFilteredPixels == null || !alreadyFilteredPixels[i, j])
            //            {
            //                if ((brushCenter == default(Point) && brushRadius == 0) || (CountDistance(brushCenter, new Point(i, j))) <= brushRadius)
            //                {
            //                    int oldBlue = currentLine[x] + delta;
            //                    oldBlue = oldBlue > 255 ? 255 : oldBlue < 0 ? 0 : oldBlue;
            //                    int oldGreen = currentLine[x + 1] + delta;
            //                    oldGreen = oldGreen > 255 ? 255 : oldGreen < 0 ? 0 : oldGreen;
            //                    int oldRed = currentLine[x + 2] + delta;
            //                    oldRed = oldRed > 255 ? 255 :oldRed < 0 ? 0 : oldRed;

            //                    currentLine[x] = (byte)(oldBlue);
            //                    currentLine[x + 1] = (byte)(oldGreen);
            //                    currentLine[x + 2] = (byte)(oldRed);

            //                    if (alreadyFilteredPixels != null)
            //                        alreadyFilteredPixels[i, j] = true;
            //                }
            //            }
            //        }
            //    });
            //    outputBitmap.UnlockBits(bitmapData);
            //}

            return outputBitmap;
        }

    }

    public class ContrastFilter : Filter
    {
        Filter inputFilter;
        int contrast;

        public ContrastFilter(Color[,] pictureColors, int contrast, Filter filter = null): base(pictureColors)
        {
            inputFilter = filter;
            this.contrast = contrast;
        }

        public override Bitmap ApplyFilter(Bitmap inputBitmap, FilterCanalArgs args, Point brushCenter = default(Point), int brushRadius = 0, bool[,] alreadyFilteredPixels = null)
        {
            Bitmap outputBitmap = inputBitmap;
            if (inputFilter != null)
                outputBitmap = inputFilter.ApplyFilter(inputBitmap, args, brushCenter, brushRadius);
            int bitmapWidth = inputBitmap.Width, bitmapHeight = inputBitmap.Height;
            int forWidth, forHeight;
            forWidth = (brushRadius == 0) ? bitmapWidth : brushRadius + brushCenter.X;
            forHeight = (brushRadius == 0) ? bitmapHeight : brushRadius + brushCenter.Y;
            for (int i = (brushRadius == 0) ? 0 : brushCenter.X - brushRadius; i < forWidth; i++) // if brush - iterate only inside rectangle containing that brush
            {
                for (int j = (brushRadius == 0) ? 0 : brushCenter.Y - brushRadius; j < forHeight; j++)
                {
                    if (i < 0 || j < 0 || i >= bitmapWidth || j >= bitmapHeight) continue;
                    if (alreadyFilteredPixels == null || !alreadyFilteredPixels[i, j])
                    {
                        if ((brushCenter == default(Point) && brushRadius == 0) || (CountDistance(brushCenter, new Point(i, j))) <= brushRadius)
                        {
                            int newRed = base.PictureColors[i, j].R;
                            int newGreen = base.PictureColors[i, j].G;
                            int newBlue = base.PictureColors[i, j].B;

                            if (args.Red)
                            {
                                newRed = base.PictureColors[i, j].R - 128;
                                newRed *= contrast;
                                newRed += 128;
                                newRed = newRed > 255 ? 255 : newRed < 0 ? 0 : newRed;
                            }
                            if (args.Green)
                            {
                                newGreen = base.PictureColors[i, j].G - 128;
                                newGreen *= contrast;
                                newGreen += 128;
                                newGreen = newGreen > 255 ? 255 : newGreen < 0 ? 0 : newGreen;
                            }
                            if (args.Blue)
                            {
                                newBlue = base.PictureColors[i, j].B - 128;
                                newBlue *= contrast;
                                newBlue += 128;
                                newBlue = newBlue > 255 ? 255 : newBlue < 0 ? 0 : newBlue;
                            }

                            Color newColor = Color.FromArgb(newRed, newGreen, newBlue);
                            outputBitmap.SetPixel(i, j, newColor);
                            if (alreadyFilteredPixels != null)
                                alreadyFilteredPixels[i, j] = true;
                        }
                    }
                }
            }

            return outputBitmap;
        }

    }

    public class GammaFilter : Filter
    {
        Filter inputFilter;
        double gamma;

        public GammaFilter(Color[,] pictureColors, Filter filter = null) : base(pictureColors)
        {
            inputFilter = filter;
            this.gamma = 2.2;
        }

        public override Bitmap ApplyFilter(Bitmap inputBitmap, FilterCanalArgs args, Point brushCenter = default(Point), int brushRadius = 0, bool[,] alreadyFilteredPixels = null)
        {
            Bitmap outputBitmap = inputBitmap;
            if (inputFilter != null)
                outputBitmap = inputFilter.ApplyFilter(inputBitmap, args, brushCenter, brushRadius);
            int bitmapWidth = inputBitmap.Width, bitmapHeight = inputBitmap.Height;

            int forWidth, forHeight;
            forWidth = (brushRadius == 0) ? bitmapWidth : brushRadius + brushCenter.X;
            forHeight = (brushRadius == 0) ? bitmapHeight : brushRadius + brushCenter.Y;
            for (int i = (brushRadius == 0) ? 0 : brushCenter.X - brushRadius; i < forWidth; i++) // if brush - iterate only inside rectangle containing that brush
            {
                for (int j = (brushRadius == 0) ? 0 : brushCenter.Y - brushRadius; j < forHeight; j++)
                {
                    if (i < 0 || j < 0 || i >= bitmapWidth || j >= bitmapHeight) continue;
                    if (alreadyFilteredPixels == null || !alreadyFilteredPixels[i, j])
                    {
                        if ((brushCenter == default(Point) && brushRadius == 0) || (CountDistance(brushCenter, new Point(i, j))) <= brushRadius)
                        {
                            int newRed = base.PictureColors[i, j].R;
                            int newGreen = base.PictureColors[i, j].G;
                            int newBlue = base.PictureColors[i, j].B;

                            if (args.Red)
                            {
                                newRed = (int)(255 * Math.Pow((double)base.PictureColors[i, j].R / 255.0, 1.0 / gamma));
                                newRed = newRed > 255 ? 255 : newRed < 0 ? 0 : newRed;
                            }
                            if (args.Green)
                            {
                                newGreen = (int)(255 * Math.Pow((double)base.PictureColors[i, j].G / 255.0, 1.0 / gamma));
                                newGreen = newGreen > 255 ? 255 : newGreen < 0 ? 0 : newGreen;
                            }
                            if (args.Blue)
                            {
                                newBlue = (int)(255 * Math.Pow((double)base.PictureColors[i, j].B / 255.0, 1.0 / gamma));
                                newBlue = newBlue > 255 ? 255 : newBlue < 0 ? 0 : newBlue;
                            }
                            
                            Color newColor = Color.FromArgb(newRed, newGreen, newBlue);
                            outputBitmap.SetPixel(i, j, newColor);
                            if (alreadyFilteredPixels != null)
                                alreadyFilteredPixels[i, j] = true;
                        }
                    }
                }
            }

            //unsafe
            //{
            //    BitmapData bitmapData = outputBitmap.LockBits(new Rectangle(0, 0, outputBitmap.Width, outputBitmap.Height), ImageLockMode.ReadWrite, outputBitmap.PixelFormat);

            //    int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(outputBitmap.PixelFormat) / 8;
            //    int heightInPixels = (brushRadius == 0) ? bitmapHeight : brushRadius + brushCenter.Y;
            //    int widthInPixels = (brushRadius == 0) ? bitmapWidth : brushRadius + brushCenter.X;
            //    int widthInBytes = widthInPixels * bytesPerPixel;
            //    byte* PtrFirstPixel = (byte*)bitmapData.Scan0;

            //    Parallel.For((brushRadius == 0) ? 0 : brushCenter.Y - brushRadius, heightInPixels, y =>
            //    {
            //        byte* currentLine = PtrFirstPixel + (y * bitmapData.Stride);
            //        for (int x = (brushRadius == 0) ? 0 : brushCenter.X - brushRadius; x < widthInBytes; x = x + bytesPerPixel)
            //        {
            //            int i = x / bytesPerPixel;
            //            int j = y;
            //            if (alreadyFilteredPixels == null || !alreadyFilteredPixels[i, j])
            //            {
            //                if ((brushCenter == default(Point) && brushRadius == 0) || (CountDistance(brushCenter, new Point(i, j))) <= brushRadius)
            //                {
            //                    int oldBlue = (int)(255 * Math.Pow((double)currentLine[x] / 255.0, 1.0 / gamma));
            //                    int oldGreen = (int)(255 * Math.Pow((double)currentLine[x + 1] / 255.0, 1.0 / gamma));
            //                    int oldRed = (int)(255 * Math.Pow((double)currentLine[x + 2] / 255.0, 1.0 / gamma));

            //                    oldBlue = oldBlue > 255 ? 255 : oldBlue < 0 ? 0 : oldBlue;
            //                    oldGreen = oldGreen > 255 ? 255 : oldGreen < 0 ? 0 : oldGreen;
            //                    oldRed = oldRed > 255 ? 255 : oldRed < 0 ? 0 : oldRed;

            //                    currentLine[x] = (byte)(oldBlue);
            //                    currentLine[x + 1] = (byte)(oldGreen);
            //                    currentLine[x + 2] = (byte)(oldRed);

            //                    if (alreadyFilteredPixels != null)
            //                        alreadyFilteredPixels[i, j] = true;
            //                }
            //            }
            //        }
            //    });
            //    outputBitmap.UnlockBits(bitmapData);
            //}

            return outputBitmap;
        }

    }

    public class GreyFilter : Filter
    {
        Filter inputFilter;

        public GreyFilter(Color[,] pictureColors, Filter filter = null) : base(pictureColors)
        {
            inputFilter = filter;
        }

        public override Bitmap ApplyFilter(Bitmap inputBitmap, FilterCanalArgs args, Point brushCenter = default(Point), int brushRadius = 0, bool[,] alreadyFilteredPixels = null)
        {
            Bitmap outputBitmap = inputBitmap;
            if (inputFilter != null)
                outputBitmap = inputFilter.ApplyFilter(inputBitmap, args, brushCenter, brushRadius);
            int bitmapWidth = inputBitmap.Width, bitmapHeight = inputBitmap.Height;

            int forWidth, forHeight;
            forWidth = (brushRadius == 0) ? bitmapWidth : brushRadius + brushCenter.X;
            forHeight = (brushRadius == 0) ? bitmapHeight : brushRadius + brushCenter.Y;
            for (int i = (brushRadius == 0) ? 0 : brushCenter.X - brushRadius; i < forWidth; i++) // if brush - iterate only inside rectangle containing that brush
            {
                for (int j = (brushRadius == 0) ? 0 : brushCenter.Y - brushRadius; j < forHeight; j++)
                {
                    if (i < 0 || j < 0 || i >= bitmapWidth || j >= bitmapHeight) continue;
                    if (alreadyFilteredPixels == null || !alreadyFilteredPixels[i, j])
                    {
                        if ((brushCenter == default(Point) && brushRadius == 0) || (CountDistance(brushCenter, new Point(i, j))) <= brushRadius)
                        {
                            int newRed = base.PictureColors[i, j].R;
                            int newGreen = base.PictureColors[i, j].G;
                            int newBlue = base.PictureColors[i, j].B;

                            int gray = (int)(0.299 * newRed + 0.587 * newGreen + 0.114 * newBlue);

                            if (args.Red)
                            {
                                newRed = gray;
                                newRed = newRed > 255 ? 255 : newRed < 0 ? 0 : newRed;
                            }
                            if (args.Green)
                            {
                                newGreen = gray;
                                newGreen = newGreen > 255 ? 255 : newGreen < 0 ? 0 : newGreen;
                            }
                            if (args.Blue)
                            {
                                newBlue = gray;
                                newBlue = newBlue > 255 ? 255 : newBlue < 0 ? 0 : newBlue;
                            }

                            Color newColor = Color.FromArgb(newRed, newGreen, newBlue);
                            outputBitmap.SetPixel(i, j, newColor);
                            if (alreadyFilteredPixels != null)
                                alreadyFilteredPixels[i, j] = true;
                        }
                    }
                }
            }

            //unsafe
            //{
            //    BitmapData bitmapData = outputBitmap.LockBits(new Rectangle(0, 0, outputBitmap.Width, outputBitmap.Height), ImageLockMode.ReadWrite, outputBitmap.PixelFormat);

            //    int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(outputBitmap.PixelFormat) / 8;
            //    int heightInPixels = (brushRadius == 0) ? bitmapHeight : brushRadius + brushCenter.Y;
            //    int widthInPixels = (brushRadius == 0) ? bitmapWidth : brushRadius + brushCenter.X;
            //    int widthInBytes = widthInPixels * bytesPerPixel;
            //    byte* PtrFirstPixel = (byte*)bitmapData.Scan0;

            //    Parallel.For((brushRadius == 0) ? 0 : brushCenter.Y - brushRadius, heightInPixels, y =>
            //    {
            //        byte* currentLine = PtrFirstPixel + (y * bitmapData.Stride);
            //        for (int x = (brushRadius == 0) ? 0 : brushCenter.X - brushRadius; x < widthInBytes; x = x + bytesPerPixel)
            //        {
            //            int i = x / bytesPerPixel;
            //            int j = y;
            //            if (alreadyFilteredPixels == null || !alreadyFilteredPixels[i, j])
            //            {
            //                if ((brushCenter == default(Point) && brushRadius == 0) || (CountDistance(brushCenter, new Point(i, j))) <= brushRadius)
            //                {
            //                    int oldBlue = currentLine[x];
            //                    int oldGreen = currentLine[x + 1];
            //                    int oldRed = currentLine[x + 2];

            //                    int gray = (int)(0.299 * oldRed + 0.587 * oldGreen + 0.114 * oldBlue);

            //                    oldRed = oldGreen = oldBlue = gray;

            //                    oldBlue = oldBlue > 255 ? 255 : oldBlue < 0 ? 0 : oldBlue;
            //                    oldGreen = oldGreen > 255 ? 255 : oldGreen < 0 ? 0 : oldGreen;
            //                    oldRed = oldRed > 255 ? 255 : oldRed < 0 ? 0 : oldRed;

            //                    currentLine[x] = (byte)(oldBlue);
            //                    currentLine[x + 1] = (byte)(oldGreen);
            //                    currentLine[x + 2] = (byte)(oldRed);

            //                    if (alreadyFilteredPixels != null)
            //                        alreadyFilteredPixels[i, j] = true;
            //                }
            //            }
            //        }
            //    });
            //    outputBitmap.UnlockBits(bitmapData);
            //}

            return outputBitmap;
        }

    }


    public class ThresholdFilter : Filter
    {
        Filter inputFilter;
        int threshold;

        public ThresholdFilter(Color[,] pictureColors, int threshold, Filter filter = null) : base(pictureColors)
        {
            inputFilter = filter;
            this.threshold = threshold;
        }

        public override Bitmap ApplyFilter(Bitmap inputBitmap, FilterCanalArgs args, Point brushCenter = default(Point), int brushRadius = 0, bool[,] alreadyFilteredPixels = null)
        {
            Bitmap outputBitmap = inputBitmap;
            if (inputFilter != null)
                outputBitmap = inputFilter.ApplyFilter(inputBitmap, args, brushCenter, brushRadius);
            int bitmapWidth = inputBitmap.Width, bitmapHeight = inputBitmap.Height;

            int forWidth, forHeight;
            forWidth = (brushRadius == 0) ? bitmapWidth : brushRadius + brushCenter.X;
            forHeight = (brushRadius == 0) ? bitmapHeight : brushRadius + brushCenter.Y;
            for (int i = (brushRadius == 0) ? 0 : brushCenter.X - brushRadius; i < forWidth; i++) // if brush - iterate only inside rectangle containing that brush
            {
                for (int j = (brushRadius == 0) ? 0 : brushCenter.Y - brushRadius; j < forHeight; j++)
                {
                    if (i < 0 || j < 0 || i >= bitmapWidth || j >= bitmapHeight) continue;
                    if (alreadyFilteredPixels == null || !alreadyFilteredPixels[i, j])
                    {
                        if ((brushCenter == default(Point) && brushRadius == 0) || (CountDistance(brushCenter, new Point(i, j))) <= brushRadius)
                        {
                            int newRed = base.PictureColors[i, j].R;
                            int newGreen = base.PictureColors[i, j].G;
                            int newBlue = base.PictureColors[i, j].B;

                            int gray = (int)(0.299 * newRed + 0.587 * newGreen + 0.114 * newBlue);

                            if (args.Red)
                            {
                                newRed = gray;
                                newRed = newRed > threshold ? 255 : 0;
                            }
                            if (args.Green)
                            {
                                newGreen = gray;
                                newGreen = newGreen > threshold ? 255 : 0;
                            }
                            if (args.Blue)
                            {
                                newBlue = gray;
                                newBlue = newBlue > threshold ? 255 : 0;
                            }

                            Color newColor = Color.FromArgb(newRed, newGreen, newBlue);
                            outputBitmap.SetPixel(i, j, newColor);
                            if (alreadyFilteredPixels != null)
                                alreadyFilteredPixels[i, j] = true;
                        }
                    }
                }
            }

            return outputBitmap;
        }
    }

    public class NormalizeHistogramFilter : Filter
    {
        Filter inputFilter;

        public NormalizeHistogramFilter(Color[,] pictureColors, Filter filter = null) : base(pictureColors)
        {
            inputFilter = filter;
        }

        public override Bitmap ApplyFilter(Bitmap inputBitmap, FilterCanalArgs args, Point brushCenter = default(Point), int brushRadius = 0, bool[,] alreadyFilteredPixels = null)
        {
            Bitmap outputBitmap = inputBitmap;
            if (inputFilter != null)
                outputBitmap = inputFilter.ApplyFilter(inputBitmap, args, brushCenter, brushRadius);
            int bitmapWidth = inputBitmap.Width, bitmapHeight = inputBitmap.Height;

            var tempArray = PictureColors.Cast<Color>().ToArray();

            int redMin = tempArray.Min(e => e.R);
            int redMax = tempArray.Max(e => e.R);
            double redFraction = 255 / (double)(redMax - redMin);
            int greenMin = tempArray.Min(e => e.G);
            int greenMax = tempArray.Max(e => e.G);
            double greenFraction = 255 / (double)(greenMax - greenMin);
            int blueMin = tempArray.Min(e => e.B);
            int blueMax = tempArray.Max(e => e.B);
            double blueFraction = 255 / (double)(blueMax - blueMin);

            int forWidth, forHeight;
            forWidth = (brushRadius == 0) ? bitmapWidth : brushRadius + brushCenter.X;
            forHeight = (brushRadius == 0) ? bitmapHeight : brushRadius + brushCenter.Y;
            for (int i = (brushRadius == 0) ? 0 : brushCenter.X - brushRadius; i < forWidth; i++) // if brush - iterate only inside rectangle containing that brush
            {
                for (int j = (brushRadius == 0) ? 0 : brushCenter.Y - brushRadius; j < forHeight; j++)
                {
                    if (i < 0 || j < 0 || i >= bitmapWidth || j >= bitmapHeight) continue;
                    if (alreadyFilteredPixels == null || !alreadyFilteredPixels[i, j])
                    {
                        if ((brushCenter == default(Point) && brushRadius == 0) || (CountDistance(brushCenter, new Point(i, j))) <= brushRadius)
                        {
                            int newRed = base.PictureColors[i, j].R;
                            int newGreen = base.PictureColors[i, j].G;
                            int newBlue = base.PictureColors[i, j].B;

                            if (args.Red)
                            {
                                newRed = (int)(redFraction * (newRed - redMin));
                            }
                            if (args.Green)
                            {
                                newGreen = (int)(greenFraction * (newGreen - greenMin));
                            }
                            if (args.Blue)
                            {
                                newBlue = (int)(blueFraction * (newBlue - blueMin));
                            }

                            Color newColor = Color.FromArgb(newRed, newGreen, newBlue);
                            outputBitmap.SetPixel(i, j, newColor);
                            if (alreadyFilteredPixels != null)
                                alreadyFilteredPixels[i, j] = true;
                        }
                    }
                }
            }

            return outputBitmap;
        }
    }

    public class EqualizeHistogramFilter : Filter
    {
        Filter inputFilter;

        public EqualizeHistogramFilter(Color[,] pictureColors, Filter filter = null) : base(pictureColors)
        {
            inputFilter = filter;
        }

        public override Bitmap ApplyFilter(Bitmap inputBitmap, FilterCanalArgs args, Point brushCenter = default(Point), int brushRadius = 0, bool[,] alreadyFilteredPixels = null)
        {
            Bitmap outputBitmap = inputBitmap;
            if (inputFilter != null)
                outputBitmap = inputFilter.ApplyFilter(inputBitmap, args, brushCenter, brushRadius);
            int bitmapWidth = inputBitmap.Width, bitmapHeight = inputBitmap.Height;

            //var tempArray = PictureColors.Cast<Color>().ToArray();

            ImageData statistics = new ImageData(PictureColors);
            int totalPixelsNumber = bitmapWidth * bitmapHeight;

            var redPartialSums = CalculatePartialSums(statistics, 'R', totalPixelsNumber);
            var greenPartialSums = CalculatePartialSums(statistics, 'G', totalPixelsNumber);
            var bluePartialSums = CalculatePartialSums(statistics, 'B', totalPixelsNumber);

            int forWidth, forHeight;
            forWidth = (brushRadius == 0) ? bitmapWidth : brushRadius + brushCenter.X;
            forHeight = (brushRadius == 0) ? bitmapHeight : brushRadius + brushCenter.Y;
            for (int i = (brushRadius == 0) ? 0 : brushCenter.X - brushRadius; i < forWidth; i++) // if brush - iterate only inside rectangle containing that brush
            {
                for (int j = (brushRadius == 0) ? 0 : brushCenter.Y - brushRadius; j < forHeight; j++)
                {
                    if (i < 0 || j < 0 || i >= bitmapWidth || j >= bitmapHeight) continue;
                    if (alreadyFilteredPixels == null || !alreadyFilteredPixels[i, j])
                    {
                        if ((brushCenter == default(Point) && brushRadius == 0) || (CountDistance(brushCenter, new Point(i, j))) <= brushRadius)
                        {
                            int newRed = base.PictureColors[i, j].R;
                            int newGreen = base.PictureColors[i, j].G;
                            int newBlue = base.PictureColors[i, j].B;

                            if (args.Red)
                            {
                                newRed = redPartialSums[newRed]; //CalculatePixelIntesityTransforamtion(redPartialSums, newRed, 'R', totalPixelsNumber);
                            }
                            if (args.Green)
                            {
                                newGreen = greenPartialSums[newGreen]; //CalculatePixelIntesityTransforamtion(greenPartialSums, newGreen, 'G', totalPixelsNumber);
                            }
                            if (args.Blue)
                            {
                                newBlue = bluePartialSums[newBlue]; //CalculatePixelIntesityTransforamtion(bluePartialSums, newBlue, 'B', totalPixelsNumber);
                            }

                            Color newColor = Color.FromArgb(newRed, newGreen, newBlue);
                            outputBitmap.SetPixel(i, j, newColor);
                            if (alreadyFilteredPixels != null)
                                alreadyFilteredPixels[i, j] = true;
                        }
                    }
                }
            }

            return outputBitmap;
        }


        private int[] CalculatePartialSums(ImageData imageData, char channel, int totalPixelsNumber)
        {
            int[] partialSums = new int[256];
            for (int i = 0; i < 256; ++i)
            {
                switch (channel)
                {
                    case 'R':
                        partialSums[i] = imageData.RedValues.Take(i + 1).Sum();
                        partialSums[i] = (int)Math.Floor(255 * ((double)partialSums[i] / totalPixelsNumber));
                        break;
                    case 'G':
                        partialSums[i] = imageData.GreenValues.Take(i + 1).Sum();
                        partialSums[i] = (int)Math.Floor(255 * ((double)partialSums[i] / totalPixelsNumber));
                        break;
                    case 'B':
                        partialSums[i] = imageData.BlueValues.Take(i + 1).Sum();
                        partialSums[i] = (int)Math.Floor(255 * ((double)partialSums[i] / totalPixelsNumber));
                        break;
                }
            }
            return partialSums;
        }
    }

    public class AveragingFilter : Filter
    {
        Filter inputFilter;

        public AveragingFilter(Color[,] pictureColors, Filter filter = null) : base(pictureColors)
        {
            inputFilter = filter;
        }

        public override Bitmap ApplyFilter(Bitmap inputBitmap, FilterCanalArgs args, Point brushCenter = default(Point), int brushRadius = 0, bool[,] alreadyFilteredPixels = null)
        {
            Bitmap outputBitmap = inputBitmap;
            if (inputFilter != null)
                outputBitmap = inputFilter.ApplyFilter(inputBitmap, args, brushCenter, brushRadius);
            int bitmapWidth = inputBitmap.Width, bitmapHeight = inputBitmap.Height;

            int forWidth, forHeight;
            forWidth = (brushRadius == 0) ? bitmapWidth : brushRadius + brushCenter.X;
            forHeight = (brushRadius == 0) ? bitmapHeight : brushRadius + brushCenter.Y;
            for (int i = (brushRadius == 0) ? 0 : brushCenter.X - brushRadius; i < forWidth; i++) // if brush - iterate only inside rectangle containing that brush
            {
                for (int j = (brushRadius == 0) ? 0 : brushCenter.Y - brushRadius; j < forHeight; j++)
                {
                    if (i < 0 || j < 0 || i >= bitmapWidth || j >= bitmapHeight) continue;
                    if (alreadyFilteredPixels == null || !alreadyFilteredPixels[i, j])
                    {
                        if ((brushCenter == default(Point) && brushRadius == 0) || (CountDistance(brushCenter, new Point(i, j))) <= brushRadius)
                        {
                            int newRed = base.PictureColors[i, j].R;
                            int newGreen = base.PictureColors[i, j].G;
                            int newBlue = base.PictureColors[i, j].B;

                            int[] averagesArray = Helpers.PerformFilterOnPixelColor(i, j, PictureColors, Helpers.GetAveragingMatrix());

                            if (args.Red)
                            {
                                newRed = averagesArray[0];
                            }
                            if (args.Green)
                            {
                                newGreen = averagesArray[1];
                            }
                            if (args.Blue)
                            {
                                newBlue = averagesArray[2];
                            }

                            Helpers.CheckAndAmendChannels(ref newRed, ref newGreen, ref newBlue);

                            Color newColor = Color.FromArgb(newRed, newGreen, newBlue);
                            outputBitmap.SetPixel(i, j, newColor);
                            if (alreadyFilteredPixels != null)
                                alreadyFilteredPixels[i, j] = true;
                        }
                    }
                }
            }

            return outputBitmap;
        }
    }

    public class GaussianFilter : Filter
    {
        Filter inputFilter;
        double b;

        public GaussianFilter(Color[,] pictureColors, double b, Filter filter = null) : base(pictureColors)
        {
            inputFilter = filter;
            this.b = b;
        }

        public override Bitmap ApplyFilter(Bitmap inputBitmap, FilterCanalArgs args, Point brushCenter = default(Point), int brushRadius = 0, bool[,] alreadyFilteredPixels = null)
        {
            Bitmap outputBitmap = inputBitmap;
            if (inputFilter != null)
                outputBitmap = inputFilter.ApplyFilter(inputBitmap, args, brushCenter, brushRadius);
            int bitmapWidth = inputBitmap.Width, bitmapHeight = inputBitmap.Height;

            int forWidth, forHeight;
            forWidth = (brushRadius == 0) ? bitmapWidth : brushRadius + brushCenter.X;
            forHeight = (brushRadius == 0) ? bitmapHeight : brushRadius + brushCenter.Y;
            for (int i = (brushRadius == 0) ? 0 : brushCenter.X - brushRadius; i < forWidth; i++) // if brush - iterate only inside rectangle containing that brush
            {
                for (int j = (brushRadius == 0) ? 0 : brushCenter.Y - brushRadius; j < forHeight; j++)
                {
                    if (i < 0 || j < 0 || i >= bitmapWidth || j >= bitmapHeight) continue;
                    if (alreadyFilteredPixels == null || !alreadyFilteredPixels[i, j])
                    {
                        if ((brushCenter == default(Point) && brushRadius == 0) || (CountDistance(brushCenter, new Point(i, j))) <= brushRadius)
                        {
                            int newRed = base.PictureColors[i, j].R;
                            int newGreen = base.PictureColors[i, j].G;
                            int newBlue = base.PictureColors[i, j].B;

                            int[] averagesArray = Helpers.PerformFilterOnPixelColor(i, j, PictureColors, Helpers.GetGaussianFilterMatrix(b));

                            if (args.Red)
                            {
                                newRed = averagesArray[0];
                            }
                            if (args.Green)
                            {
                                newGreen = averagesArray[1];
                            }
                            if (args.Blue)
                            {
                                newBlue = averagesArray[2];
                            }

                            Helpers.CheckAndAmendChannels(ref newRed, ref newGreen, ref newBlue);

                            Color newColor = Color.FromArgb(newRed, newGreen, newBlue);
                            outputBitmap.SetPixel(i, j, newColor);
                            if (alreadyFilteredPixels != null)
                                alreadyFilteredPixels[i, j] = true;
                        }
                    }
                }
            }

            return outputBitmap;
        }
    }

    public class LaplacianFilter : Filter
    {
        Filter inputFilter;

        public LaplacianFilter(Color[,] pictureColors, Filter filter = null) : base(pictureColors)
        {
            inputFilter = filter;
        }

        public override Bitmap ApplyFilter(Bitmap inputBitmap, FilterCanalArgs args, Point brushCenter = default(Point), int brushRadius = 0, bool[,] alreadyFilteredPixels = null)
        {
            Bitmap outputBitmap = inputBitmap;
            if (inputFilter != null)
                outputBitmap = inputFilter.ApplyFilter(inputBitmap, args, brushCenter, brushRadius);
            int bitmapWidth = inputBitmap.Width, bitmapHeight = inputBitmap.Height;

            int forWidth, forHeight;
            forWidth = (brushRadius == 0) ? bitmapWidth : brushRadius + brushCenter.X;
            forHeight = (brushRadius == 0) ? bitmapHeight : brushRadius + brushCenter.Y;
            for (int i = (brushRadius == 0) ? 0 : brushCenter.X - brushRadius; i < forWidth; i++) // if brush - iterate only inside rectangle containing that brush
            {
                for (int j = (brushRadius == 0) ? 0 : brushCenter.Y - brushRadius; j < forHeight; j++)
                {
                    if (i < 0 || j < 0 || i >= bitmapWidth || j >= bitmapHeight) continue;
                    if (alreadyFilteredPixels == null || !alreadyFilteredPixels[i, j])
                    {
                        if ((brushCenter == default(Point) && brushRadius == 0) || (CountDistance(brushCenter, new Point(i, j))) <= brushRadius)
                        {
                            int newRed = base.PictureColors[i, j].R;
                            int newGreen = base.PictureColors[i, j].G;
                            int newBlue = base.PictureColors[i, j].B;

                            int[] averagesArray = Helpers.PerformFilterOnPixelColor(i, j, PictureColors, Helpers.GetLaplacianFilterMatrix());

                            if (args.Red)
                            {
                                newRed = averagesArray[0];
                            }
                            if (args.Green)
                            {
                                newGreen = averagesArray[1];
                            }
                            if (args.Blue)
                            {
                                newBlue = averagesArray[2];
                            }

                            Helpers.CheckAndAmendChannels(ref newRed, ref newGreen, ref newBlue);

                            Color newColor = Color.FromArgb(newRed, newGreen, newBlue);
                            outputBitmap.SetPixel(i, j, newColor);
                            if (alreadyFilteredPixels != null)
                                alreadyFilteredPixels[i, j] = true;
                        }
                    }
                }
            }

            return outputBitmap;
        }
    }

    public class LaplacianDerivativesOnDiagFilter : Filter
    {
        Filter inputFilter;

        public LaplacianDerivativesOnDiagFilter(Color[,] pictureColors, Filter filter = null) : base(pictureColors)
        {
            inputFilter = filter;
        }

        public override Bitmap ApplyFilter(Bitmap inputBitmap, FilterCanalArgs args, Point brushCenter = default(Point), int brushRadius = 0, bool[,] alreadyFilteredPixels = null)
        {
            Bitmap outputBitmap = inputBitmap;
            if (inputFilter != null)
                outputBitmap = inputFilter.ApplyFilter(inputBitmap, args, brushCenter, brushRadius);
            int bitmapWidth = inputBitmap.Width, bitmapHeight = inputBitmap.Height;

            int forWidth, forHeight;
            forWidth = (brushRadius == 0) ? bitmapWidth : brushRadius + brushCenter.X;
            forHeight = (brushRadius == 0) ? bitmapHeight : brushRadius + brushCenter.Y;
            for (int i = (brushRadius == 0) ? 0 : brushCenter.X - brushRadius; i < forWidth; i++) // if brush - iterate only inside rectangle containing that brush
            {
                for (int j = (brushRadius == 0) ? 0 : brushCenter.Y - brushRadius; j < forHeight; j++)
                {
                    if (i < 0 || j < 0 || i >= bitmapWidth || j >= bitmapHeight) continue;
                    if (alreadyFilteredPixels == null || !alreadyFilteredPixels[i, j])
                    {
                        if ((brushCenter == default(Point) && brushRadius == 0) || (CountDistance(brushCenter, new Point(i, j))) <= brushRadius)
                        {
                            int newRed = base.PictureColors[i, j].R;
                            int newGreen = base.PictureColors[i, j].G;
                            int newBlue = base.PictureColors[i, j].B;

                            int[] averagesArray = Helpers.PerformFilterOnPixelColor(i, j, PictureColors, Helpers.GetLaplacianFilterMatrixDerivativesOnDiag());

                            if (args.Red)
                            {
                                newRed = averagesArray[0];
                            }
                            if (args.Green)
                            {
                                newGreen = averagesArray[1];
                            }
                            if (args.Blue)
                            {
                                newBlue = averagesArray[2];
                            }

                            Helpers.CheckAndAmendChannels(ref newRed, ref newGreen, ref newBlue);

                            Color newColor = Color.FromArgb(newRed, newGreen, newBlue);
                            outputBitmap.SetPixel(i, j, newColor);
                            if (alreadyFilteredPixels != null)
                                alreadyFilteredPixels[i, j] = true;
                        }
                    }
                }
            }

            return outputBitmap;
        }
    }

    public class LaplacianThreeParallelLinesFilter : Filter
    {
        Filter inputFilter;

        public LaplacianThreeParallelLinesFilter(Color[,] pictureColors, Filter filter = null) : base(pictureColors)
        {
            inputFilter = filter;
        }

        public override Bitmap ApplyFilter(Bitmap inputBitmap, FilterCanalArgs args, Point brushCenter = default(Point), int brushRadius = 0, bool[,] alreadyFilteredPixels = null)
        {
            Bitmap outputBitmap = inputBitmap;
            if (inputFilter != null)
                outputBitmap = inputFilter.ApplyFilter(inputBitmap, args, brushCenter, brushRadius);
            int bitmapWidth = inputBitmap.Width, bitmapHeight = inputBitmap.Height;

            int forWidth, forHeight;
            forWidth = (brushRadius == 0) ? bitmapWidth : brushRadius + brushCenter.X;
            forHeight = (brushRadius == 0) ? bitmapHeight : brushRadius + brushCenter.Y;
            for (int i = (brushRadius == 0) ? 0 : brushCenter.X - brushRadius; i < forWidth; i++) // if brush - iterate only inside rectangle containing that brush
            {
                for (int j = (brushRadius == 0) ? 0 : brushCenter.Y - brushRadius; j < forHeight; j++)
                {
                    if (i < 0 || j < 0 || i >= bitmapWidth || j >= bitmapHeight) continue;
                    if (alreadyFilteredPixels == null || !alreadyFilteredPixels[i, j])
                    {
                        if ((brushCenter == default(Point) && brushRadius == 0) || (CountDistance(brushCenter, new Point(i, j))) <= brushRadius)
                        {
                            int newRed = base.PictureColors[i, j].R;
                            int newGreen = base.PictureColors[i, j].G;
                            int newBlue = base.PictureColors[i, j].B;

                            int[] averagesArray = Helpers.PerformFilterOnPixelColor(i, j, PictureColors, Helpers.GetLaplacianFilterMatrixThreeParallelLines());

                            if (args.Red)
                            {
                                newRed = averagesArray[0];
                            }
                            if (args.Green)
                            {
                                newGreen = averagesArray[1];
                            }
                            if (args.Blue)
                            {
                                newBlue = averagesArray[2];
                            }

                            Helpers.CheckAndAmendChannels(ref newRed, ref newGreen, ref newBlue);

                            Color newColor = Color.FromArgb(newRed, newGreen, newBlue);
                            outputBitmap.SetPixel(i, j, newColor);
                            if (alreadyFilteredPixels != null)
                                alreadyFilteredPixels[i, j] = true;
                        }
                    }
                }
            }

            return outputBitmap;
        }
    }

    public class RobertsCrossFilter : Filter
    {
        Filter inputFilter;

        public RobertsCrossFilter(Color[,] pictureColors, Filter filter = null) : base(pictureColors)
        {
            inputFilter = filter;
        }

        public override Bitmap ApplyFilter(Bitmap inputBitmap, FilterCanalArgs args, Point brushCenter = default(Point), int brushRadius = 0, bool[,] alreadyFilteredPixels = null)
        {
            Bitmap outputBitmap = inputBitmap;
            if (inputFilter != null)
                outputBitmap = inputFilter.ApplyFilter(inputBitmap, args, brushCenter, brushRadius);
            int bitmapWidth = inputBitmap.Width, bitmapHeight = inputBitmap.Height;

            int forWidth, forHeight;
            forWidth = (brushRadius == 0) ? bitmapWidth : brushRadius + brushCenter.X;
            forHeight = (brushRadius == 0) ? bitmapHeight : brushRadius + brushCenter.Y;
            for (int i = (brushRadius == 0) ? 0 : brushCenter.X - brushRadius; i < forWidth; i++) // if brush - iterate only inside rectangle containing that brush
            {
                for (int j = (brushRadius == 0) ? 0 : brushCenter.Y - brushRadius; j < forHeight; j++)
                {
                    if (i < 0 || j < 0 || i >= bitmapWidth || j >= bitmapHeight) continue;
                    if (alreadyFilteredPixels == null || !alreadyFilteredPixels[i, j])
                    {
                        if ((brushCenter == default(Point) && brushRadius == 0) || (CountDistance(brushCenter, new Point(i, j))) <= brushRadius)
                        {
                            int newRed = base.PictureColors[i, j].R;
                            int newGreen = base.PictureColors[i, j].G;
                            int newBlue = base.PictureColors[i, j].B;

                            int[] xArray = Helpers.PerformFilterOnPixelColor(i, j, PictureColors, Helpers.GetRobertsCrossX());
                            int[] yArray = Helpers.PerformFilterOnPixelColor(i, j, PictureColors, Helpers.GetRobertsCrossY());

                            if (args.Red)
                            {
                                newRed = (int)Math.Sqrt(xArray[0] * xArray[0] + yArray[0] * yArray[0]);
                            }
                            if (args.Green)
                            {
                                newGreen = (int)Math.Sqrt(xArray[1] * xArray[1] + yArray[1] * yArray[1]);
                            }
                            if (args.Blue)
                            {
                                newBlue = (int)Math.Sqrt(xArray[2] * xArray[2] + yArray[2] * yArray[2]);
                            }

                            Helpers.CheckAndAmendChannels(ref newRed, ref newGreen, ref newBlue);

                            Color newColor = Color.FromArgb(newRed, newGreen, newBlue);
                            outputBitmap.SetPixel(i, j, newColor);
                            if (alreadyFilteredPixels != null)
                                alreadyFilteredPixels[i, j] = true;
                        }
                    }
                }
            }

            return outputBitmap;
        }
    }

}
