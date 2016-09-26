﻿using NetMud.DataStructure.Behaviors.Rendering;
using NutMud.Commands.Attributes;
using System.Collections.Generic;

using NetMud.Utility;
using NetMud.DataStructure.Base.EntityBackingData;
using NetMud.Data.EntityBackingData;
using NetMud.Data.Game;
using NetMud.Commands.Attributes;
using NetMud.Communication.Messaging;
using NetMud.DataStructure.Base.System;

namespace NutMud.Commands.System
{
    /// <summary>
    /// Spawns a new inanimate into the world.  Missing target parameter = container you're standing in
    /// </summary>
    [CommandKeyword("SpawnNewObject", false)]
    [CommandPermission(StaffRank.Admin)]
    [CommandParameter(CommandUsage.Subject, typeof(InanimateData), new CacheReferenceType[] { CacheReferenceType.Data }, "[0-9]+", false)] //for IDs
    [CommandParameter(CommandUsage.Subject, typeof(InanimateData), new CacheReferenceType[] { CacheReferenceType.Data }, "[a-zA-z]+", false)] //for names
    [CommandParameter(CommandUsage.Target, typeof(IContains), new CacheReferenceType[] { CacheReferenceType.Entity }, true)]
    [CommandRange(CommandRangeType.Touch, 0)]
    public class SpawnNewObject : CommandPartial, IHelpful
    {
        /// <summary>
        /// All Commands require a generic constructor
        /// </summary>
        public SpawnNewObject()
        {
            //Generic constructor for all IHelpfuls is needed
        }

        /// <summary>
        /// Executes this command
        /// </summary>
        public override void Execute()
        {
            var newObject = (IInanimateData)Subject;
            var sb = new List<string>();
            IContains spawnTo;

            //No target = spawn to room you're in
            if (Target != null)
                spawnTo = (IContains)Target;
            else
                spawnTo = OriginLocation;

            var entityObject = new Inanimate(newObject, spawnTo);

            //TODO: keywords is janky, location should have its own identifier name somehow for output purposes - DISPLAY short/long NAME
            sb.Add(string.Format("{0} spawned to {1}", entityObject.DataTemplate<IData>().Name, spawnTo.Keywords[0]));

            var messagingObject = new MessageCluster(sb, new string[] { "You are ALIVE" }, new string[] { "You have been given $S$" }, new string[] { "$S$ appears in the $T$." }, new string[] { });

            messagingObject.ExecuteMessaging(Actor, entityObject, spawnTo, OriginLocation, null);
        }

        /// <summary>
        /// Renders syntactical help for the command, invokes automatically when syntax is bungled
        /// </summary>
        /// <returns>string</returns>
        public override IEnumerable<string> RenderSyntaxHelp()
        {
            var sb = new List<string>();

            sb.Add(string.Format("Valid Syntax: spawnNewObject &lt;object name&gt;"));
            sb.Add("spawnNewObject  &lt;object name&gt;  &lt;location name to spawn to&gt;".PadWithString(14, "&nbsp;", true));

            return sb;
        }

        /// <summary>
        /// Renders the help text
        /// </summary>
        /// <returns>string</returns>
        public IEnumerable<string> RenderHelpBody()
        {
            var sb = new List<string>();

            sb.Add(string.Format("SpawnNewObject spawns a new object from its data template into the room or into a specified inventory."));

            return sb;
        }
    }
}
