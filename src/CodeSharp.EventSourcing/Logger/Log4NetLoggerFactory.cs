//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.IO;
using System.Xml;
using log4net;
using log4net.Config;

namespace CodeSharp.EventSourcing
{
    public class Log4NetLoggerFactory : ILoggerFactory
    {
        public Log4NetLoggerFactory()
        {
            var configFile = Configuration.Instance.Settings["log4netConfigFile"] as string;
            var file = new FileInfo(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFile));
            if (file.Exists)
            {
                XmlConfigurator.ConfigureAndWatch(file);
            }
            else
            {
                var log4netElement = (Configuration.Instance.Settings["log4net"] as XmlNode) as XmlElement;
                XmlConfigurator.Configure(log4netElement);
            }
        }

        ILogger ILoggerFactory.Create(string name)
        {
            return new Log4NetLogger(LogManager.GetLogger(name));
        }
        ILogger ILoggerFactory.Create(Type type)
        {
            return new Log4NetLogger(LogManager.GetLogger(type));
        }
    }
}
