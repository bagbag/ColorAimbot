using System.ComponentModel;
using System.Drawing;
using Amib.Threading;
using ColorAimbot.ViewModels;

namespace ColorAimbot.Helper
{
    internal static class StaticInstance
    {
        private static readonly object _lockobj = new object();
        private static readonly SmartThreadPool _stp;
        private volatile static float _fps;

        public static SmartThreadPool STP { get { lock (_lockobj)return _stp; } }
        public static float FPS { get { lock (_lockobj)return _fps; } set { lock (_lockobj)_fps = _mainWindowViewModel.FPS = value; } }

        internal static readonly MainWindowViewModel _mainWindowViewModel = new MainWindowViewModel();

        static StaticInstance()
        {
            _stp = new SmartThreadPool(new STPStartInfo
                                      {
                                          AreThreadsBackground = true,
                                          MaxWorkerThreads = 15
                                      });

            STP.Start();
        }
    }
}
