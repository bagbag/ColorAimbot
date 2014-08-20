using MahApps.Metro.Controls;

namespace ColorAimbot
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        Aimbot aimbot = new Aimbot();
        public MainWindow()
        {
            InitializeComponent();

            aimbot.Start();
        }
    }
}
