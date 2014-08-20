using System.Collections.Concurrent;
using System.IO;
using AForge.Imaging;
using ColorAimbot.Filter;
using ColorAimbot.Target;

namespace ColorAimbot.Helper
{
    public class Settings
    {
        private object lockobj = new object();

        private string path = "";
        private int searchWidth = 300;
        private int searchHeight = 300;
        private int minWidth = 10;
        private int minHeight = 10;
        private float scaleFactor = 0.5f;
        private TargetWindow targetWindow;
        private ConcurrentBag<TargetDescriptor> targetDescriptors;

        public int SearchWidth { get { lock (lockobj)return searchWidth; } set { lock (lockobj)searchWidth = value; } }

        public int SearchHeight { get { lock (lockobj) return searchHeight; } set { lock (lockobj)searchHeight = value; } }

        public int MinWidth { get { lock (lockobj)return minWidth; } set { lock (lockobj)minWidth = value; } }

        public int MinHeight { get { lock (lockobj)return minHeight; } set { lock (lockobj)minHeight = value; } }

        public float ScaleFactor { get { lock (lockobj)return scaleFactor; } set { lock (lockobj)scaleFactor = value; } }

        public TargetWindow TargetWindow { get { lock (lockobj) return targetWindow; } set { lock (lockobj)targetWindow = value; } }

        public ConcurrentBag<TargetDescriptor> TargetDescriptors { get { lock (lockobj)return targetDescriptors; } set { lock (lockobj)targetDescriptors = value; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">null to select appdata</param>
        public Settings(string path = null)
        {
            this.path = path;

            targetWindow = new TargetWindow(this);
            targetDescriptors = new ConcurrentBag<TargetDescriptor>();

            if (!File.Exists(path))
            {
                targetDescriptors.Add(new TargetDescriptor(new RGBFilter(new RGB(240, 0, 0), new RGB(255, 20, 20)), 255));
            }
        }

        ~Settings()
        {

        }
    }
}
