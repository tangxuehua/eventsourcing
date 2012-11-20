//Copyright (c) CodeSharp.  All rights reserved.

namespace CodeSharp.EventSourcing
{
    public interface IConfigurationInitializer
    {
        void Initialize(Configuration configuration);
    }
}