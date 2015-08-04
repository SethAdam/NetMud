﻿using NetMud.DataStructure.Base.System;
using NetMud.DataStructure.Behaviors.Rendering;
using NutMud.Commands.Attributes;
using System.Collections.Generic;

using NetMud.Utility;
using NetMud.DataStructure.SupportingClasses;
using NetMud.Data.Game;
using NetMud.Backup;
using System.Web.Hosting;

namespace NutMud.Commands.System
{
    /// <summary>
    /// Invokes the current container's RenderToLook
    /// </summary>
    [CommandKeyword("quit", false)]
    [CommandKeyword("exit", false)]
    [CommandPermission(StaffRank.Player)]
    [CommandRange(CommandRangeType.Touch, 0)]
    public class Quit : ICommand, IHelpful
    {
        /// <summary>
        /// The entity invoking the command
        /// </summary>
        public IActor Actor { get; set; }

        /// <summary>
        /// The entity the command refers to
        /// </summary>
        public object Subject { get; set; }

        /// <summary>
        /// When there is a predicate parameter, the entity that is being targetting (subject become "with")
        /// </summary>
        public object Target { get; set; }

        /// <summary>
        /// Any tertiary entity being referenced in command parameters
        /// </summary>
        public object Supporting { get; set; }

        /// <summary>
        /// Container the Actor is in when the command is invoked
        /// </summary>
        public ILocation OriginLocation { get; set; }

        /// <summary>
        /// Valid containers by range from OriginLocation
        /// </summary>
        public IEnumerable<ILocation> Surroundings { get; set; }

        /// <summary>
        /// All Commands require a generic constructor
        /// </summary>
        public Quit()
        {
            //Generic constructor for all IHelpfuls is needed
        }

        /// <summary>
        /// Executes this command
        /// </summary>
        public void Execute()
        {
            var sb = new List<string>();

            var player = (Player)Actor;

            sb.Add("You exit this reality.");

            var messagingObject = new MessageCluster(RenderUtility.EncapsulateOutput(sb), string.Empty, string.Empty, "$A$ exits this reality.", string.Empty);

            messagingObject.ExecuteMessaging(Actor, null, null, OriginLocation, null);

            var hotBack = new HotBackup(HostingEnvironment.MapPath("/HotBackup/"));

            //Save the player out
            hotBack.WriteOnePlayer(player);
            player.CloseConnection();
        }

        /// <summary>
        /// Renders syntactical help for the command, invokes automatically when syntax is bungled
        /// </summary>
        /// <returns>string</returns>
        public IEnumerable<string> RenderSyntaxHelp()
        {
            var sb = new List<string>();

            sb.Add("Valid Syntax: quit");
            sb.Add("exit".PadWithString(14, "&nbsp;", true));

            return sb;
        }

        /// <summary>
        /// Renders the help text
        /// </summary>
        /// <returns>string</returns>
        public IEnumerable<string> RenderHelpBody()
        {
            var sb = new List<string>();

            sb.Add(string.Format("Quit/Exit removes your character from the live game allowing you to leave safely or switch characters."));

            return sb;
        }
    }
}