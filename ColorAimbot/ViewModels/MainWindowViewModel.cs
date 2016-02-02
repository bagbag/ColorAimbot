using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ColorAimbot.Annotations;

namespace ColorAimbot.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private float _fps;
        private int _visibleTargets;
        private string _windowTitle;

        public float FPS
        {
            get { return (float)Math.Round(_fps,1); }
            set
            {
                if (value.Equals(_fps)) return;
                _fps = value;
                OnPropertyChanged();
            }
        }

        public int VisibleTargets
        {
            get { return _visibleTargets; }
            set
            {
                if (value == _visibleTargets) return;
                _visibleTargets = value;
                OnPropertyChanged();
            }
        }

        public string WindowTitle
        {
            get { return _windowTitle; }
            set
            {
                if (value == _windowTitle) return;
                _windowTitle = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
