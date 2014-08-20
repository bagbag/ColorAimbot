using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;
using ColorAimbot.Filter;
using ColorAimbot.Helper;
using ColorAimbot.Target;

namespace ColorAimbot.Worker
{
    public class FilterQueueWorker : QueueWorkerBase<Bitmap>
    {
        private readonly QueueWorkerBase<KeyValuePair<KeyValuePair<Bitmap, BitmapData>, TargetDescriptor>[]> outQueue;

        private Settings settings;

        public FilterQueueWorker(QueueWorkerBase<KeyValuePair<KeyValuePair<Bitmap, BitmapData>, TargetDescriptor>[]> outQueue,ref Settings settings)
        {
            this.outQueue = outQueue;

            this.settings = settings;
        }

        public override void processItem(Bitmap bitmap)
        {
            if (outQueue.Count != 0)
            {
                Thread.Sleep(1);
                return;
            }

            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            KeyValuePair<KeyValuePair<Bitmap, BitmapData>, TargetDescriptor>[] filteredBitmaps = new KeyValuePair<KeyValuePair<Bitmap, BitmapData>, TargetDescriptor>[settings.TargetDescriptors.Count];

            object lockobj = new object();

            int i = 0;
            Parallel.ForEach(settings.TargetDescriptors, targetDescriptor =>
                                                 {
                                                     var result = new KeyValuePair<KeyValuePair<Bitmap, BitmapData>, TargetDescriptor>(ColorFilter.FilterImage(ref bitmapData, targetDescriptor.filter, targetDescriptor.priority), targetDescriptor);

                                                     lock (lockobj)
                                                         filteredBitmaps[i++] = result;
                                                 });

            StaticInstance.STP.QueueWorkItem((b, bd) =>
                                             {
                                                 b.UnlockBits(bd);
                                                 b.Dispose();
                                             }, bitmap, bitmapData);

            outQueue.Enqueue(filteredBitmaps);
        }
    }
}
