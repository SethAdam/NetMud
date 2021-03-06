﻿using System;
using System.Collections.Generic;

namespace NetMud.DataStructure.SupportingClasses
{
    /// <summary>
    /// Criteria used to lookup constants values
    /// </summary>
    public interface ILookupCriteria : IComparable<ILookupCriteria>, IEquatable<ILookupCriteria>
    {
        /// <summary>
        /// The type of criteria to look for
        /// </summary>
        Dictionary<CriteriaType, string> Criterion { get; set; }
    }

    /// <summary>
    /// Criteria type for constant values lookup
    /// </summary>
    public enum CriteriaType : short
    {
        Race = 0,
        Language = 1,
        GameLanguage = 2,
        PortType = 3,
        TimeOfDay = 4,
        Season = 5
    }
}
