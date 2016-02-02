using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using AForge.Imaging;
using ColorAimbot.Filter;
using ColorAimbot.Helper;
using ColorAimbot.Target;
using MahApps.Metro.Controls;

namespace ColorAimbot
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        readonly Aimbot _aimbot = new Aimbot();
        
        public MainWindow()
        {
            InitializeComponent();
            DataContext = StaticInstance._mainWindowViewModel;

            StaticInstance._mainWindowViewModel.PropertyChanged += MainWindowViewModelOnPropertyChanged;

            _aimbot.Settings.TargetWindow.UseWindowTitle("Counter");
            _aimbot.Settings.SearchWidth = 400;
            _aimbot.Settings.SearchHeight = 400;
            _aimbot.Settings.TargetDescriptors.Add(new TargetDescriptor(new RGBFilter(new RGB(220,0,0), new RGB(255,30,30)), 255));
            _aimbot.Start();
        }

        private void MainWindowViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            switch (propertyChangedEventArgs.PropertyName)
            {
                case "WindowTitle":
                    _aimbot.Settings.TargetWindow.UseWindowTitle(StaticInstance._mainWindowViewModel.WindowTitle);
                    break;
            }
        }

        enum TargetWindowType
        {
            Title,
            Foreground,
            Handle
        }
    }
}
