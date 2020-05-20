using System.Collections.ObjectModel;
using System.Windows.Controls;

using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

using Shadowsocks.Net.UI.Business;
using Shadowsocks.Net.UI.WPF.ConfigModule.Views;

namespace Shadowsocks.Net.UI.WPF.ConfigModule.ViewModels
{
    public class ConfigListViewModel : BindableBase
    {
        private readonly IEventAggregator _ea;

        private ObservableCollection<ServerConfig> _configs;

        public ObservableCollection<ServerConfig> Configs
        {
            get { return _configs; }
            set
            {
                SetProperty(ref _configs, value);
            }
        }

        public DelegateCommand<WrapPanel> ConfigsPanelInitCommand { get; private set; }

        public ConfigListViewModel(IEventAggregator ea)

        {
            _ea = ea;

            ConfigsPanelInitCommand = new DelegateCommand<WrapPanel>(ConfigsInit);
        }

        private void ConfigsInit(WrapPanel panel)
        {
            foreach (var config in _configs)
            {
                var node = new ConfigListNode()
                {
                    DataContext = new ConfigListNodeViewModel(_ea)
                    {
                        ServerConfig = config
                    }
                };

                panel.Children.Add(node);
            }
        }
    }
}