using System.Diagnostics;
using System.Threading;

namespace ColorAimbot.Worker
{
    public class WorkerBase
    {
        private Thread _processThread;
        private volatile bool _doProcess;

        private int _counter = 0;

        public virtual bool Start()
        {
            if (_doProcess) return false;

            _processThread = new Thread(worker) {IsBackground = true};

            _processThread.Start();
            _doProcess = true;

            return true;
        }

        public virtual bool Stop()
        {
            if (!_doProcess) return false;

            _doProcess = false;
            return true;
        }

        public int OperationsPerSecond { get; private set; }

        private Stopwatch sw = Stopwatch.StartNew();

        private void worker()
        {
            while (true)
            {
                if (sw.Elapsed.TotalSeconds >= 1)
                {
                    OperationsPerSecond = _counter;
                    _counter = 0;
                    sw.Restart();
                }
                process();
                _counter++;
            }
        }

        public virtual void process()
        {
        }
    }
}
