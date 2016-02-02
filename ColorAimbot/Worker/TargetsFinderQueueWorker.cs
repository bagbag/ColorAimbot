using System;
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
        private readonly QueueWorkerBase<ConcurrentBag<Target.Target>> _outQueue;
        private readonly Settings _settings;

        private readonly object _lockObject = new object();

        public TargetsFinderQueueWorker(QueueWorkerBase<ConcurrentBag<Target.Target>> outQueue, ref Settings settings)
        {
            _outQueue = outQueue;
            _settings = settings;
        }


        public override void processItem(KeyValuePair<KeyValuePair<Bitmap, BitmapData>, TargetDescriptor>[] item)
        {
            if (_outQueue.Count != 0)
            {
                Thread.Sleep(1);
                return;
            }

            var targetList = new ConcurrentBag<Target.Target>();

            var totalTargets = 0;

            Parallel.ForEach(item, pair =>
            {
                var targetCounter = new BlobCounter
                {
                    CoupledSizeFiltering = true,
                    BackgroundThreshold = Color.Black,
                    FilterBlobs = true,
                    MinWidth = (int)(_settings.MinWidth * _settings.ScaleFactor),
                    MinHeight = (int)(_settings.MinHeight * _settings.ScaleFactor),
                    ObjectsOrder = ObjectsOrder.None
                };

                var image = pair.Key;
                targetCounter.ProcessImage(image.Value);

                var targets = targetCounter.GetObjectsInformation();

                foreach (var target in targets)
                {
                    targetList.Add(new Target.Target(pair.Value, target));
                }

                lock(_lockObject)
                    totalTargets += targets.Length;

                StaticInstance.STP.QueueWorkItem(p =>
                {
                    p.Key.UnlockBits(p.Value);
                    p.Key.Dispose();
                }, pair.Key);
            });

            StaticInstance._mainWindowViewModel.VisibleTargets = totalTargets;

            Console.WriteLine(targetList.Count);

            _outQueue.Enqueue(targetList);
        }
    }
}
