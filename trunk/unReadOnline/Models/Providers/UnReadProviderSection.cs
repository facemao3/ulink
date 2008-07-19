using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace unReadOnline.Models.Providers
{
    /// <summary>
    /// A configuration section for web.config.
    /// </summary>
    /// <remarks>
    /// In the config section you can specify the provider you 
    /// want to use for ingNote.NET.
    /// </remarks>
    public class UnReadProviderSection : ConfigurationSection
    {

        /// <summary>
        /// 已注册的 provider 集合.
        /// </summary>
        [ConfigurationProperty("providers")]
        public ProviderSettingsCollection Providers
        {
            get { return (ProviderSettingsCollection)base["providers"]; }
        }

        /// <summary>
        /// 默认provider的名称
        /// </summary>
        [StringValidator(MinLength = 1)]
        [ConfigurationProperty("defaultProvider", DefaultValue = "XmlProvider")]
        public string DefaultProvider
        {
            get { return (string)base["defaultProvider"]; }
            set { base["defaultProvider"] = value; }
        }
    }
}
