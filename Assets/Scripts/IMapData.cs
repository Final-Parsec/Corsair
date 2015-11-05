﻿namespace FinalParsec.Corsair
{
    using UnityEngine;

    /// <summary>
    ///     Describes a standard means to get information about a map.
    /// </summary>
    public interface IMapData
    {
        /// <summary>
        ///     Gets the duration, in seconds, each frame of the map's animation should be displayed.
        /// </summary>
        float AnimationSpeed { get; }

        /// <summary>
        ///     Gets the location of the destination node (where enemies go).
        /// </summary>
        Vector2 DestinationNode { get; }
        
        /// <summary>
        ///     Gets the location of the nodes where enemies spawn.
        /// </summary>
        Vector2[] EnemySpawnTileIndicies { get; }

        /// <summary>
        ///     Gets a value indicating whether the map uses an isometric grid.
        /// </summary>
        bool IsIsoGrid { get; }

        /// <summary>
        ///     Gets the name of the map.
        /// </summary>
        string MapName { get; }

        /// <summary>
        ///     Gets the dimensions of the node map.
        /// </summary>
        Vector2 NodeSize { get; }

        /// <summary>
        ///     Gets the 2D tile set.
        /// </summary>
        Tile[,] Tiles { get; }

        /// <summary>
        ///     Gets the size, in pixels, of an individual tile.
        /// </summary>
        Vector2 TileSize { get; }
    }
}