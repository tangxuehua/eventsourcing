//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections.Generic;

namespace CodeSharp.EventSourcing
{
    [Serializable]
    public class Message
    {
        /// <summary>
        /// Gets/sets the identifier of this message.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets/sets the identifier that is copied to <see cref="CorrelationId"/>.
        /// </summary>
        public string IdForCorrelation { get; set; }

        /// <summary>
        /// Gets/sets the uniqe identifier of another message bundle
        /// this message bundle is associated with.
        /// </summary>
        public string CorrelationId { get; set; }

        /// <summary>
        /// Gets/sets the reply-to address of the message bundle - replaces 'ReturnAddress'.
        /// </summary>
        public Address ReplyToAddress { get; set; }

        /// <summary>
        /// Gets/sets whether or not the message is supposed to be guaranteed deliverable.
        /// </summary>
        public bool Recoverable { get; set; }

        /// <summary>
        /// Indicates to the infrastructure the message intent (publish, or regular send).
        /// </summary>
        public MessageIntentEnum MessageIntent { get; set; }

        private TimeSpan timeToBeReceived = TimeSpan.MaxValue;

        /// <summary>
        /// Gets/sets the maximum time limit in which the message bundle must be received.
        /// </summary>
        public TimeSpan TimeToBeReceived
        {
            get { return timeToBeReceived; }
            set { timeToBeReceived = value; }
        }

        /// <summary>
        /// Gets/sets other applicative out-of-band information.
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Gets/sets a byte array to the body content of the message
        /// </summary>
        public byte[] Body { get; set; }
    }

    /// <summary>
    /// COntains transport message headers
    /// </summary>
    public static class TransportHeaderKeys
    {
        /// <summary>
        /// Header key for setting/getting the ID of the message as it was when it failed processing.
        /// </summary>
        public const string OriginalId = "EventSourcing.AsyncEventPublisher.OriginalId";

        /// <summary>
        /// Used for correlation id message.
        /// </summary>
        public const string IdForCorrelation = "CorrId";

        /// <summary>
        /// Used for deserialize a message.
        /// </summary>
        public const string MessageFullTypeName = "MessageFullTypeName";

        /// <summary>
        /// Return OriginalId if present. If not return Transport message Id.
        /// </summary>
        /// <param name="transportMessage"></param>
        /// <returns></returns>
        public static string GetOriginalId(this Message message)
        {
            if (message.Headers.ContainsKey(OriginalId) && (!string.IsNullOrWhiteSpace(message.Headers[OriginalId])))
            {
                return message.Headers[OriginalId];
            }
            return message.Id;
        }
        /// <summary>
        /// Returns IdForCorrelation if not null, otherwise, return Transport message Id.
        /// </summary>
        /// <param name="transportMessage"></param>
        /// <returns></returns>
        public static string GetIdForCorrelation(this Message message)
        {
            if (message.Headers.ContainsKey(IdForCorrelation) && (!string.IsNullOrWhiteSpace(message.Headers[IdForCorrelation])))
            {
                return message.Headers[IdForCorrelation];
            }
            return message.Id;
        }
    }

    ///<summary>
    /// Enumeration defining different kinds of message intent like Send and Publish.
    ///</summary>
    public enum MessageIntentEnum
    {
        /// <summary>
        /// Initialization
        /// </summary>
        Init = 0,

        ///<summary>
        /// Regular point-to-point send
        ///</summary>
        Send = 1,

        ///<summary>
        /// Publish, not a regular point-to-point send
        ///</summary>
        Publish = 2,

        /// <summary>
        /// Subscribe
        /// </summary>
        Subscribe = 3,

        /// <summary>
        /// Unsubscribe
        /// </summary>
        Unsubscribe = 4,
    }
}
