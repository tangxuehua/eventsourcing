//Copyright (c) CodeSharp.  All rights reserved.

using System.IO;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// Interface used for serializing and deserializing messages.
    /// </summary>
    public interface IMessageSerializer
    {
        /// <summary>
        /// Serializes the given message into the given stream.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="stream"></param>
        void Serialize(object message, Stream stream);

        /// <summary>
        /// Deserializes a raw message from the given message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        object Deserialize(Message message);
    }
}