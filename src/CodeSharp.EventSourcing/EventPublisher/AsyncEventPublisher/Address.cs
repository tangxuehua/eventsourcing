//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Net;
using System.Runtime.Serialization;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 表示一个EndPoint的地址
    /// </summary>
    [Serializable]
    public class Address : ISerializable
    {
        private static AddressMode addressMode = AddressMode.Local;
        private static string defaultMachine = Environment.MachineName;

        /// <summary>
        /// Get the address of this endpoint.
        /// </summary>
        public static Address Local { get; private set; }

        /// <summary>
        /// Get the address of this endpoint.
        /// </summary>
        public static Address Self
        {
            get
            {
                return new Address("__self", "localhost");
            }
        }

        /// <summary>
        /// Get the address of this endpoint.
        /// </summary>
        public static Address Undefined
        {
            get
            {
                return new Address("", "");
            }
        }

        /// <summary>
        /// Sets the address of this endpoint.
        /// Will throw an exception if overwriting a previous value (but value will still be set).
        /// </summary>
        /// <param name="queue"></param>
        public static void InitializeLocalAddress(string address)
        {
            Local = Parse(address);
        }

        /// <summary>
        /// Sets the address mode, can only be done as long as the local address is not been initialized.By default the default machine equals Environment.MachineName
        /// </summary>
        /// <param name="machineName"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void OverrideDefaultMachine(string machineName)
        {
            defaultMachine = machineName;
        }

        /// <summary>
        /// Sets the name of the machine to be used when none is specified in the address.
        /// </summary>
        /// <param name="mode"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void InitializeAddressMode(AddressMode mode)
        {
            addressMode = mode;
        }

        /// <summary>
        /// Parses a string and returns an Address.
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        public static Address Parse(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                throw new InvalidOperationException("Invalid endpoint address specified");
            }

            var items = address.Split('@');

            var queue = items[0];
            var machine = defaultMachine;

            if (items.Length == 2)
            {
                if (items[1] != "." && items[1].ToLower() != "localhost" && items[1] != IPAddress.Loopback.ToString())
                {
                    machine = items[1];
                }
            }
            return new Address(queue, machine);
        }

        ///<summary>
        /// Instantiate a new Address for a known queue on a given machine.
        ///</summary>
        ///<param name="queueName"></param>
        ///<param name="machineName"></param>
        public Address(string queueName, string machineName)
        {
            Queue = queueName.ToLower();
            Machine = addressMode == AddressMode.Local ? machineName.ToLower() : machineName;
        }

        /// <summary>
        /// Deserializes an Address.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected Address(SerializationInfo info, StreamingContext context)
        {
            Queue = info.GetString("Queue");
            Machine = info.GetString("Machine");
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Queue", Queue);
            info.AddValue("Machine", Machine);
        }

        /// <summary>
        /// Creates a new Address whose Queue is derived from the Queue of the existing Address
        /// together with the provided qualifier. For example: queue.qualifier@machine
        /// </summary>
        /// <param name="qualifier"></param>
        /// <returns></returns>
        public Address SubScope(string qualifier)
        {
            return new Address(Queue + "." + qualifier, Machine);
        }

        /// <summary>
        /// Provides a hash code of the Address.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((Queue != null ? Queue.GetHashCode() : 0) * 397) ^ (Machine != null ? Machine.GetHashCode() : 0);
            }
        }

        /// <summary>
        /// Returns a string representation of the address.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Queue + "@" + Machine;
        }

        /// <summary>
        /// The (lowercase) name of the queue not including the name of the machine or location depending on the address mode.
        /// </summary>
        public string Queue { get; private set; }

        /// <summary>
        /// The (lowercase) name of the machine or the (normal) name of the location depending on the address mode.
        /// </summary>
        public string Machine { get; private set; }

        /// <summary>
        /// Overloading for the == for the class Address
        /// </summary>
        /// <param name="left">Left hand side of == operator</param>
        /// <param name="right">Right hand side of == operator</param>
        /// <returns>true if the LHS is equal to RHS</returns>
        public static bool operator ==(Address left, Address right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Overloading for the != for the class Address
        /// </summary>
        /// <param name="left">Left hand side of != operator</param>
        /// <param name="right">Right hand side of != operator</param>
        /// <returns>true if the LHS is not equal to RHS</returns>
        public static bool operator !=(Address left, Address right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Address)) return false;
            return Equals((Address)obj);
        }

        /// <summary>
        /// Check this is equal to other Address
        /// </summary>
        /// <param name="other">refrence addressed to be checked with this</param>
        /// <returns>true if this is equal to other</returns>
        public bool Equals(Address other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Queue, Queue) && Equals(other.Machine, Machine);
        }
    }

    /// <summary>
    /// Determines how the azure location behaves
    /// </summary>
    public enum AddressMode
    {
        /// <summary>
        /// Addressing behavior is confirm to local queueing policies, eg. MSMQ
        /// </summary>
        Local,
        /// <summary>
        /// Addressing behavior is confirm to remote queueing policies, eg. Azure
        /// </summary>
        Remote
    }
}
