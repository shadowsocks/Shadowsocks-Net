using System.Windows;

using Prism.Regions;

namespace Shadowsocks.Net.UI.WPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IRegionManager RegionManager;

        public MainWindow(IRegionManager regionManager)
        {
            InitializeComponent();

            RegionManager = regionManager;
        }
    }
}