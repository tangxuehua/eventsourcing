//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Reflection;

namespace CodeSharp.EventSourcing
{
    public interface IAsyncEventHandlerProvider : IEventHandlerProvider<EventHandlerMetaData>
    {
    }
}
