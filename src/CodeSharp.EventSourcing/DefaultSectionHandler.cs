//Copyright (c) CodeSharp.  All rights reserved.

using System.Configuration;
using System.Xml;

namespace CodeSharp.EventSourcing
{
    public class DefaultSectionHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            return section;
        }
    }
}