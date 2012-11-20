//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.IO;
using log4net;
using log4net.Config;

namespace CodeSharp.EventSourcing
{
    public class Log4NetLoggerFactory : ILoggerFactory
    {
        public Log4NetLoggerFactory()
        {
            var configFile = Configuration.Instance.Properties["log4netConfigFile"];
            XmlConfigurator.ConfigureAndWatch(new FileInfo(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFile)));
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
