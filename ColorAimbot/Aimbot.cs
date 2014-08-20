using System;
using System.IO;
using ColorAimbot.Helper;
using ColorAimbot.Worker;

namespace ColorAimbot
{
    internal class Aimbot
    {
        private readonly Settings settings;
        public Settings Settings { get { return settings; } }

        private readonly AimingQueueWorker aimingQueueWorker;
        private readonly TargetsFinderQueueWorker targetsFinderQueueWorker;
        private readonly FilterQueueWorker filterQueueWorker;
        private readonly ResizeQueueWorker resizeQueueWorker;
        private readonly ScreenshotWorker screenshotWorker;

        public Aimbot(string settingsPath = @"ColorAimbot Settings.xml")
        {
            settings = new Settings(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), settingsPath));

            aimingQueueWorker = new AimingQueueWorker(ref settings);
            targetsFinderQueueWorker = new TargetsFinderQueueWorker(aimingQueueWorker, ref settings);
            filterQueueWorker = new FilterQueueWorker(targetsFinderQueueWorker, ref settings);
            resizeQueueWorker = new ResizeQueueWorker(filterQueueWorker, ref settings);
            screenshotWorker = new ScreenshotWorker(resizeQueueWorker, ref settings);
        }

        public void Start()
        {
            screenshotWorker.Start();
            resizeQueueWorker.Start();
            filterQueueWorker.Start();
            targetsFinderQueueWorker.Start();
            aimingQueueWorker.Start();
        }

        public void Stop()
        {
            screenshotWorker.Stop();
            resizeQueueWorker.Stop();
            filterQueueWorker.Stop();
            targetsFinderQueueWorker.Stop();
            aimingQueueWorker.Stop();
        }
    }
}
