using System;
using System.Reflection;
using System.Windows;

using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;

using Shadowsocks.Net.UI.WPF.Views;

namespace Shadowsocks.Net.UI.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(ViewTypeToViewModelTypeResolver);
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ConfigModule.ModuleImpl>();
        }

        private static Type ViewTypeToViewModelTypeResolver(Type viewType)
        {
            var viewName = viewType.FullName.Replace(".Controls.", ".ViewModels.").Replace(".Views.", ".ViewModels.");
            var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
            var viewModelName = $"{viewName}ViewModel, {viewAssemblyName}";

            return Type.GetType(viewModelName);
        }
    }
}