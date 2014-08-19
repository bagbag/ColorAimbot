using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amib.Threading;

namespace aforgefiltertest
{
    internal static class StaticInstance
    {
        public static SmartThreadPool SmartThreadPool { get; set; }

        static StaticInstance()
        {
            SmartThreadPool = new SmartThreadPool(new STPStartInfo()
                                                  {
                                                      AreThreadsBackground = true,
                                                      MaxWorkerThreads = 15
                                                  });

            SmartThreadPool.Start();
        }
    }
}
