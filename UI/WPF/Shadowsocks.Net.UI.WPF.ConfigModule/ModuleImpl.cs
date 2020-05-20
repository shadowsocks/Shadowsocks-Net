using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

using Shadowsocks.Net.UI.WPF.ConfigModule.Controls;
using Shadowsocks.Net.UI.WPF.ConfigModule.Views;

namespace Shadowsocks.Net.UI.WPF.ConfigModule
{
    public class ModuleImpl : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();

            regionManager.RegisterViewWithRegion("ContentRegion", typeof(ConfigList));
            regionManager.RegisterViewWithRegion("ServerConfigRegion", typeof(Config));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }
}