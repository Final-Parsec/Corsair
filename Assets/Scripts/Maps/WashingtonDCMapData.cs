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
                    new Vector2(0, 6),
                    new Vector2(0, 4),
                    new Vector2(0, 2),
                    new Vector2(1, 4),
                    new Vector2(31, 5),
                    new Vector2(31, 7),
                    new Vector2(31, 3),
                    new Vector2(30, 5),
                    new Vector2(15, 0),
                    new Vector2(13, 0),
                    new Vector2(17, 0),
                    new Vector2(15, 1)
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

        private Vector2 tileGap;
        /// <summary>
        ///     Gets the left and bottom spacing between the tile map and the node map
        /// </summary>
        public Vector2 TileGap {
            get
            {
                return tileGap;
            }
            set
            {
                tileGap = value;
            }
        }

        private Vector2[] leftDownFenceLocations = new Vector2[] { new Vector2(5, 15), new Vector2(5, 14) , new Vector2(5, 13) ,  new Vector2(5, 12) , new Vector2(5, 11) , new Vector2(5, 10), new Vector2(5,9), new Vector2(5, 8) ,
            new Vector2(5, 7), new Vector2(5, 3) ,  new Vector2(5, 2), new Vector2(25, 15) , new Vector2(25, 14) , new Vector2(25, 13), new Vector2(25,12), new Vector2(25, 11) , new Vector2(25, 10) , new Vector2(25, 9), new Vector2(25,8), new Vector2(25, 7) ,
            new Vector2(25, 3), new Vector2(25,2)};

        private Vector2[] frontFenceLocations = new Vector2[] { new Vector2(6, 16), new Vector2(8, 16) , new Vector2(10, 16) ,  new Vector2(12, 16) , new Vector2(12, 16) , new Vector2(18, 16), new Vector2(20,16), new Vector2(22, 16) ,
            new Vector2(24, 16), new Vector2(24, 2) , new Vector2(22, 2) ,  new Vector2(20, 2), new Vector2(18, 2) , new Vector2(12, 2) , new Vector2(10, 2), new Vector2(8,2), new Vector2(6, 2)};

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
                
                this.TileGap = new Vector2(lengthX/4, lengthY/4);

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

                        // Top section where white house is
                        if((y - lengthY / 4 >= 16 && y - lengthY / 4 <= 19))
                        {
                            tile.isBuildable = false;
                            tile.isWalkable = false;
                        }

                        // Sides of fence
                        if (x - lengthX / 4 >= 26 || x - lengthX / 4 <= 4 || y - lengthY / 4 <= 1)
                        {
                            tile.isBuildable = false;
                        }

                        // Stop walking through fence
                        if ((x - lengthX / 4 == 4 && (y - lengthY / 4 != 6 && y - lengthY / 4 != 5)) ||
                            (x - lengthX / 4 == 26 && (y - lengthY / 4 != 6 && y - lengthY / 4 != 5)) ||
                            (y - lengthY / 4 == 1 && (x - lengthX / 4 != 13 && x - lengthX / 4 != 17 && x - lengthX / 4 != 15)))
                        {
                            tile.isWalkable = false;
                        }

                    }
                }

                foreach (var spawn in this.EnemySpawnTileIndicies)
                {
                    this.tiles[spawn.XInt() + lengthX / 4, spawn.YInt() + lengthY / 4].isBuildable = false;
                }

                this.tiles[DestinationNode.XInt() + lengthX / 4, DestinationNode.YInt() + lengthY / 4].doodads.AddFirst(Resources.Load<GameObject>("Doodads/White House with Trump/White House with Trump"));
                this.tiles[DestinationNode.XInt() + lengthX / 4, DestinationNode.YInt() + lengthY / 4].isBuildable = false;

                // Fences
                foreach (var location in leftDownFenceLocations)
                {
                    this.tiles[location.XInt() + lengthX / 4, location.YInt() + lengthY / 4].doodads.AddFirst(Resources.Load<GameObject>("Doodads/Fence/FenceDown"));
                    this.tiles[location.XInt() + lengthX / 4, location.YInt() + lengthY / 4].isBuildable = false;
                    this.tiles[location.XInt() + lengthX / 4, location.YInt() + lengthY / 4].isWalkable = false;
                }
                foreach (var location in frontFenceLocations)
                {
                    this.tiles[location.XInt() + lengthX / 4, location.YInt() + lengthY / 4].doodads.AddFirst(Resources.Load<GameObject>("Doodads/Fence/FenceFront"));
                    this.tiles[location.XInt() + lengthX / 4, location.YInt() + lengthY / 4].isBuildable = false;
                    this.tiles[location.XInt() + lengthX / 4, location.YInt() + lengthY / 4].isWalkable = false;
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