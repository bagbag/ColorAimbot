using System;
using System.IO;
using ColorAimbot.Helper;
using ColorAimbot.Worker;

namespace ColorAimbot
{
    internal class Aimbot
    {
        private readonly Settings _settings;
        public Settings Settings { get { return _settings; } }

        private readonly AimingQueueWorker _aimingQueueWorker;
        private readonly TargetsFinderQueueWorker _targetsFinderQueueWorker;
        private readonly FilterQueueWorker _filterQueueWorker;
        private readonly ResizeQueueWorker _resizeQueueWorker;
        private readonly ScreenshotWorker _screenshotWorker;

        public Aimbot(string settingsPath = @"ColorAimbot Settings.xml")
        {
            _settings = new Settings(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), settingsPath));

            _aimingQueueWorker = new AimingQueueWorker(ref _settings);
            _targetsFinderQueueWorker = new TargetsFinderQueueWorker(_aimingQueueWorker, ref _settings);
            _filterQueueWorker = new FilterQueueWorker(_targetsFinderQueueWorker, ref _settings);
            _resizeQueueWorker = new ResizeQueueWorker(_filterQueueWorker, ref _settings);
            _screenshotWorker = new ScreenshotWorker(_resizeQueueWorker, ref _settings);
        }

        public void Start()
        {
            _screenshotWorker.Start();
            _resizeQueueWorker.Start();
            _filterQueueWorker.Start();
            _targetsFinderQueueWorker.Start();
            _aimingQueueWorker.Start();
        }

        public void Stop()
        {
            _screenshotWorker.Stop();
            _resizeQueueWorker.Stop();
            _filterQueueWorker.Stop();
            _targetsFinderQueueWorker.Stop();
            _aimingQueueWorker.Stop();
        }
    }
}
