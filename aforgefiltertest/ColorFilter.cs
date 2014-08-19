using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AForge.Imaging;

namespace aforgefiltertest
{
    internal static class ColorFilter
    {
        public static unsafe void FilterImage(ref Bitmap bitmap, ref BitmapData bitmapData, RGBFilter filter, byte setValue)
        {
            Bitmap filteredImage = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format8bppIndexed);
            BitmapData filteredBitmapData = filteredImage.LockBits(new Rectangle(0, 0, filteredImage.Width, filteredImage.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            long startaddr = bitmapData.Scan0.ToInt64();
            int rowoffset = bitmapData.Stride;
            int rows = bitmapData.Height;

            long filteredstartaddr = filteredBitmapData.Scan0.ToInt64();
            int filteredrowoffset = filteredBitmapData.Stride;

            Parallel.For((long) 0, rows, row =>
                                         {
                                             byte* dataptr = (byte*) (startaddr + rowoffset * row);
                                             byte* rowendptr = dataptr + rowoffset;

                                             byte* filtereddataptr = (byte*) (filteredstartaddr + filteredrowoffset * row);

                                             while (dataptr < rowendptr)
                                             {
                                                 if (dataptr[2] >= filter.minRGB.Red && dataptr[2] <= filter.maxRGB.Red && //red
                                                     dataptr[1] >= filter.minRGB.Green && dataptr[1] <= filter.maxRGB.Green && //green
                                                     dataptr[0] >= filter.minRGB.Blue && dataptr[0] <= filter.maxRGB.Blue) //blue

                                                     filtereddataptr[0] = setValue;
                                                 else
                                                     filtereddataptr[0] = 0;

                                                 dataptr += 3;
                                                 filtereddataptr++;
                                             }
                                         });

            StaticInstance.SmartThreadPool.QueueWorkItem((b, bd) =>
                                                         {
                                                             b.UnlockBits(bd);
                                                             b.Dispose();
                                                         }, bitmap, bitmapData);

            bitmap = filteredImage;
            bitmapData = filteredBitmapData;
        }

        public static unsafe void FilterImage(ref Bitmap bitmap, ref BitmapData bitmapData, HSLFilter filter, byte setValue)
        {
            Bitmap filteredImage = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format8bppIndexed);
            BitmapData filteredBitmapData = filteredImage.LockBits(new Rectangle(0, 0, filteredImage.Width, filteredImage.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            long startaddr = bitmapData.Scan0.ToInt64();
            int rowoffset = bitmapData.Stride;
            int rows = bitmapData.Height;

            long filteredstartaddr = filteredBitmapData.Scan0.ToInt64();
            int filteredrowoffset = filteredBitmapData.Stride;

            Parallel.For((long) 0, rows, row =>
                                         {
                                             byte* dataptr = (byte*) (startaddr + rowoffset * row);
                                             byte* rowendptr = dataptr + rowoffset;

                                             byte* filtereddataptr = (byte*) (filteredstartaddr + filteredrowoffset * row);

                                             while (dataptr < rowendptr)
                                             {
                                                 RGB pixelrgb = new RGB(dataptr[2], dataptr[1], dataptr[0]);
                                                 HSL pixelhsl = HSL.FromRGB(pixelrgb);

                                                 if (pixelhsl.Hue >= filter.minHSL.Hue && pixelhsl.Hue <= filter.maxHSL.Hue &&
                                                     pixelhsl.Saturation >= filter.minHSL.Saturation && pixelhsl.Saturation <= filter.maxHSL.Saturation &&
                                                     pixelhsl.Luminance >= filter.minHSL.Luminance && pixelhsl.Luminance <= filter.maxHSL.Luminance)

                                                     filtereddataptr[0] = setValue;
                                                 else
                                                     filtereddataptr[0] = 0;

                                                 dataptr += 3;
                                                 filtereddataptr++;
                                             }
                                         });

            StaticInstance.SmartThreadPool.QueueWorkItem((b, bd) =>
                                                         {
                                                             b.UnlockBits(bd);
                                                             b.Dispose();
                                                         }, bitmap, bitmapData);

            bitmap = filteredImage;
            bitmapData = filteredBitmapData;
        }
    }

    internal struct RGBFilter
    {
        public RGB minRGB;
        public RGB maxRGB;

        public RGBFilter(RGB minRGB, RGB maxRGB)
        {
            this.minRGB = minRGB;
            this.maxRGB = maxRGB;
        }
    }

    internal struct HSLFilter
    {
        public HSL minHSL;
        public HSL maxHSL;

        public HSLFilter(HSL minHSL, HSL maxHSL)
        {
            this.minHSL = minHSL;
            this.maxHSL = maxHSL;
        }
    }
}
