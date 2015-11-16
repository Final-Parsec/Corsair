namespace FinalParsec.Corsair.Maps
{
    using System;
    using UnityEngine;

    /// <summary>
    ///     Provides information about a map.
    ///     Instances of this class are intended to be serialiable so that they can be saved.
    /// </summary>
    [Serializable]
    internal class MapData : IMapData
    {
        /// <summary>
        ///     Backing field for <see cref="AnimationSpeed" /> property.
        /// </summary>
        [SerializeField]
        private float animationSpeed = .28f;

        /// <summary>
        ///     Backing field for <see cref="DestinationMode" /> property.
        /// </summary>
        [SerializeField]
        private Vector2 destinationMode = new Vector2(50, 29);

        /// <summary>
        ///     Backing field for <see cref="EnemySpawnTileIndicies" /> property.
        /// </summary>
        [SerializeField]
        private Vector2[] enemySpawnTileIndicies = { new Vector2(5, 5), new Vector2(5, 15), new Vector2(5, 25) };

        /// <summary>
        ///     Backing field for <see cref="IsIsoGrid" /> property.
        /// </summary>
        [SerializeField]
        private bool isIsoGrid = true;

        /// <summary>
        ///     Backing field for <see cref="MapName" /> property.
        /// </summary>
        [SerializeField]
        private string mapName = "MyPersistableMapData";

        /// <summary>
        ///     Backing field for <see cref="NodeSize" /> property.
        /// </summary>
        [SerializeField]
        private Vector2 nodeSize = new Vector2(32, 16);

        /// <summary>
        ///     Backing field for <see cref="Tiles" /> property.
        /// </summary>
        private Tile[,] tiles;

        /// <summary>
        ///     Backing field for <see cref="TileSize" /> property.
        /// </summary>
        [SerializeField]
        private Vector2 tileSize = new Vector2(64, 32);

        /// <summary>
        ///     Initializes a new instance of the <see cref="MapData" /> class.
        /// </summary>
        /// <param name="mapData">
        ///     The information to use when initializing the object.
        /// </param>
        public MapData(IMapData mapData)
        {
        }

        /// <summary>
        ///     Gets the duration, in seconds, each frame of the map's animation should be displayed.
        /// </summary>
        public float AnimationSpeed
        {
            get
            {
                return this.animationSpeed;
            }
        }

        /// <summary>
        ///     Gets the location of the destination node (where enemies go).
        /// </summary>
        public Vector2 DestinationNode
        {
            get
            {
                return this.destinationMode;
            }
        }

        /// <summary>
        ///     Gets the location of the nodes where enemies spawn.
        /// </summary>
        public Vector2[] EnemySpawnTileIndicies
        {
            get
            {
                return this.enemySpawnTileIndicies;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the map uses an isometric grid.
        /// </summary>
        public bool IsIsoGrid
        {
            get
            {
                return this.isIsoGrid;
            }
        }

        /// <summary>
        ///     Gets the name of the map.
        /// </summary>
        public string MapName
        {
            get
            {
                return this.mapName;
            }
        }

        /// <summary>
        ///     Gets the dimensions of the node map.
        /// </summary>
        public Vector2 NodeSize
        {
            get
            {
                return this.nodeSize;
            }
        }

        /// <summary>
        ///     Gets or sets the tile texture to use for all tiles.
        ///     This is temporary.
        /// </summary>
        public Texture2D TileTexture
        {
            get;
            set;
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

                const int lengthX = 68;
                const int lengthY = 36;

                this.tiles = new Tile[lengthX, lengthY];

                for (var x = 0; x < lengthX; x++)
                {
                    for (var y = 0; y < lengthY; y++)
                    {
                        var tile = new Tile(new Texture2D[] { this.TileTexture }, false, false, false);
                        this.tiles[x, y] = tile;

                        if (x > 1 && x < lengthX - 6 && y > 1 && y < lengthY - 6)
                        {
                            tile.isBuildable = true;
                            tile.isWalkable = true;
                            tile.isNode = true;
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
                return this.tileSize;
            }
        }
    }
}