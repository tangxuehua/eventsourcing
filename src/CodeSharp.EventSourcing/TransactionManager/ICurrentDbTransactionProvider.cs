//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Data;

namespace CodeSharp.EventSourcing
{
    public interface ICurrentDbTransactionProvider
    {
        IDbTransaction CurrentTransaction { get; }
    }
}
