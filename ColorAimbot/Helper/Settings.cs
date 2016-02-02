using System.Collections.Concurrent;
using System.IO;
using AForge.Imaging;
using ColorAimbot.Filter;
using ColorAimbot.Target;

namespace ColorAimbot.Helper
{
    public class Settings
    {
        private readonly object _lockobj = new object();

        private string _path;
        private int _searchWidth = 600;
        private int _searchHeight = 600;
        private int _minWidth = 8;
        private int _minHeight = 8;
        private float _scaleFactor = 1f;
        private TargetWindow _targetWindow;
        private ConcurrentBag<TargetDescriptor> _targetDescriptors;

        public int SearchWidth { get { lock (_lockobj)return _searchWidth; } set { lock (_lockobj)_searchWidth = value; } }
        public int SearchHeight { get { lock (_lockobj) return _searchHeight; } set { lock (_lockobj)_searchHeight = value; } }
        public int MinWidth { get { lock (_lockobj)return _minWidth; } set { lock (_lockobj)_minWidth = value; } }
        public int MinHeight { get { lock (_lockobj)return _minHeight; } set { lock (_lockobj)_minHeight = value; } }
        public float ScaleFactor { get { lock (_lockobj)return _scaleFactor; } set { lock (_lockobj)_scaleFactor = value; } }
        public TargetWindow TargetWindow { get { lock (_lockobj) return _targetWindow; } set { lock (_lockobj)_targetWindow = value; } }
        public ConcurrentBag<TargetDescriptor> TargetDescriptors { get { lock (_lockobj)return _targetDescriptors; } set { lock (_lockobj)_targetDescriptors = value; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">null to use defaults</param>
        public Settings(string path = null)
        {
            _path = path;
            _targetWindow = new TargetWindow(this);
            _targetDescriptors = new ConcurrentBag<TargetDescriptor>();
        }

        ~Settings()
        {

        }
    }
}
