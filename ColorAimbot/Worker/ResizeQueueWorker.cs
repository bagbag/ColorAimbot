using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using ColorAimbot.Helper;

namespace ColorAimbot.Worker
{
    public class ResizeQueueWorker : QueueWorkerBase<Bitmap>
    {
        private readonly QueueWorkerBase<Bitmap> outQueue;

        private Settings settings;

        public ResizeQueueWorker(QueueWorkerBase<Bitmap> outQueue,ref Settings settings)
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

                Bitmap scaledBitmap;

                if (settings.ScaleFactor < 1.0f)
                {
                    var scaledWidth = (int)(bitmap.Width * settings.ScaleFactor);
                    var scaledHeight = (int)(bitmap.Height * settings.ScaleFactor);

                    scaledBitmap = new Bitmap(scaledWidth, scaledHeight);

                    Graphics scaler = Graphics.FromImage(scaledBitmap);
                    scaler.InterpolationMode = InterpolationMode.NearestNeighbor;
                    scaler.DrawImage(bitmap, 0, 0, scaledWidth, scaledHeight);

                    StaticInstance.STP.QueueWorkItem((s) => s.Dispose(), bitmap);
                    StaticInstance.STP.QueueWorkItem((s) => s.Dispose(), scaler);
                }
                else
                    scaledBitmap = bitmap;

                outQueue.Enqueue(scaledBitmap);
        }
    }
}
