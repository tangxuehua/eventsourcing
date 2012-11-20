//Copyright (c) CodeSharp.  All rights reserved.

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// Defines a transport to send and receive message.
    /// </summary>
    public interface IMessageTransport
    {
        /// <summary>
        /// Initialize the message transport.
        /// </summary>
        /// <param name="address">The address of the message source</param>
        void Init(Address address);
        /// <summary>
        /// Send a message to a target endpoint address.
        /// </summary>
        void SendMessage(Message message, Address targetAddress);
        /// <summary>
        /// Tries to receive a message from the address passed in Init method.
        /// </summary>
        /// <returns>The received message available. If no message is present null will be returned.</returns>
        Message Receive();
    }
}
