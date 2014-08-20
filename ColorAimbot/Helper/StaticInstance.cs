using Amib.Threading;

namespace ColorAimbot.Helper
{
    internal static class StaticInstance
    {
        private static object lockobj = new object();
        private static readonly SmartThreadPool stp;
        private volatile static float fps;
        public static SmartThreadPool STP { get { lock (lockobj)return stp; } }

        public static float FPS { get { lock (lockobj)return fps; } set { lock (lockobj)fps = value; } }

        static StaticInstance()
        {
            stp = new SmartThreadPool(new STPStartInfo
                                      {
                                          AreThreadsBackground = true,
                                          MaxWorkerThreads = 15
                                      });

            STP.Start();

            FPS = 0;
        }
    }
}
