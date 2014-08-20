using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;
using AForge.Imaging;
using ColorAimbot.Helper;
using ColorAimbot.Target;

namespace ColorAimbot.Worker
{
    internal class TargetsFinderQueueWorker : QueueWorkerBase<KeyValuePair<KeyValuePair<Bitmap, BitmapData>, TargetDescriptor>[]>
    {
        private readonly QueueWorkerBase<ConcurrentBag<Target.Target>> outQueue;

        private Settings settings;

        public TargetsFinderQueueWorker(QueueWorkerBase<ConcurrentBag<Target.Target>> outQueue,ref Settings settings)
        {
            this.outQueue = outQueue;
            this.settings = settings;
        }

        public override void processItem(KeyValuePair<KeyValuePair<Bitmap, BitmapData>, TargetDescriptor>[] item)
        {
            if (outQueue.Count != 0)
            {
                Thread.Sleep(1);
                return;
            }

            var targetList = new ConcurrentBag<Target.Target>();

            Parallel.ForEach(item, pair =>
                                   {
                                       BlobCounter targetCounter = new BlobCounter();
                                       targetCounter.CoupledSizeFiltering = true;
                                       targetCounter.BackgroundThreshold = Color.Black;
                                       targetCounter.FilterBlobs = true;
                                       targetCounter.MinWidth = (int)(settings.MinWidth * settings.ScaleFactor);
                                       targetCounter.MinHeight = (int)(settings.MinHeight * settings.ScaleFactor);
                                       targetCounter.ObjectsOrder = ObjectsOrder.None;

                                       var image = pair.Key;
                                       targetCounter.ProcessImage(image.Value);

                                       var targets = targetCounter.GetObjectsInformation();

                                       foreach (var target in targets)
                                       {
                                           targetList.Add(new Target.Target(pair.Value, target));
                                       }

                                       StaticInstance.STP.QueueWorkItem(p =>
                                                                        {
                                                                            p.Key.UnlockBits(p.Value);
                                                                            p.Key.Dispose();
                                                                        }, pair.Key);
                                   });

            outQueue.Enqueue(targetList);
        }
    }
}
