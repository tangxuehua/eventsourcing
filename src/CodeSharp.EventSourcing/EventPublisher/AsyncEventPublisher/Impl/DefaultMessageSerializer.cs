//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// Binary implementation of the message serializer.
    /// </summary>
    public class DefaultMessageSerializer : IMessageSerializer
    {
        private ISerializer _serializer;
        private readonly BinaryFormatter _binaryFormatter = new BinaryFormatter();

        public DefaultMessageSerializer(ISerializer serializer)
        {
            _serializer = serializer;
        }

        /// <summary>
        /// Serializes the given message to the given stream.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="stream"></param>
        public void Serialize(object message, Stream stream)
        {
            var stringValue = _serializer.Serialize(message);
            _binaryFormatter.Serialize(stream, new List<object> { stringValue });
        }

        /// <summary>
        /// Deserializes a raw message from the given message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public object Deserialize(Message message)
        {
            if (message == null || message.Body == null || message.Body.Length == 0)
            {
                return null;
            }

            var stream = new MemoryStream(message.Body);
            var body = _binaryFormatter.Deserialize(stream) as List<object>;

            if (body == null || body.Count == 0)
            {
                return null;
            }

            var stringValue = body.First() as string;
            if (string.IsNullOrEmpty(stringValue))
            {
                return null;
            }

            var messageTypeFullName = message.Headers[TransportHeaderKeys.MessageFullTypeName];
            if (string.IsNullOrEmpty(messageTypeFullName))
            {
                return null;
            }

            var messageType = Type.GetType(messageTypeFullName);

            return _serializer.Deserialize(stringValue, messageType);
        }
    }
}