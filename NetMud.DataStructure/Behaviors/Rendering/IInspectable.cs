﻿using NetMud.DataStructure.Base.System;
using System.Collections.Generic;
namespace NetMud.DataStructure.Behaviors.Rendering
{
    /// <summary>
    /// Indicates an entity can be Inspected (part of rendering)
    /// </summary>
    public interface IInspectable
    {
        /// <summary>
        /// Renders "display" from scan command
        /// </summary>
        /// <param name="actor">entity initiating the command</param>
        /// <returns>the scan output</returns>
        IEnumerable<string> RenderToInspect(IEntity actor);
    }
}
