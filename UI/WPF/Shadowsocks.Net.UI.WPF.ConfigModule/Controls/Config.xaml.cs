using System.ComponentModel;
using System.Globalization;
using System.Windows.Controls;

using Prism.Common;
using Prism.Regions;

using Shadowsocks.Net.UI.Business;
using Shadowsocks.Net.UI.WPF.ConfigModule.ViewModels;

namespace Shadowsocks.Net.UI.WPF.ConfigModule.Controls
{
    /// <summary>
    /// Interaction logic for Config.xaml
    /// </summary>
    public partial class Config : UserControl
    {
        public Config()
        {
            InitializeComponent();

            RegionContext.GetObservableContext(this).PropertyChanged += ServerConfig_PropertyChanged;
        }

        private void ServerConfig_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var context = sender as ObservableObject<object>;

            (DataContext as ConfigViewModel).ServerConfig = context.Value as ServerConfig;
        }
    }

    public class HostValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            // TODO: What's next? We should try to check if the host is valid
            return string.IsNullOrWhiteSpace(value as string) ? new ValidationResult(false, "Invalid host address.") : ValidationResult.ValidResult;
        }
    }

    public class PasswordValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return string.IsNullOrEmpty(value as string) ? new ValidationResult(false, "Please enter the password.") : ValidationResult.ValidResult;
        }
    }

    public class EncryptionValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return string.IsNullOrWhiteSpace(value as string) ? new ValidationResult(false, "Please select a valid value.") : ValidationResult.ValidResult;
        }
    }

    public class PortValidationRule : ValidationRule
    {
        public ushort Min { get; set; }
        public ushort Max { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (!string.IsNullOrWhiteSpace(value as string))
            {
                if (ushort.TryParse(value as string, out var port))
                {
                    if (port >= Min && port <= Max)
                    {
                        return ValidationResult.ValidResult;
                    }
                }

                return new ValidationResult(false, "Invalid port.");
            }

            return new ValidationResult(false, "Please enter the port.");
        }
    }

    public class TimeoutValidationRule : ValidationRule
    {
        public byte Min { get; set; }
        public byte Max { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (!string.IsNullOrWhiteSpace(value as string))
            {
                if (byte.TryParse(value as string, out var timeout))
                {
                    if (timeout >= Min && timeout <= Max)
                    {
                        return ValidationResult.ValidResult;
                    }
                }

                return new ValidationResult(false, "Invalid timeout.");
            }

            return new ValidationResult(false, "Please enter the timeout.");
        }
    }
}