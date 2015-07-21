﻿using NetMud.DataAccess;
using NetMud.DataStructure.Base.EntityBackingData;
using NetMud.DataStructure.Base.Place;
using NetMud.DataStructure.Behaviors.Rendering;
using NetMud.DataStructure.Behaviors.System;
using NetMud.DataStructure.SupportingClasses;
using NetMud.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NetMud.Data.Game
{
    public class Path : EntityPartial, IPath
    {
        public ILocation ToLocation { get; set; }
        public ILocation FromLocation { get; set; }
        public MessageCluster Enter { get; set; }
        public MovementDirectionType MovementDirection { get; private set; }

        public Path()
        {
            Enter = new MessageCluster();
        }

        public Path(IPathData backingStore)
        {
            Enter = new MessageCluster();
            DataTemplate = backingStore;
            GetFromWorldOrSpawn();
        }

        public void GetFromWorldOrSpawn()
        {
            var liveWorld = new LiveCache();

            //Try to see if they are already there
            var me = liveWorld.Get<Path>(DataTemplate.ID);

            //Isn't in the world currently
            if (me == default(IPath))
                SpawnNewInWorld();
            else
            {
                BirthMark = me.BirthMark;
                Keywords = me.Keywords;
                Birthdate = me.Birthdate;
                CurrentLocation = me.CurrentLocation;
                DataTemplate = me.DataTemplate;
                FromLocation = me.FromLocation;
                ToLocation = me.ToLocation;
                Enter = me.Enter;
                MovementDirection = me.MovementDirection;
            }
        }

        public override void SpawnNewInWorld()
        {
            var liveWorld = new LiveCache();
            var bS = (IPathData)DataTemplate;

            SpawnNewInWorld(null);
        }

        public override void SpawnNewInWorld(IContains spawnTo)
        {
            var liveWorld = new LiveCache();
            var bS = (IPathData)DataTemplate;
            var locationAssembly = Assembly.GetAssembly(typeof(ILocation));

            MovementDirection = MessagingUtility.TranslateDegreesToDirection(bS.DegreesFromNorth);

            BirthMark = Birthmarker.GetBirthmark(bS);
            Keywords = new string[] { bS.Name.ToLower(), MovementDirection.ToString().ToLower() };
            Birthdate = DateTime.Now;

            //paths need two locations
            ILocation fromLocation = null;
            var fromLocationType = locationAssembly.DefinedTypes.FirstOrDefault(tp => tp.Name.Equals(bS.FromLocationType));

            if (fromLocationType != null && !String.IsNullOrWhiteSpace(bS.FromLocationID))
            {
                if (fromLocationType.GetInterfaces().Contains(typeof(ISpawnAsSingleton)))
                {
                    long fromLocationID = long.Parse(bS.FromLocationID);
                    fromLocation = liveWorld.Get<ILocation>(fromLocationID, fromLocationType);
                }
                else
                {
                    var cacheKey = new LiveCacheKey(fromLocationType, bS.FromLocationID);
                    fromLocation = liveWorld.Get<ILocation>(cacheKey);
                }
            }

            ILocation toLocation = null;
            var toLocationType = locationAssembly.DefinedTypes.FirstOrDefault(tp => tp.Name.Equals(bS.ToLocationType));

            if (toLocationType != null && !String.IsNullOrWhiteSpace(bS.ToLocationID))
            {
                if (toLocationType.GetInterfaces().Contains(typeof(ISpawnAsSingleton)))
                {
                    long toLocationID = long.Parse(bS.ToLocationID);
                    toLocation = liveWorld.Get<ILocation>(toLocationID, toLocationType);
                }
                else
                {
                    var cacheKey = new LiveCacheKey(toLocationType, bS.ToLocationID);
                    toLocation = liveWorld.Get<ILocation>(cacheKey);
                }
            }

            FromLocation = fromLocation;
            ToLocation = toLocation;
            CurrentLocation = fromLocation;

            Enter = new MessageCluster(bS.MessageToActor, "$A$ enters you", String.Empty, bS.MessageToOrigin, bS.MessageToDestination);
            Enter.ToSurrounding.Add(bS.VisibleStrength, new Tuple<MessagingType, string>(MessagingType.Visible, bS.VisibleToSurroundings));
            Enter.ToSurrounding.Add(bS.AudibleStrength, new Tuple<MessagingType, string>(MessagingType.Visible, bS.AudibleToSurroundings));

            fromLocation.MoveTo<IPath>(this);
        }

        public override IEnumerable<string> RenderToLook()
        {
            var sb = new List<string>();
            var bS = (IPathData)DataTemplate;

            sb.Add(string.Format("{0} heads in the direction of {1} from {2} to {3}", bS.Name, MovementDirection.ToString(), FromLocation.DataTemplate.Name, ToLocation.DataTemplate.Name));

            return sb;
        }
    }
}