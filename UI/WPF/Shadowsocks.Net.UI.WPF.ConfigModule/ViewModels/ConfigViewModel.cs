using System.Collections.Generic;
using System.Windows.Controls;

using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

using Shadowsocks.Net.UI.Business;
using Shadowsocks.Net.UI.WPF.ConfigModule.Events;

namespace Shadowsocks.Net.UI.WPF.ConfigModule.ViewModels
{
    internal class ConfigViewModel : BindableBase
    {
        private readonly IEventAggregator _ea;

        public IList<string> EncryptionMethods { get; set; }

        private ServerConfig _serverConfig;

        public ServerConfig ServerConfig
        {
            get { return _serverConfig; }
            set
            {
                SetProperty(ref _serverConfig, value);
            }
        }

        private bool _passwordIsRevealed = false;

        public bool PasswordIsRevealed
        {
            get { return _passwordIsRevealed; }
            set
            {
                SetProperty(ref _passwordIsRevealed, value);
            }
        }

        public DelegateCommand<PasswordBox> PasswordChangedCommand { get; private set; }

        public ConfigViewModel(IEventAggregator ea)
        {
            _ea = ea;

            EncryptionMethods = new List<string>()
            {
                "AES", "GCM", "CHACHA20-IETF-POLY1305"
            };

            PasswordChangedCommand = new DelegateCommand<PasswordBox>(PasswordChanged);

            _ea.GetEvent<ConfigValidationEvent>().Subscribe(ValidationReceived);
        }

        private void ValidationReceived(bool isSuccess)
        {
            if (!isSuccess)
            {
                PasswordIsRevealed = true;
            }
        }

        private void PasswordChanged(PasswordBox box)
        {
            ServerConfig.Password = box.Password;
        }
    }
}