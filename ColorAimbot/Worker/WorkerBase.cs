using System.Diagnostics;
using System.Threading;

namespace ColorAimbot.Worker
{
    public class WorkerBase
    {
        private Thread processThread;
        private volatile bool doProcess;

        public int counter = 0;

        public virtual bool Start()
        {
            if (doProcess) return false;

            processThread = new Thread(worker) {IsBackground = true};

            processThread.Start();
            doProcess = true;

            return true;
        }

        public virtual bool Stop()
        {
            if (!doProcess) return false;

            doProcess = false;
            return true;
        }

        public int operationsPerSecond { get; private set; }

        private Stopwatch sw = Stopwatch.StartNew();
        private void worker()
        {
            while (true)
            {
                if (sw.Elapsed.TotalSeconds >= 1)
                {
                    operationsPerSecond = counter;
                    counter = 0;
                    sw.Restart();
                }
                process();
                counter++;
            }
        }

        public virtual void process()
        {
        }
    }
}
