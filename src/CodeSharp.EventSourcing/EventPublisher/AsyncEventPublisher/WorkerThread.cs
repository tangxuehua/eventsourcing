//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Threading;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// Represents a worker thread that will repeatedly execute a callback.
    /// </summary>
    public class WorkerThread
    {
        private readonly System.Action methodToRunInLoop;
        private readonly Thread thread;
        private volatile bool stopRequested;
        private readonly object lockObj = new object();
        private readonly static ILogger _logger = ObjectContainer.Resolve<ILoggerFactory>().Create("EventSourcing.WorkerThread");

        /// <summary>
        /// Initializes a new WorkerThread for the specified method to run.
        /// </summary>
        /// <param name="methodToRunInLoop">The delegate method to execute in a loop.</param>
        public WorkerThread(System.Action methodToRunInLoop)
        {
            this.methodToRunInLoop = methodToRunInLoop;
            thread = new Thread(Loop);
            thread.SetApartmentState(ApartmentState.MTA);
            thread.Name = string.Format("MessageReceiveWorker.{0}", thread.ManagedThreadId);
            thread.IsBackground = true;
        }

        /// <summary>
        /// Event raised when the worker thread has stopped.
        /// </summary>
        public event EventHandler Stopped;

        /// <summary>
        /// Starts the worker thread.
        /// </summary>
        public void Start()
        {
            if (!thread.IsAlive)
            {
                thread.Start();
            }
        }
        /// <summary>
        /// Stops the worker thread.
        /// </summary>
        public void Stop()
        {
            lock (lockObj)
            {
                stopRequested = true;
            }
        }

        /// <summary>
        /// Executes the delegate method until the <see cref="Stop"/>
        /// method is called.
        /// </summary>
        protected void Loop()
        {
            while (!StopRequested)
            {
                try
                {
                    methodToRunInLoop();
                }
                catch (Exception e)
                {
                    _logger.Error("Exception reached top level.", e);
                }
            }

            if (Stopped != null)
            {
                Stopped(this, null);
            }
        }
        /// <summary>
        /// Gets whether or not a stop request has been received.
        /// </summary>
        protected bool StopRequested
        {
            get
            {
                bool result;
                lock (lockObj)
                {
                    result = stopRequested;
                }
                return result;
            }
        }
    }
}
