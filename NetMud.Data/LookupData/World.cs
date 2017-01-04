﻿using NetMud.Data.System;
using NetMud.DataAccess.Cache;
using NetMud.DataStructure.Base.EntityBackingData;
using NetMud.DataStructure.Base.Place;
using NetMud.DataStructure.SupportingClasses;
using System;
using System.Collections.Generic;

namespace NetMud.Data.LookupData
{
    /// <summary>
    /// World for holding world maps
    /// </summary>
    [Serializable]
    [IgnoreAutomatedBackup]
    public class World : BackingDataPartial, IWorld
    {
        public IMap WorldMap { get; private set; }

        public long FullDiameter { get; set; }

        public HashSet<IStratum> Strata { get; set; }

        public HashSet<IChunk> Chunks { get; set; }

        public WorldType Topography { get; set; }

        public World(string name)
        {
            Name = name;
            Created = DateTime.UtcNow;
            LastRevised = DateTime.UtcNow;

            Strata = new HashSet<IStratum>();
            Chunks = new HashSet<IChunk>();

            //Set the id right now
            GetNextId();
        }

        /// <summary>
        /// Gets the errors for data fitness
        /// </summary>
        /// <returns>a bunch of text saying how awful your data is</returns>
        public override IList<string> FitnessReport()
        {
            var dataProblems = base.FitnessReport();

            if (FullDiameter < 1)
                dataProblems.Add("Diameter is 0 or less.");

            if (WorldMap == null)
                dataProblems.Add("World Map is null.");

            if (Strata.Count == 0)
                dataProblems.Add("World is void, no strata detected.");

            if (Chunks.Count == 0)
                dataProblems.Add("No chunks currently loaded to world.");

            return dataProblems;
        }
    }
}
