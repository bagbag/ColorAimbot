using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using AForge.Imaging;
using ColorAimbot.Helper;

namespace ColorAimbot.Filter
{
    internal static class ColorFilter
    {
        public static unsafe KeyValuePair<Bitmap,BitmapData> FilterImage(ref BitmapData bitmapData, IFilter filter, byte setValue)
        {
            var filteredBitmap = new Bitmap(bitmapData.Width, bitmapData.Height, PixelFormat.Format8bppIndexed);
            var filteredBitmapData = filteredBitmap.LockBits(new Rectangle(0, 0, filteredBitmap.Width, filteredBitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            var startaddr = bitmapData.Scan0.ToInt64();
            var rowoffset = bitmapData.Stride;
            var rows = bitmapData.Height;

            var filteredstartaddr = filteredBitmapData.Scan0.ToInt64();
            var filteredrowoffset = filteredBitmapData.Stride;

            Parallel.For((long) 0, rows, row =>
                                         {
                                             var dataptr = (byte*) (startaddr + rowoffset * row);
                                             var rowendptr = dataptr + rowoffset;

                                             var filtereddataptr = (byte*) (filteredstartaddr + filteredrowoffset * row);

                                             while (dataptr < rowendptr)
                                             {
                                                 if (filter.FilterPixel(dataptr[2], dataptr[1], dataptr[0]))

                                                     filtereddataptr[0] = setValue;
                                                 else
                                                     filtereddataptr[0] = 0;

                                                 dataptr += 3;
                                                 filtereddataptr++;
                                             }
                                         });
            
            return new KeyValuePair<Bitmap, BitmapData>(filteredBitmap, filteredBitmapData);
        }
    }
}
