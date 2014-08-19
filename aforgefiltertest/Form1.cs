using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Imaging;
using AForge.Imaging.Filters;
using Amib.Threading;
using Image = System.Drawing.Image;
using Timer = System.Windows.Forms.Timer;

namespace aforgefiltertest
{
    public partial class Form1 : Form
    {
        private readonly RGBFilter rgbfilter = new RGBFilter(new RGB(240, 0, 0), new RGB(255, 20, 20));

        private int scaleFactor = 2;

        public Form1()
        {
            InitializeComponent();

            pictureBox1.Image = new Bitmap(50, 50);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Thread screenshotThread = new Thread(GetScreenshots);
            screenshotThread.IsBackground = true;
            screenshotThread.Start();

            Thread scaleThread = new Thread(ScaleScreenhots);
            scaleThread.IsBackground = true;
            scaleThread.Start();

            Thread filterthread = new Thread(filter);
            filterthread.IsBackground = true;
            filterthread.Start();

            Thread getTargetsthread = new Thread(GetTargets);
            getTargetsthread.IsBackground = true;
            getTargetsthread.Start();

            Thread drawUIthread = new Thread(DrawUI);
            drawUIthread.IsBackground = true;
            drawUIthread.Start();
        }

        private void GetScreenshots()
        {
            Bitmap picture = new Bitmap(@"C:\Users\Patrick\Desktop\l4d.png");

            while (true)
            {
                if (scalingQueue.Count < 5)
                {
                    //Bitmap screenshot = picture.Clone() as Bitmap;
                    IntPtr windowHandle = User32.FindWindowByCaption(IntPtr.Zero, "Left 4 Dead");
                    RECT windowRect;
                    User32.GetWindowRect(new HandleRef(this, windowHandle), out windowRect);

                    Bitmap screenshot = new Bitmap(windowRect.Right - windowRect.Left, windowRect.Bottom - windowRect.Top);
                    Graphics g = Graphics.FromImage(screenshot);
                    g.CopyFromScreen(windowRect.Left, windowRect.Top, 0, 0, screenshot.Size);
                    g.Dispose();

                    //Invoke(new MethodInvoker(() => pictureBox1.Image = screenshot));

                    lock (scalingQueuelockobj)
                        scalingQueue.Enqueue(screenshot);
                }
                else
                    Thread.Sleep(1);
            }
        }

        private volatile object scalingQueuelockobj = new object();
        private volatile Queue<Bitmap> scalingQueue = new Queue<Bitmap>();

        private void ScaleScreenhots()
        {
            while (true)
            {
                while (true)
                {
                    lock (scalingQueuelockobj)
                        if (scalingQueue.Count > 0)
                            break;

                    Thread.Sleep(1);
                }

                Bitmap screenshot;

                lock (scalingQueuelockobj)
                    screenshot = scalingQueue.Dequeue();

                var scaledWidth = (int) (screenshot.Width * 1f/scaleFactor);
                var scaledHeight = (int)(screenshot.Height * 1f / scaleFactor);

                Bitmap scaledBitmap;
                if (scaleFactor > 1)
                {
                    scaledBitmap = new Bitmap(scaledWidth, scaledHeight);

                    Graphics scaler = Graphics.FromImage(scaledBitmap);
                    scaler.InterpolationMode = InterpolationMode.NearestNeighbor;
                    scaler.DrawImage(screenshot, 0, 0, scaledWidth, scaledHeight);

                    StaticInstance.SmartThreadPool.QueueWorkItem((s) => s.Dispose(), screenshot);
                    StaticInstance.SmartThreadPool.QueueWorkItem((s) => s.Dispose(), scaler);
                }
                else 
                    scaledBitmap = screenshot;

                var bitmapData = scaledBitmap.LockBits(new Rectangle(0, 0, scaledWidth, scaledHeight), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                lock (screenshotsQueueLockobj)
                    screenshotsQueue.Enqueue(new KeyValuePair<Bitmap, BitmapData>(scaledBitmap, bitmapData));
            }
        }


        private volatile object screenshotsQueueLockobj = new object();
        private volatile Queue<KeyValuePair<Bitmap, BitmapData>> screenshotsQueue = new Queue<KeyValuePair<Bitmap, BitmapData>>();

        private void filter()
        {
            while (true)
            {
                while (true)
                {
                    lock (screenshotsQueueLockobj)
                        if (screenshotsQueue.Count > 0)
                            break;

                    Thread.Sleep(1);
                }

                Bitmap screenshot;
                BitmapData bitmapData;
                KeyValuePair<Bitmap, BitmapData> pair;

                lock (screenshotsQueueLockobj)
                    pair = screenshotsQueue.Dequeue();

                screenshot = pair.Key;
                bitmapData = pair.Value;

                ColorFilter.FilterImage(ref screenshot, ref bitmapData, rgbfilter, 255);

                lock (filteredImageQueueLockobj)
                    filteredImageQueue.Enqueue(new KeyValuePair<Bitmap, BitmapData>(screenshot, bitmapData));
            }
        }

        private volatile object filteredImageQueueLockobj = new object();
        private Queue<KeyValuePair<Bitmap, BitmapData>> filteredImageQueue = new Queue<KeyValuePair<Bitmap, BitmapData>>();

        private void GetTargets()
        {
            while (true)
            {
                while (true)
                {
                    lock (filteredImageQueueLockobj)
                        if (filteredImageQueue.Count > 0)
                            break;

                    Thread.Sleep(1);
                }

                Bitmap screenshot;
                BitmapData bitmapData;
                KeyValuePair<Bitmap, BitmapData> pair;

                lock (filteredImageQueueLockobj)
                    pair = filteredImageQueue.Dequeue();

                screenshot = pair.Key;
                bitmapData = pair.Value;

                BlobCounter targetCounter = new BlobCounter();
                targetCounter.CoupledSizeFiltering = true;
                targetCounter.BackgroundThreshold = Color.Black;
                targetCounter.FilterBlobs = true;
                targetCounter.MinWidth = 15 / scaleFactor;
                targetCounter.MinHeight = 15 / scaleFactor;
                targetCounter.ObjectsOrder = ObjectsOrder.Area;
                targetCounter.ProcessImage(bitmapData);

                Blob[] targets = targetCounter.GetObjectsInformation();

                lock (targetsQueueLockobj)
                    targetsQueue.Enqueue(new KeyValuePair<KeyValuePair<Bitmap, BitmapData>, Blob[]>(new KeyValuePair<Bitmap, BitmapData>(screenshot, bitmapData), targets));
            }
        }

        private volatile object targetsQueueLockobj = new object();
        private Queue<KeyValuePair<KeyValuePair<Bitmap, BitmapData>, Blob[]>> targetsQueue = new Queue<KeyValuePair<KeyValuePair<Bitmap, BitmapData>, Blob[]>>();

        private volatile object isworkinglock = new object();
        private volatile bool isworking = false;

        private void DrawUI()
        {
            Stopwatch sw = Stopwatch.StartNew();
            int frames = 0;
            while (true)
            {
                if (sw.Elapsed.TotalSeconds >= 1)
                {
                    Console.WriteLine (frames);
                    frames = 0;
                    sw.Restart();
                }
                frames++;

                while (true)
                {
                    lock (targetsQueueLockobj)
                        if (targetsQueue.Count > 0)
                            break;

                    Thread.Sleep(1);
                }

                Bitmap screenshot;
                BitmapData bitmapData;
                Blob[] targets;
                KeyValuePair<KeyValuePair<Bitmap, BitmapData>, Blob[]> queueData;

                lock (targetsQueue)
                    queueData = targetsQueue.Dequeue();

                screenshot = queueData.Key.Key;
                bitmapData = queueData.Key.Value;
                targets = queueData.Value;

                lock (isworkinglock)
                {
                    if (!isworking)
                    {
                        lock (isworkinglock)
                            isworking = true;
                        StaticInstance.SmartThreadPool.QueueWorkItem((s, b) =>
                                                                     {
                                                                         s.UnlockBits(b);
                                                                         MemoryStream ms = new MemoryStream();
                                                                         s.Save(ms,ImageFormat.Png);
                                                                         s.Dispose();
                                                                         s = Bitmap.FromStream(ms) as Bitmap;
                                                                         Graphics g = Graphics.FromImage(s);

                                                                         foreach (Blob target in targets)
                                                                         {
                                                                             g.DrawRectangle(Pens.DeepSkyBlue, target.Rectangle);
                                                                             g.DrawEllipse(Pens.Red, target.CenterOfGravity.X-4, target.CenterOfGravity.Y-4, 8, 8);
                                                                         }

                                                                         Image oldImage = null;
                                                                         Invoke(new MethodInvoker(() => oldImage = pictureBox1.Image));
                                                                         Invoke(new MethodInvoker(() => pictureBox1.Image = s));
                                                                         oldImage.Dispose();
                                                                         lock (isworkinglock)
                                                                             isworking = false;
                                                                     }, screenshot, bitmapData);
                    }
                    else
                    {
                        StaticInstance.SmartThreadPool.QueueWorkItem((s, b) =>
                                                                     {
                                                                         s.UnlockBits(bitmapData);
                                                                         s.Dispose();
                                                                     }, screenshot, bitmapData);
                    }
                }
            }
        }
    }
}
