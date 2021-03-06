﻿using NetMud.Communication.Messaging;
using NetMud.Data.System;
using NetMud.DataAccess.Cache;
using NetMud.DataStructure.Base.Entity;
using NetMud.DataStructure.Base.EntityBackingData;
using NetMud.DataStructure.Base.System;
using NetMud.DataStructure.Behaviors.Existential;
using NetMud.DataStructure.Behaviors.Rendering;
using NetMud.DataStructure.SupportingClasses;
using NetMud.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace NetMud.Data.Game
{
    /// <summary>
    /// live player character entities
    /// </summary>
    [Serializable]
    [IgnoreAutomatedBackup]
    public class Player : EntityPartial, IPlayer
    {
        /// <summary>
        /// The name of the object in the data template
        /// </summary>
        public override string DataTemplateName
        {
            get
            {
                if (DataTemplate<ICharacter>() == null)
                    return String.Empty;

                return DataTemplate<ICharacter>().Name;
            }
        }

        /// <summary>
        /// News up an empty entity
        /// </summary>
        public Player()
        {
            Inventory = new EntityContainer<IInanimate>();
        }

        /// <summary>
        /// News up an entity with its backing data
        /// </summary>
        /// <param name="character">the backing data</param>
        public Player(ICharacter character)
        {
            Inventory = new EntityContainer<IInanimate>();
            DataTemplateId = character.ID;
            AccountHandle = character.AccountHandle;
            GetFromWorldOrSpawn();
        }

        [ScriptIgnore]
        [JsonIgnore]
        private LiveCacheKey _descriptorKey;

        /// <summary>
        /// The connection the player is using to chat with us
        /// </summary>
        [ScriptIgnore]
        [JsonIgnore]
        public IDescriptor Descriptor
        {
            get
            {
                if (_descriptorKey == null)
                    return default(IDescriptor);

                return LiveCache.Get<IDescriptor>(_descriptorKey);
            }

            set
            {
                _descriptorKey = new LiveCacheKey(typeof(IDescriptor), value.BirthMark);

                LiveCache.Add(value);
            }
        }

        [ScriptIgnore]
        [JsonIgnore]
        public override IChannelType ConnectionType
        {
            get
            {
                //All player descriptors should be of ichanneltype too
                return (IChannelType)Descriptor;
            }
        }

        /// <summary>
        /// The account this character belongs to
        /// </summary>
        public string AccountHandle { get; set; }

        /// <summary>
        /// The backing data for this entity
        /// </summary>
        public override T DataTemplate<T>()
        {
            return (T)PlayerDataCache.Get(new PlayerDataCacheKey(typeof(ICharacter), AccountHandle, DataTemplateId));
        }

        /// <summary>
        /// Function used to close this connection
        /// </summary>
        public void CloseConnection()
        {
            Descriptor.Disconnect(String.Empty);
        }

        public override bool WriteTo(IEnumerable<string> input)
        {
            var strings = MessagingUtility.TranslateColorVariables(input.ToArray(), this);

            return Descriptor.SendWrapper(strings);
        }

        /// <summary>
        /// Birthmark for current live location of this
        /// </summary>
        private string _currentLocationBirthmark;

        /// <summary>
        /// Restful location container this is inside of
        /// </summary>
        [ScriptIgnore]
        [JsonIgnore]
        public override IContains InsideOf
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(_currentLocationBirthmark))
                    return LiveCache.Get<IContains>(new LiveCacheKey(typeof(IContains), _currentLocationBirthmark));

                return null;
            }
            set
            {
                if (value == null)
                    return;

                _currentLocationBirthmark = value.BirthMark;
                UpsertToLiveWorldCache();

                //We save character data to ensure the player remains where it was on last known change
                var ch = DataTemplate<ICharacter>();
                ch.Save();
            }
        }

        /// <summary>
        /// Get's the entity's model dimensions
        /// </summary>
        /// <returns>height, length, width</returns>
        public override Tuple<int, int, int> GetModelDimensions()
        {
            var charData = DataTemplate<ICharacter>(); ;
            var height = charData.RaceData.Head.Model.Height + charData.RaceData.Torso.Model.Height + charData.RaceData.Legs.Item1.Model.Height;
            var length = charData.RaceData.Torso.Model.Length;
            var width = charData.RaceData.Torso.Model.Width;

            return new Tuple<int, int, int>(height, length, width);
        }

        #region Rendering
        /// <summary>
        /// Render this to a look command (what something sees when it 'look's at this
        /// </summary>
        /// <returns>the output strings</returns>
        public override IEnumerable<string> RenderToLook(IEntity actor)
        {
            var sb = new List<string>();
            var ch = DataTemplate<ICharacter>(); ;

            sb.Add(string.Format("This is {0}", ch.FullName()));

            return sb;
        }
        #endregion

        #region Container
        /// <summary>
        /// Inanimates contained in this
        /// </summary>
        public IEntityContainer<IInanimate> Inventory { get; set; }

        /// <summary>
        /// Get all of the entities matching a type inside this
        /// </summary>
        /// <typeparam name="T">the type</typeparam>
        /// <returns>the contained entities</returns>
        public IEnumerable<T> GetContents<T>()
        {
            var implimentedTypes = DataUtility.GetAllImplimentingedTypes(typeof(T));

            var contents = new List<T>();

            if (implimentedTypes.Contains(typeof(IInanimate)))
                contents.AddRange(Inventory.EntitiesContained().Select(ent => (T)ent));

            return contents;
        }

        /// <summary>
        /// Get all of the entities matching a type inside this in a named container
        /// </summary>
        /// <typeparam name="T">the type</typeparam>
        /// <returns>the contained entities</returns>
        /// <param name="containerName">the name of the container</param>
        public IEnumerable<T> GetContents<T>(string containerName)
        {
            var implimentedTypes = DataUtility.GetAllImplimentingedTypes(typeof(T));

            var contents = new List<T>();

            if (implimentedTypes.Contains(typeof(IInanimate)))
                contents.AddRange(Inventory.EntitiesContained(containerName).Select(ent => (T)ent));

            return contents;
        }

        /// <summary>
        /// Move an entity into this
        /// </summary>
        /// <typeparam name="T">the type of the entity to add</typeparam>
        /// <param name="thing">the entity to add</param>
        /// <returns>errors</returns>
        public string MoveInto<T>(T thing)
        {
            return MoveInto<T>(thing, string.Empty);
        }

        /// <summary>
        /// Move an entity into a named container in this
        /// </summary>
        /// <typeparam name="T">the type of the entity to add</typeparam>
        /// <param name="thing">the entity to add</param>
        /// <param name="containerName">the name of the container</param>
        /// <returns>errors</returns>
        public string MoveInto<T>(T thing, string containerName)
        {
            var implimentedTypes = DataUtility.GetAllImplimentingedTypes(typeof(T));

            if (implimentedTypes.Contains(typeof(IInanimate)))
            {
                var obj = (IInanimate)thing;

                if (Inventory.Contains(obj, containerName))
                    return "That is already in the container";

                Inventory.Add(obj, containerName);
                obj.InsideOf = this;
                this.UpsertToLiveWorldCache();
                return string.Empty;
            }

            return "Invalid type to move to container.";
        }

        /// <summary>
        /// Move an entity out of this
        /// </summary>
        /// <typeparam name="T">the type of entity to remove</typeparam>
        /// <param name="thing">the entity</param>
        /// <returns>errors</returns>
        public string MoveFrom<T>(T thing)
        {
            return MoveFrom<T>(thing, string.Empty);
        }

        /// <summary>
        /// Move an entity out of this' named container
        /// </summary>
        /// <typeparam name="T">the type of entity to remove</typeparam>
        /// <param name="thing">the entity</param>
        /// <param name="containerName">the name of the container</param>
        /// <returns>errors</returns>
        public string MoveFrom<T>(T thing, string containerName)
        {
            var implimentedTypes = DataUtility.GetAllImplimentingedTypes(typeof(T));

            if (implimentedTypes.Contains(typeof(IInanimate)))
            {
                var obj = (IInanimate)thing;

                if (!Inventory.Contains(obj, containerName))
                    return "That is not in the container";

                Inventory.Remove(obj, containerName);
                obj.InsideOf = null;
                this.UpsertToLiveWorldCache();
                return string.Empty;
            }

            return "Invalid type to move from container.";
        }
        #endregion

        #region SpawnBehavior
        /// <summary>
        /// Tries to find this entity in the world based on its ID or gets a new one from the db and puts it in the world
        /// </summary>
        public void GetFromWorldOrSpawn()
        {
            //Try to see if they are already there
            var me = LiveCache.Get<Player>(DataTemplateId);

            //Isn't in the world currently
            if (me == default(IPlayer))
                SpawnNewInWorld();
            else
            {
                BirthMark = me.BirthMark;
                Birthdate = me.Birthdate;
                DataTemplateId = me.DataTemplate<ICharacter>().ID;
                Inventory = me.Inventory;
                Keywords = me.Keywords;

                if (me.InsideOf == null)
                {
                    var newLoc = GetBaseSpawn();
                    this.Reposition(newLoc, null);
                }
                else
                    me.InsideOf.MoveInto<IPlayer>(this);
            }
        }

        /// <summary>
        /// Find the emergency we dont know where to spawn this guy spawn location
        /// </summary>
        /// <returns>The emergency spawn location</returns>
        private IGlobalPosition GetBaseSpawn()
        {
            var chr = DataTemplate<ICharacter>(); ;

            var gPos = chr.StillANoob ? chr.RaceData.StartingLocation : chr.RaceData.EmergencyLocation;

            return gPos;
        }

        /// <summary>
        /// Spawn this new into the live world
        /// </summary>
        public override void SpawnNewInWorld()
        {
            var ch = DataTemplate<ICharacter>(); ;
        }

        /// <summary>
        /// Spawn a new instance of this entity into the live world in a set position
        /// </summary>
        /// <param name="position">x,y,z coordinates to spawn into</param>
        public override void SpawnNewInWorld(IGlobalPosition position)
        {
            //We can't even try this until we know if the data is there
            if (DataTemplate<ICharacter>() == null)
                throw new InvalidOperationException("Missing backing data store on player spawn event.");

            var ch = DataTemplate<ICharacter>();

            BirthMark = LiveCache.GetUniqueIdentifier(ch);
            Keywords = new string[] { ch.Name.ToLower(), ch.SurName.ToLower() };
            Birthdate = DateTime.Now;

            //Set the data context's stuff too so we don't have to do this over again
            ch.Save();

            Inventory = new EntityContainer<IInanimate>();

            this.Reposition(position, null);

            LiveCache.Add(this);
        }

        /// <summary>
        /// Spawn this new into the live world into a specified container
        /// </summary>
        /// <param name="spawnTo">the location/container this should spawn into</param>
        public override void SpawnNewInWorld(IContains spawnTo)
        {
            if (spawnTo == null)
                SpawnNewInWorld(GetBaseSpawn());

            var ch = DataTemplate<ICharacter>();

            BirthMark = LiveCache.GetUniqueIdentifier(ch);
            Keywords = new string[] { ch.Name.ToLower(), ch.SurName.ToLower() };
            Birthdate = DateTime.Now;

            InsideOf = spawnTo;

            //Set the data context's stuff too so we don't have to do this over again
            ch.Save();

            spawnTo.MoveInto<IPlayer>(this);

            Inventory = new EntityContainer<IInanimate>();

            LiveCache.Add(this);
        }
        #endregion
    }
}
