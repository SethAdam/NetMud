﻿using NetMud.DataStructure.Base.EntityBackingData;
using NetMud.DataStructure.Base.Place;
using NetMud.DataStructure.Behaviors.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMud.Cartography
{
    /// <summary>
    /// Methods for handling with the coordinate maps
    /// </summary>
    public static class Cartographer
    {
        /// <summary>
        /// Render a 3d map down to 2d
        /// </summary>
        /// <param name="zIndex"></param>
        /// <returns>flattened map</returns>
        public static long[,] GetSinglePlane(long[,,] fullMap, int zIndex)
        {
            if (zIndex > fullMap.GetUpperBound(2))
                throw new InvalidOperationException("Requested zIndex greater than upper Z bound of map.");

            var flatMap = new long[fullMap.GetUpperBound(0), fullMap.GetUpperBound(1)];

            int x, y;
            for (x = 0; x < fullMap.GetUpperBound(0); x++)
                for (y = 0; y < fullMap.GetUpperBound(1); y++)
                    flatMap[x, y] = fullMap[x, y, zIndex];

            return flatMap;
        }

        /// <summary>
        /// Generate a room map starting in a room backing data with a radius around it
        /// </summary>
        /// <param name="room">the starting room</param>
        /// <param name="radius">the radius of rooms to go out to. -1 means "generate the entire world"</param>
        /// <param name="recenter">find the center node of the array and return an array with that node at absolute center</param>
        /// <returns>a 3d array of rooms</returns>
        public static long[, ,] GenerateMapFromRoom(IRoomData room, int radius, bool recenter = false)
        {
            var diameter = radius * 2 + 1;
            var returnMap = new long[diameter, diameter, diameter];

            return returnMap;
        }

        /// <summary>
        /// Gives back the original map but with all rooms that fall outside of the indicated bounds removed
        /// </summary>
        /// <param name="xBounds">The upper and lower bounds to grab for X axis</param>
        /// <param name="yBounds">The upper and lower bounds to grab for Y axis</param>
        /// <param name="zBounds">The upper and lower bounds to grab for Z axis</param>
        /// <param name="map">The map to take from</param>
        /// <param name="shrink">Return a new array that is bound to the size of the remaining data</param>
        /// <returns>the new sliced array</returns>
        public static long[, ,] TakeSliceOfMap(Tuple<int, int> xBounds, Tuple<int, int> yBounds, Tuple<int, int> zBounds, long[, ,] map, bool shrink = false)
        {
            var newMap = new long[map.GetUpperBound(0), map.GetUpperBound(1), map.GetUpperBound(2)];

            int x, y, z, xLowest = 0, yLowest = 0, zLowest = 0;

            for (x = 0; x < map.GetUpperBound(0); x++)
                if (x >= xBounds.Item2 && x <= xBounds.Item1)
                    for (y = 0; y < map.GetUpperBound(1); y++)
                        if (y >= yBounds.Item2 && y <= yBounds.Item1)
                            for (z = 0; z < map.GetUpperBound(2); z++)
                                if (z >= zBounds.Item2 && z <= zBounds.Item1 && map[x, y, z] != null)
                                {
                                    newMap[x, y, z] = map[x, y, z];

                                    if (xLowest > x)
                                        xLowest = x;

                                    if (yLowest > y)
                                        yLowest = y;

                                    if (zLowest > z)
                                        zLowest = z;
                                }

            //Maps were the same size or we didnt want to shrink
            if (!shrink || (xLowest <= 0 && yLowest <= 0 && zLowest <= 0))
                return newMap;

            var shrunkMap = new long[newMap.GetUpperBound(0) - xLowest, newMap.GetUpperBound(1) - yLowest, newMap.GetUpperBound(2) - yLowest];

            for (x = 0; x < shrunkMap.GetUpperBound(0); x++)
                if (x >= xBounds.Item2 && x <= xBounds.Item1)
                    for (y = 0; y < shrunkMap.GetUpperBound(1); y++)
                        if (y >= yBounds.Item2 && y <= yBounds.Item1)
                            for (z = 0; z < shrunkMap.GetUpperBound(2); z++)
                                shrunkMap[x, y, z] = newMap[x + xLowest, y + yLowest, z + zLowest];

            return shrunkMap;
        }
    }
}
