namespace FinalParsec.Corsair.Maps
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class WashingtonDCMapData : IMapData
    {
        private readonly Texture2D[] tileTextures;

        /// <summary>
        ///     Initializes a new instance of the <see cref="WashingtonDCMapData" /> class.
        /// </summary>
        public WashingtonDCMapData()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="WashingtonDCMapData" /> class.
        /// </summary>
        public WashingtonDCMapData(Texture2D grid)
        {
            this.grid = grid;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="WashingtonDCMapData" /> class.
        /// </summary>
        public WashingtonDCMapData(Texture2D[] tileTextures, Texture2D grid)
        {
            this.tileTextures = tileTextures;
            this.grid = grid;
        }

        /// <summary>
        ///     Backing field for <see cref="Tiles" />.
        /// </summary>
        private Tile[,] tiles;

        /// <summary>
        ///     Gets the duration, in seconds, each frame of the map's animation should be displayed.
        /// </summary>
        public float AnimationSpeed
        {
            get { return .28f; }
        }

        /// <summary>
        ///     Gets the location of the destination node (where enemies go).
        /// </summary>
        public Vector2 DestinationNode
        {
            get
            {
                return new Vector2(15, 15);
            }
        }

        /// <summary>
        ///     Gets the location of the nodes where enemies spawn.
        /// </summary>
        public Vector2[] EnemySpawnTileIndicies
        {
            get
            {
                return new[]
                {
                    new Vector2(0, 5),
                    new Vector2(31, 5),
                    new Vector2(15, 2)
                };
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the map uses an isometric grid.
        /// </summary>
        public bool IsIsoGrid
        {
            get { return true; }
        }

        /// <summary>
        ///     Gets the name of the map.
        /// </summary>
        public string MapName
        {
            get
            {
                return "Washington DC";
            }
            set
            {
                throw new NotSupportedException("Not intended for use in map editor.");
            }
        }

        /// <summary>
        ///     Gets the dimensions of the node map.
        /// </summary>
        public Vector2 NodeSize
        {
            get
            {
                return new Vector2(32, 16);
            }
        }

        /// <summary>
        ///     Gets the 2D tile set.
        /// </summary>
        public Tile[,] Tiles
        {
            get
            {
                if (this.tiles != default(Tile[,]))
                {
                    return this.tiles;
                }

                var lengthX = (int)this.NodeSize.x * 2 + 1;
                var lengthY = (int)this.NodeSize.y * 2 + 11;

                this.tiles = new Tile[lengthX, lengthY];

                for (var x = 0; x < lengthX; x++)
                {
                    for (var y = 0; y < lengthY; y++)
                    {
                        var textures = this.tileTextures;
                        var tile = new Tile(textures, false, false, false);

                        this.tiles[x, y] = tile;

                        if (x >= lengthX/4 &&
                            x < lengthX / 4 * 3 &&
                            y >= lengthY / 4 &&
                            y < lengthY / 4 * 3)
                        {
                            tile.isBuildable = true;
                            tile.isWalkable = true;
                            tile.isNode = true;
                        }

                        if (x == this.DestinationNode.XInt() &&
                            y == this.DestinationNode.YInt())
                        {
                            tile.doodads.AddFirst(Resources.Load<GameObject>("Doodads/White House with Trump/White House with Trump"));
                        }
                    }
                }

                return this.tiles;
            }
        }

        /// <summary>
        ///     Gets the size, in pixels, of an individual tile.
        /// </summary>
        public Vector2 TileSize
        {
            get
            {
                return new Vector2(128, 64);
            }
        }

        private Texture2D grid;
        public Texture2D Grid
        {
            get
            {
                return grid;
            }
        }
    }
}