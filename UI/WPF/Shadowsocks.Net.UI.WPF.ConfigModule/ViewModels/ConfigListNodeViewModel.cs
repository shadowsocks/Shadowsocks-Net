using System.Windows.Input;

using MaterialDesignThemes.Wpf;

using Prism.Events;
using Prism.Mvvm;

using Shadowsocks.Net.UI.Business;
using Shadowsocks.Net.UI.WPF.ConfigModule.Views;
using Shadowsocks.Net.UI.WPF.Infrastructure.Command;

namespace Shadowsocks.Net.UI.WPF.ConfigModule.ViewModels
{
    public class ConfigListNodeViewModel : BindableBase
    {
        private readonly IEventAggregator _ea;

        private ServerConfig _serverConfig;

        public ServerConfig ServerConfig
        {
            get { return _serverConfig; }
            set
            {
                SetProperty(ref _serverConfig, value);
            }
        }

        public ICommand OpenConfigDialogCommand => new AnotherCommandImplementation(OpenConfigDialog);

        public ConfigListNodeViewModel(IEventAggregator ea)
        {
            _ea = ea;
        }

        private async void OpenConfigDialog(object o)
        {
            var view = new ConfigDialog
            {
                DataContext = new ConfigDialogViewModel(_ea)
                {
                    ServerConfig = _serverConfig
                }
            };

            var result = await DialogHost.Show(view, "RootDialog", ConfigDialogClosingEventHandler);
        }

        private void ConfigDialogClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
        }
    }
}