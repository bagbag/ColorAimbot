using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Threading;
using System.Windows.Input;
using ColorAimbot.Helper;
using Mouse = ColorAimbot.Helper.Mouse;
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

            var highestPriority = 0;

            foreach (var t in targets)
            {
                if (t.targetDescriptor.priority > highestPriority)
                    highestPriority = t.targetDescriptor.priority;
            }

            var highestPriorityTargets = targets.Where(t => t.targetDescriptor.priority == highestPriority).ToList();



            var mouse = settings.TargetWindow.CursorPosition;
            var shortestDistance = double.MaxValue;
            var mouseDelta = new Vector();

            foreach (var t in highestPriorityTargets)
            {
                var targetPos = Vector.Multiply(new Vector(t.targetBlob.CenterOfGravity.X, t.targetBlob.Rectangle.Y+t.targetBlob.Rectangle.Height*0.05), 1.0 / settings.ScaleFactor);
                var targetDelta = Vector.Subtract(targetPos, mouse);

                if (targetDelta.Length < shortestDistance)
                {
                    mouseDelta = targetDelta;
                    shortestDistance = targetDelta.Length;
                }
            }

            var formula = (1 - Math.Pow(1.4, -(mouseDelta.Length / 35))) * 2800;

            var origDelta = mouseDelta;
            mouseDelta.Normalize();

            var movement = Vector.Multiply(mouseDelta, formula / StaticInstance.FPS);

            if (double.IsNaN(movement.X) || double.IsNaN(movement.Y))
                return;

            //if (movement.Length > origDelta.Length)
             //   movement = origDelta;

            if (movement.Length < 1)
                return;
            
            Console.WriteLine(movement.Length);
            Mouse.Move(movement);
        }
    }
}
