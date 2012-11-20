//Copyright (c) CodeSharp.  All rights reserved.

using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace CodeSharp.EventSourcing
{
    public class ConfigurationSectionHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            var settings = new Dictionary<string, string>();

            foreach (XmlNode childNode in section)
            {
                if (childNode.NodeType == XmlNodeType.Element)
                {
                    settings.Add(childNode.Attributes["key"].Value, childNode.Attributes["value"].Value);
                }
            }

            return settings;
        }
    }
}