using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Shadowsocks.Net.UI.Business
{
    public class ServerConfig : INotifyPropertyChanged, ICloneable
    {
        #region Properties

        private string _host;

        public string Host
        {
            get { return _host; }
            set
            {
                SetProperty(ref _host, value);
            }
        }

        private ushort _port;

        public ushort Port
        {
            get { return _port; }
            set
            {
                SetProperty(ref _port, value);
            }
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set
            {
                SetProperty(ref _password, value);
            }
        }

        public string _cipherName;

        public string CipherName
        {
            get { return _cipherName; }
            set
            {
                SetProperty(ref _cipherName, value);
            }
        }

        private byte _timeout;

        public byte Timeout
        {
            get { return _timeout; }
            set
            {
                SetProperty(ref _timeout, value);
            }
        }

        private string _plugin;

        public string Plugin
        {
            get { return _plugin; }
            set
            {
                SetProperty(ref _plugin, value);
            }
        }

        private string _pluginOptions;

        public string PluginOptions
        {
            get { return _pluginOptions; }
            set
            {
                SetProperty(ref _pluginOptions, value);
            }
        }

        private string _pluginArgs;

        public string PluginArgs
        {
            get { return _pluginArgs; }
            set
            {
                SetProperty(ref _pluginArgs, value);
            }
        }

        private string _remark;

        public string Remark
        {
            get { return _remark; }
            set
            {
                SetProperty(ref _remark, value);
            }
        }

        #endregion Properties

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void UpdateProperty(ServerConfig newConfig)
        {
            Host = newConfig.Host;
            Port = newConfig.Port;
            Password = newConfig.Password;
            CipherName = newConfig.CipherName;
            Timeout = newConfig.Timeout;
            Plugin = newConfig.Plugin;
            PluginOptions = newConfig.PluginOptions;
            PluginArgs = newConfig.PluginArgs;
            Remark = newConfig.Remark;
        }

        protected void OnPropertyChanged([CallerMemberName]string propertyname = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

        #endregion INotifyPropertyChanged

        private void SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(storage, value))
                return;

            storage = value;
            OnPropertyChanged(propertyName);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}