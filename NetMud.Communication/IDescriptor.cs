﻿using NetMud.Authentication;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetMud.Communication
{
    /// <summary>
    /// Interface defining players connected to a server
    /// </summary>
    public interface IDescriptor
    {
        /// <summary>
        /// The user manager for the application, handles authentication from the web
        /// </summary>
        ApplicationUserManager UserManager { get; set; }

        /// <summary>
        /// The cache key for the global cache system
        /// </summary>
        string CacheKey { get; }

        /// <summary>
        /// Handles initial connection
        /// </summary>
        bool OnOpen();

        /// <summary>
        /// Handles when the connection closes
        /// </summary>
        /// <param name="e">events for closing</param>
        void OnClose();

        /// <summary>
        /// Handles when the connection faults
        /// </summary>
        /// <param name="e">events for the error</param>
        void OnError(Exception err);

        /// <summary>
        /// Handles when the connected descriptor sends input
        /// </summary>
        /// <param name="e">the events of the message</param>
        bool OnMessage(string message);

        /// <summary>
        /// Wraps sending messages to the connected descriptor
        /// </summary>
        /// <param name="strings">the output</param>
        /// <returns>success status</returns>
        bool SendWrapper(IEnumerable<string> strings);

        /// <summary>
        /// Wraps sending messages to the connected descriptor
        /// </summary>
        /// <param name="str">the output</param>
        /// <returns>success status</returns>
        bool SendWrapper(string str);

        /// <summary>
        /// Disconnects this descriptor forcibly
        /// </summary>
        /// <param name="finalMessage">the final string to send the client</param>
        void Disconnect(string finalMessage);
    }
}
