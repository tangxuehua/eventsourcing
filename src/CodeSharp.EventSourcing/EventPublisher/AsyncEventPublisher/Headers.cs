//Copyright (c) CodeSharp.  All rights reserved.

using System;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// Static class containing headers used by AsyncMessageBus.
    /// </summary>
    public static class Headers
    {
        /// <summary>
        /// Header for retrieving from which Http endpoint the message arrived.
        /// </summary>
        public const string HttpFrom = "AsyncMessageBus.From";

        /// <summary>
        /// Header for specifying to which Http endpoint the message should be delivered.
        /// </summary>
        public const string HttpTo = "AsyncMessageBus.To";

        /// <summary>
        /// Header for specifying to which queue behind the http gateway should the message be delivered.
        /// This header is considered an applicative header.
        /// </summary>
        public const string RouteTo = "AsyncMessageBus.Header.RouteTo";

        /// <summary>
        /// Header for specifying to which sites the gateway should send the message. For multiple
        /// sites a comma separated list can be used
        /// This header is considered an applicative header.
        /// </summary>
        public const string DestinationSites = "AsyncMessageBus.DestinationSites";

        /// <summary>
        /// Header for specifying the key for the site where this message originated. 
        /// This header is considered an applicative header.
        /// </summary>
        public const string OriginatingSite = "AsyncMessageBus.OriginatingSite";

        /// <summary>
        /// Header for time when a message expires in the timeout manager
        /// This header is considered an applicative header.
        /// </summary>
        public const string Expire = "AsyncMessageBus.Timeout.Expire";

        /// <summary>
        /// Header containing the id of the saga instance the sent the message
        /// This header is considered an applicative header.
        /// </summary>
        public const string SagaId = "AsyncMessageBus.SagaId";

        /// <summary>
        /// Header telling the timeout manager to clear previous timeouts
        /// This header is considered an applicative header.
        /// </summary>
        public const string ClearTimeouts = "AsyncMessageBus.ClearTimeouts";


        /// <summary>
        /// Prefix included on the wire when sending applicative headers.
        /// </summary>
        public const string HeaderName = "Header";

        /// <summary>
        /// Header containing the windows identity name
        /// </summary>
        public const string WindowsIdentityName = "WinIdName";

        /// <summary>
        /// Header telling the AsyncMessageBus Version (beginning AsyncMessageBus V3.0.1).
        /// </summary>
        public const string NServiceBusVersion = "AsyncMessageBus.Version";

        /// <summary>
        /// Used in a header when doing a callback
        /// </summary>
        public const string ReturnMessageErrorCodeHeader = "AsyncMessageBus.ReturnMessage.ErrorCode";

        /// <summary>
        /// Header that tells if this transport message is a control message
        /// </summary>
        public const string ControlMessageHeader = "AsyncMessageBus.ControlMessage";

        /// <summary>
        /// Type of the saga that this message is targeted for
        /// </summary>
        public const string SagaType = "AsyncMessageBus.SagaType";

        /// <summary>
        /// Id of the saga that sent this message
        /// </summary>
        public const string OriginatingSagaId = "AsyncMessageBus.OriginatingSagaId";

        /// <summary>
        /// Type of the saga that sent this message
        /// </summary>
        public const string OriginatingSagaType = "AsyncMessageBus.OriginatingSagaType";

        /// <summary>
        /// The number of retries that has been performed for this message
        /// </summary>
        public const string Retries = "AsyncMessageBus.Retries";
    }
}
