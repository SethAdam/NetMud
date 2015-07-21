﻿using NetMud.DataStructure.Base.EntityBackingData;
using NetMud.DataStructure.Base.Place;
using NetMud.DataStructure.Base.Supporting;
using NetMud.DataStructure.Base.System;
using NetMud.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NetMud.DataStructure.SupportingClasses
{
    public enum MessagingType
    {
        Visible,
        Audible,
        Psychic
    }

    public enum MessagingTargetType
    {
        Actor,
        Subject,
        Target,
        OriginLocation,
        DestinationLocation,
        GenderPronoun,
        AmountOfSubject,
        AmountOfTarget,
        Direction,
        ReverseDirection
    }

    public static class MessagingUtility
    {
        private const string colorPattern = "\\%[a-zA-z]+\\%";

        public static string TranslateColorVariables(string message)
        {
            bool stillFound = true;
            while (Regex.IsMatch(message, colorPattern) && stillFound)
            {
                //Need a way to short-circut some bozo creating an infinite loop
                stillFound = false;

                //Bold
                if (message.Contains("%ST%"))
                {
                    message = ReplaceColor(message, "%ST%", "font-weight: bold;");
                    stillFound = true;
                }

                //italics
                if (message.Contains("%IT%"))
                {
                    message = ReplaceColor(message, "%IT%", "font-style: italic;");
                    stillFound = true;
                }

                //TODO: Replace color words with hex color codes and expand variety
                //Blue
                if (message.Contains("%B%"))
                {
                    message = ReplaceColor(message, "%B%", "color: #0000FF;");
                    stillFound = true;
                }


                //light-blue
                if (message.Contains("%b%"))
                {
                    message = ReplaceColor(message, "%b%", "color: #6495ED;");
                    stillFound = true;
                }


                //orange
                if (message.Contains("%O%"))
                {
                    message = ReplaceColor(message, "%O%", "color: #FF7F50;");
                    stillFound = true;
                }


                //light orange
                if (message.Contains("%o%"))
                {
                    message = ReplaceColor(message, "%o%", "color: #D2691E;");
                    stillFound = true;
                }


                //yellow
                if (message.Contains("%Y%"))
                {
                    message = ReplaceColor(message, "%Y%", "color: #FFD700;");
                    stillFound = true;
                }


                //light-yellow
                if (message.Contains("%y%"))
                {
                    message = ReplaceColor(message, "%y%", "color: #F0E68C;");
                    stillFound = true;
                }


                //green
                if (message.Contains("%G%"))
                {
                    message = ReplaceColor(message, "%G%", "color: #008000;");
                    stillFound = true;
                }


                //light green
                if (message.Contains("%g%"))
                {
                    message = ReplaceColor(message, "%g%", "color: #90EE90;");
                    stillFound = true;
                }


                //indigo
                if (message.Contains("%I%"))
                {
                    message = ReplaceColor(message, "%I%", "color: #4B0082;");
                    stillFound = true;
                }


                //light purple
                if (message.Contains("%i%"))
                {
                    message = ReplaceColor(message, "%i%", "color: #9400D3;");
                    stillFound = true;
                }


                //red
                if (message.Contains("%R%"))
                {
                    message = ReplaceColor(message, "%R%", "color: #FF4500;");
                    stillFound = true;
                }

                //light red
                if (message.Contains("%r%"))
                {
                    message = ReplaceColor(message, "%r%", "color: #800000;");
                    stillFound = true;
                }

                //pink
                if (message.Contains("%P%"))
                {
                    message = ReplaceColor(message, "%P%", "color: #FF69B4;");
                    stillFound = true;
                }

                //light pink
                if (message.Contains("%p%"))
                {
                    message = ReplaceColor(message, "%p%", "color: #FFB6C1;");
                    stillFound = true;
                }
            }

            return message;
        }

        private static string ReplaceColor(string originalString, string formatToReplace, string styleElement)
        {
            if (String.IsNullOrWhiteSpace(originalString) || String.IsNullOrWhiteSpace(formatToReplace) || String.IsNullOrWhiteSpace(styleElement))
                return originalString;

            var firstIndex = originalString.IndexOf(formatToReplace);

            if (firstIndex < 1)
                return originalString;

            var secondIndex = originalString.IndexOf(formatToReplace, firstIndex + 1);

            //Yes 1st instance but no second instance? replace them all with empty string to scrub the string.
            if (secondIndex < 1)
                return originalString.Replace(formatToReplace, String.Empty);

            var lengthToSkip = formatToReplace.Length;

            return String.Format("{0}<span style=\"{3}\">{1}</span>{2}"
                    , originalString.Substring(0, firstIndex)
                    , originalString.Substring(firstIndex + lengthToSkip, secondIndex - firstIndex - lengthToSkip)
                    , originalString.Substring(secondIndex + lengthToSkip)
                    , styleElement);
        }

        public static string TranslateEntityVariables(string message, Dictionary<MessagingTargetType, IEntity[]> entities)
        {
            foreach (KeyValuePair<MessagingTargetType, IEntity[]> kvp in entities)
            {
                IEntity thing = null;

                if (kvp.Value.Length.Equals(1))
                {
                    if (kvp.Value[0] == null)
                        continue;

                    thing = kvp.Value[0];
                }

                switch (kvp.Key)
                {
                    case MessagingTargetType.Actor:
                        message = message.Replace("$A$", thing.DataTemplate.Name);
                        break;
                    case MessagingTargetType.DestinationLocation:
                        message = message.Replace("$D$", thing.DataTemplate.Name);
                        break;
                    case MessagingTargetType.OriginLocation:
                        message = message.Replace("$O$", thing.DataTemplate.Name);
                        break;
                    case MessagingTargetType.Subject:
                        message = message.Replace("$S$", thing.DataTemplate.Name);
                        break;
                    case MessagingTargetType.Target:
                        message = message.Replace("$T$", thing.DataTemplate.Name);
                        break;
                    case MessagingTargetType.GenderPronoun:
                        if (!thing.GetType().GetInterfaces().Contains(typeof(IGender)))
                            break;

                        IGender chr = (IGender)thing;
                        message = message.Replace("$G$", chr.Gender);
                        break;
                    case MessagingTargetType.AmountOfSubject:
                        message = message.Replace("$#S$", kvp.Value.Length.ToString());
                        break;
                    case MessagingTargetType.AmountOfTarget:
                        message = message.Replace("$#T$", kvp.Value.Length.ToString());
                        break;
                    case MessagingTargetType.Direction:
                    case MessagingTargetType.ReverseDirection:
                        if (!thing.GetType().GetInterfaces().Contains(typeof(IPath)))
                            break;

                        IPathData pathData = (IPathData)thing.DataTemplate;
                        message = message.Replace("$DIR$", TranslateDegreesToDirection(pathData.DegreesFromNorth, kvp.Key == MessagingTargetType.ReverseDirection).ToString());
                        break;
                }
            }

            return message;
        }
        public static MovementDirectionType TranslateDegreesToDirection(int degreesFromNorth, bool reverse = false)
        {
            var trueDegrees = degreesFromNorth;

            if (reverse)
                trueDegrees = degreesFromNorth < 180 ? degreesFromNorth + 180 : degreesFromNorth - 180;

            if (trueDegrees > 22 && trueDegrees < 67)
                return MovementDirectionType.NorthEast;
            if (trueDegrees > 66 && trueDegrees < 111)
                return MovementDirectionType.East;
            if (trueDegrees > 110 && trueDegrees < 155)
                return MovementDirectionType.SouthEast;
            if (trueDegrees > 154 && trueDegrees < 199)
                return MovementDirectionType.South;
            if (trueDegrees > 198 && trueDegrees < 243)
                return MovementDirectionType.SouthWest;
            if (trueDegrees > 242 && trueDegrees < 287)
                return MovementDirectionType.West;
            if (trueDegrees > 286 && trueDegrees < 331)
                return MovementDirectionType.NorthWest;

            return MovementDirectionType.North;
        }

    }
}
