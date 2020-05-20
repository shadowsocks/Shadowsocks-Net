using System;
using System.Windows.Markup;

namespace Shadowsocks.Net.UI.WPF.Infrastructure
{
    public class I18NExtension : MarkupExtension
    {
        private readonly string Key;

        public I18NExtension(string key)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Key;
        }
    }
}
