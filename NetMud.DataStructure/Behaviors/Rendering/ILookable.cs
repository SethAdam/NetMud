﻿using NetMud.DataStructure.Base.System;
using System.Collections.Generic;

namespace NetMud.DataStructure.Behaviors.Rendering
{
    /// <summary>
    /// Framework for rendering output when this entity is Look(ed) at
    /// </summary>
    public interface ILookable
    {
        /// <summary>
        /// Renders output for this entity when Look targets it
        /// </summary>
        /// <param name="actor">entity initiating the command</param>
        /// <returns>the output</returns>
        IEnumerable<string> RenderToLook(IEntity actor);
    }
}
