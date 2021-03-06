﻿using System.Collections.Generic;
using NutMud.Commands.Attributes;
using NetMud.Utility;
using NetMud.Commands.Attributes;
using NetMud.DataStructure.SupportingClasses;

namespace NetMud.Commands.Movement
{
    /// <summary>
    /// Handles mobile movement commands. All cardinal directions plus "enter <door>" type pathways
    /// </summary>
    [CommandSuppressName]
    [CommandKeyword("enter", false)]
    [CommandKeyword("east", true)]
    [CommandKeyword("north", true)]
    [CommandKeyword("northeast", true)]
    [CommandKeyword("northwest", true)]
    [CommandKeyword("south", true)]
    [CommandKeyword("southwest", true)]
    [CommandKeyword("southeast", true)]
    [CommandKeyword("west", true)]
    [CommandKeyword("up", true)]
    [CommandKeyword("down", true)]
    [CommandKeyword("upnorth", true)]
    [CommandKeyword("upnortheast", true)]
    [CommandKeyword("upnorthwest", true)]
    [CommandKeyword("upsouth", true)]
    [CommandKeyword("upsouthwest", true)]
    [CommandKeyword("upsoutheast", true)]
    [CommandKeyword("upwest", true)]
    [CommandKeyword("downnorth", true)]
    [CommandKeyword("downnortheast", true)]
    [CommandKeyword("downnorthwest", true)]
    [CommandKeyword("downsouth", true)]
    [CommandKeyword("downsouthwest", true)]
    [CommandKeyword("downsoutheast", true)]
    [CommandKeyword("downwest", true)]
    [CommandPermission(StaffRank.Player)]
    //[CommandParameter(CommandUsage.Subject, typeof(IPathway), new CacheReferenceType[] { CacheReferenceType.Entity }, "[a-zA-z]+", true)]
    [CommandRange(CommandRangeType.Touch, 0)]
    public class UseExits : CommandPartial
    {
        /// <summary>
        /// All Commands require a generic constructor
        /// </summary>
        public UseExits()
        {
            //Generic constructor for all IHelpfuls is needed
        }

        /// <summary>
        /// Executes this command
        /// </summary>
        public override void Execute()
        {
            var sb = new List<string>();
           // IPathway targetPath = (IPathway)Subject;

           // targetPath.FromLocation.MoveFrom((IMobile)Actor);
           // targetPath.ToLocation.MoveInto((IMobile)Actor);

            //targetPath.Enter.ExecuteMessaging(Actor, targetPath, null, targetPath.FromLocation, targetPath.ToLocation);
        }

        /// <summary>
        /// Renders syntactical help for the command, invokes automatically when syntax is bungled
        /// </summary>
        /// <returns>string</returns>
        public override IEnumerable<string> RenderSyntaxHelp()
        {
            var sb = new List<string>();

            sb.Add(string.Format("Valid Syntax:"));
            sb.Add("east".PadWithString(14, "&nbsp;", true));
            sb.Add("north".PadWithString(14, "&nbsp;", true));
            sb.Add("northeast".PadWithString(14, "&nbsp;", true));
            sb.Add("northwest".PadWithString(14, "&nbsp;", true));
            sb.Add("south".PadWithString(14, "&nbsp;", true));
            sb.Add("southeast".PadWithString(14, "&nbsp;", true));
            sb.Add("southwest".PadWithString(14, "&nbsp;", true));
            sb.Add("west".PadWithString(14, "&nbsp;", true));
            sb.Add("up".PadWithString(14, "&nbsp;", true));
            sb.Add("down".PadWithString(14, "&nbsp;", true));
            sb.Add("upeast".PadWithString(14, "&nbsp;", true));
            sb.Add("upnorth".PadWithString(14, "&nbsp;", true));
            sb.Add("upnortheast".PadWithString(14, "&nbsp;", true));
            sb.Add("upnorthwest".PadWithString(14, "&nbsp;", true));
            sb.Add("upsouth".PadWithString(14, "&nbsp;", true));
            sb.Add("upsoutheast".PadWithString(14, "&nbsp;", true));
            sb.Add("upsouthwest".PadWithString(14, "&nbsp;", true));
            sb.Add("upwest".PadWithString(14, "&nbsp;", true));
            sb.Add("downeast".PadWithString(14, "&nbsp;", true));
            sb.Add("downnorth".PadWithString(14, "&nbsp;", true));
            sb.Add("downnortheast".PadWithString(14, "&nbsp;", true));
            sb.Add("downnorthwest".PadWithString(14, "&nbsp;", true));
            sb.Add("downsouth".PadWithString(14, "&nbsp;", true));
            sb.Add("downsoutheast".PadWithString(14, "&nbsp;", true));
            sb.Add("downsouthwest".PadWithString(14, "&nbsp;", true));
            sb.Add("downwest".PadWithString(14, "&nbsp;", true));

            sb.Add("enter &lt;exit name&gt;".PadWithString(14, "&nbsp;", true));

            return sb;
        }

        /// <summary>
        /// The custom body of help text
        /// </summary>
        public override string HelpText
        {
            get
            {
                return string.Format("These are all directions, need better help text for movements.");
            }
            set { }
        }
    }
}
