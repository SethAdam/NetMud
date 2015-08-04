﻿using NetMud.DataStructure.Base.Supporting;
using NetMud.DataStructure.Behaviors.System;
using NetMud.DataStructure.SupportingClasses;
using System;

namespace NetMud.DataStructure.Base.Entity
{
    /// <summary>
    /// Player character + account entity class
    /// </summary>
    public interface IPlayer : IMobile, ISpawnAsSingleton
    {
        /// <summary>
        /// ID for the connection the player has with us
        /// </summary>
        string DescriptorID { get; set; }

        /// <summary>
        /// Connection type the player is coming in on
        /// </summary>
        DescriptorType Descriptor { get; set; }

        /// <summary>
        /// Function used to close the connection
        /// </summary>
        Func<bool> CloseConnection { get; set; }
    }

    /// <summary>
    /// Connection type players come in on
    /// </summary>
    public enum DescriptorType
    {
        WebSockets
    }
}
