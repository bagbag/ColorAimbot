using System.Collections.Concurrent;
using System.Threading;

namespace ColorAimbot.Worker
{
    public class QueueWorkerBase<inType> : WorkerBase
    {
        private readonly ConcurrentQueue<inType> inQueue;

        public QueueWorkerBase()
        {
            inQueue = new ConcurrentQueue<inType>();
        }

        public void Enqueue(inType item)
        {
            inQueue.Enqueue(item);
        }

        public int Count { get { return inQueue.Count; } }

        public override void process()
        {
            inType item;

            if (!inQueue.TryDequeue(out item))
            {
                Thread.Sleep(1);
                return;
            }

            processItem(item);
        }

        public virtual void processItem(inType item)
        {
            
        }
    }
}
