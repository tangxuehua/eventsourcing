//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Messaging;
using System.Security.Principal;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// Msmq implementation of message transport.
    /// </summary>
    public class MsmqMessageTransport : IMessageTransport
    {
        private MessageQueue _messageQueue;
        private static readonly ILogger _logger = ObjectContainer.Resolve<ILoggerFactory>().Create("EventSourcing.MsmqMessageTransport");

        public bool PurgeOnStartup { get; set; }
        public int SecondsToWaitForMessage { get; set; }

        public MsmqMessageTransport()
        {
            PurgeOnStartup = false;
            SecondsToWaitForMessage = 1;
        }

        void IMessageTransport.Init(Address address)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }

            var machine = address.Machine;

            if (machine.ToLower() != Environment.MachineName.ToLower())
            {
                throw new InvalidOperationException(
                    string.Format("Input queue [{0}] must be on the same machine as this process [{1}].",
                    address, Environment.MachineName.ToLower()));
            }

            var fullPath = MsmqUtilities.GetFullPathWithoutPrefix(address);
            if (MessageQueue.Exists(fullPath))
            {
                _messageQueue = new MessageQueue(fullPath);
            }
            else
            {
                _messageQueue = MessageQueue.Create(fullPath);
            }

            var mpf = new MessagePropertyFilter();
            mpf.SetAll();
            _messageQueue.MessageReadPropertyFilter = mpf;

            if (PurgeOnStartup)
            {
                _messageQueue.Purge();
            }
        }
        void IMessageTransport.SendMessage(Message message, Address targetAddress)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            if (targetAddress == null)
            {
                throw new ArgumentNullException("targetAddress");
            }

            var queuePath = MsmqUtilities.GetFullPath(targetAddress);
            try
            {
                using (var messageQueue = new MessageQueue(queuePath, QueueAccessMode.SendAndReceive))
                {
                    var msmqMessage = MsmqUtilities.Convert(message);
                    messageQueue.Send(msmqMessage, MessageQueueTransactionType.Automatic);
                    message.Id = msmqMessage.Id;
                }
            }
            catch (MessageQueueException ex)
            {
                if (ex.MessageQueueErrorCode == MessageQueueErrorCode.QueueNotFound)
                {
                    _logger.Error(string.Format("消息队列未找到: [{0}]", targetAddress), ex);
                    throw;
                }
                else
                {
                    _logger.Error(string.Format("发送消息到队列时遇到异常，队列地址：{0}", targetAddress), ex);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("发送消息到队列时遇到异常，队列地址：{0}", targetAddress), ex);
                throw;
            }
        }
        Message IMessageTransport.Receive()
        {
            try
            {
                var waitSesonds = TimeSpan.FromSeconds(SecondsToWaitForMessage);
                if (_messageQueue.Peek(waitSesonds) != null)
                {
                    var message = _messageQueue.Receive(waitSesonds, MessageQueueTransactionType.Automatic);
                    if (message != null)
                    {
                        return MsmqUtilities.Convert(message);
                    }
                }
                return null;
            }
            catch (MessageQueueException ex)
            {
                if (ex.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                {
                    return null;
                }

                if (ex.MessageQueueErrorCode == MessageQueueErrorCode.AccessDenied)
                {
                    string errorException = string.Format(
                        "Do not have permission to access queue [{0}]. Make sure that the current user [{1}] has permission to Send, Receive, and Peek from this queue.",
                        _messageQueue.QueueName,
                        WindowsIdentity.GetCurrent() != null ? WindowsIdentity.GetCurrent().Name : "unknown user");
                    _logger.Fatal(errorException);
                    throw new InvalidOperationException(errorException, ex);
                }

                _logger.Error(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("接收队列消息时遇到异常，队列地址：{0}", _messageQueue.QueueName), ex);
                throw;
            }
        }
    }
}