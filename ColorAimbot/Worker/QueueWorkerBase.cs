using System.Collections.Concurrent;
using System.Threading;

namespace ColorAimbot.Worker
{
    public class QueueWorkerBase<TInType> : WorkerBase
    {
        private readonly ConcurrentQueue<TInType> inQueue;

        public QueueWorkerBase()
        {
            inQueue = new ConcurrentQueue<TInType>();
        }

        public void Enqueue(TInType item)
        {
            inQueue.Enqueue(item);
        }

        public int Count { get { return inQueue.Count; } }

        public override void process()
        {
            TInType item;

            if (!inQueue.TryDequeue(out item))
            {
                Thread.Sleep(1);
                return;
            }

            processItem(item);
        }

        public virtual void processItem(TInType item)
        {
            
        }
    }
}
