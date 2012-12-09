//Copyright (c) CodeSharp.  All rights reserved.

using System.Configuration;
using CodeSharp.EventSourcing;

namespace EventSourcing.Sample.CustomizeConfigurationInitializer
{
    public class SimpleConfigurationInitializer : IConfigurationInitializer
    {
        public void Initialize(CodeSharp.EventSourcing.Configuration configuration)
        {
            configuration.SetSettings(ConfigurationManager.AppSettings);
            configuration.Settings["log4net"] = ConfigurationManager.GetSection("log4net");
        }
    }
}