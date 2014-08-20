using System.Drawing;
using System.Threading;
using ColorAimbot.Helper;

namespace ColorAimbot.Worker
{
    internal class ScreenshotWorker : WorkerBase
    {
        private readonly QueueWorkerBase<Bitmap> outQueue;
        private Settings settings;

        public ScreenshotWorker(QueueWorkerBase<Bitmap> outQueue, ref Settings settings)
        {
            this.outQueue = outQueue;
            this.settings = settings;
        }

        public override void process()
        {
            if (outQueue.Count != 0)
            {
                Thread.Sleep(1);
                return;
            }

            if (!settings.TargetWindow.FoundWindow || !settings.TargetWindow.IsInForeground)
            {
                Thread.Sleep(50);
                return;
            }

            Bitmap screenshot = ScreenCapture.Screenshot(settings.TargetWindow.WindowRectangle);

            if (screenshot == null)
            {
                Thread.Sleep(50);
                return;
            }

            outQueue.Enqueue(screenshot);
        }
    }
}