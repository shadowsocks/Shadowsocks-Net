using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

using MaterialDesignThemes.Wpf;

using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

using Shadowsocks.Net.UI.Business;
using Shadowsocks.Net.UI.WPF.ConfigModule.Events;

namespace Shadowsocks.Net.UI.WPF.ConfigModule.ViewModels
{
    public class ConfigDialogViewModel : BindableBase
    {
        private readonly IEventAggregator _ea;

        private ServerConfig _serverConfig;

        public ServerConfig ServerConfig
        {
            get { return _serverConfig; }
            set
            {
                ServerConfigClone = value.Clone() as ServerConfig;
                SetProperty(ref _serverConfig, value);
            }
        }

        public ServerConfig ServerConfigClone { get; private set; }

        public DelegateCommand<ContentControl> OKCommand { get; private set; }

        public DelegateCommand<ContentControl> ApplyCommand { get; private set; }

        public ConfigDialogViewModel(IEventAggregator ea)
        {
            _ea = ea;

            OKCommand = new DelegateCommand<ContentControl>(OKReceived);
            ApplyCommand = new DelegateCommand<ContentControl>(ApplyReceived);
        }

        private void OKReceived(ContentControl control)
        {
            if (!CheckContent(control))
            {
                DialogHost.CloseDialogCommand.Execute(null, Application.Current.MainWindow.FindName("RootDialog") as DialogHost);
            }
        }

        private void ApplyReceived(ContentControl control)
        {
            CheckContent(control);
        }

        private bool CheckContent(ContentControl control)
        {
            var content = control.Content as DependencyObject;

            var HasError = false;

            foreach (var tb in FindVisualChildren<TextBox>(content))
            {
                var be = tb.GetBindingExpression(TextBox.TextProperty);

                SetHasError(ref HasError, be);
            }

            foreach (var comboBox in FindVisualChildren<ComboBox>(content))
            {
                var be = comboBox.GetBindingExpression(ComboBox.SelectedValueProperty);

                SetHasError(ref HasError, be);
            }

            if (!HasError)
            {
                ServerConfig.UpdateProperty(ServerConfigClone);
            }
            else
            {
                _ea.GetEvent<ConfigValidationEvent>().Publish(false);
            }

            return HasError;
        }

        private void SetHasError(ref bool hasError, BindingExpression be)
        {
            if (be == null) return;

            be.UpdateSource();

            hasError = hasError == false ? be.HasValidationError : hasError;
        }

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}