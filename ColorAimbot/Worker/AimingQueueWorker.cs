using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.Remoting.Channels;
using System.Threading;
using ColorAimbot.Helper;
using Timer = System.Timers.Timer;

namespace ColorAimbot.Worker
{
    public class AimingQueueWorker : QueueWorkerBase<ConcurrentBag<Target.Target>>
    {
        private readonly Stopwatch sw = new Stopwatch();

        private readonly object framesLockobj = new object();
        private volatile int frames;

        private readonly Settings settings;
        readonly Timer fpsTimer = new Timer();

        public AimingQueueWorker(ref Settings settings)
        {
            this.settings = settings;

            fpsTimer.Interval = 250;
            fpsTimer.Elapsed += (sender, args) => fpsCounter();
            fpsTimer.Start();
        }

        public override bool Start()
        {
            sw.Restart();
            return base.Start();
        }

        public override bool Stop()
        {
            sw.Stop();
            return base.Stop();
        }

        private void fpsCounter()
        {
            lock (framesLockobj)
            {
                StaticInstance.FPS = frames / (float) sw.Elapsed.TotalSeconds;
                frames = 0;
            }
            sw.Restart();
        }

        public override void processItem(ConcurrentBag<Target.Target> targets)
        {
            lock (framesLockobj)
                frames++;

            if (targets.Count == 0)
                return;

            var mouse = settings.TargetWindow.CursorPosition;

            Target.Target target = new Target.Target();
            double shortetDistance = float.MaxValue;
            int highestPriority = 0;

            Vector delta = new Vector();

            foreach (var t in targets)
            {
                if (t.targetDescriptor.priority > highestPriority)
                    target = t;
                else
                    continue;

                Vector targetPos = Vector.Multiply(new Vector(target.targetBlob.CenterOfGravity.X, target.targetBlob.CenterOfGravity.Y), 1.0 / settings.ScaleFactor);

                Vector d = Vector.Subtract(targetPos, mouse);

                double length = d.Length;

                if (length < shortetDistance)
                {
                    target = t;
                    delta = d;
                    shortetDistance = length;
                }
            }

            //double formula = (1 - Math.Pow(Math.E, (-(delta.Length / 350) * 2))) * 100;
            double formula = Math.Pow(delta.Length, 2) * 200;

            if (formula < 0.5)
                return;

            Vector origDelta = delta;
            delta.Normalize();

            Vector movement;

            movement = Vector.Multiply(delta, formula / StaticInstance.FPS * 50.0);

            if (double.IsNaN(movement.X) || double.IsNaN(movement.Y))
                return;

            if (movement.Length > origDelta.Length)
                movement = origDelta;

            Mouse.Move(movement);
        }
    }
}
