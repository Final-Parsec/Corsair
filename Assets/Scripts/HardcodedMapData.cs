namespace FinalParsec.Corsair
{
    using Assets.Scripts.Maps;
    using UnityEngine;

    /// <summary>
    ///     <see cref="IMapData" /> implementation with configuration data directly in the source code.
    /// </summary>
    public class HardcodedMapData : IMapData
    {
        private readonly Texture2D[] grid;

        /// <summary>
        ///     Backing store for <see cref="Tiles" />.
        /// </summary>
        private Tile[,] tiles;

        public HardcodedMapData(Texture2D[] grid)
        {
            this.grid = grid;
        }

        /// <summary>
        ///     Gets or sets the duration, in seconds, each frame of the map's animation should be displayed.
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
            get { return new Vector2(50, 29); }
        }

        /// <summary>
        ///     Gets the location of the nodes where enemies spawn.
        /// </summary>
        public Vector2[] EnemySpawnTileIndicies
        {
            get
            {
                Vector2[] arr = {new Vector2(5, 5), new Vector2(5, 15), new Vector2(5, 25)};
                return arr;
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
            get { return "Test2"; }
        }

        /// <summary>
        ///     Gets the dimensions of the node map.
        /// </summary>
        public Vector2 NodeSize
        {
            get { return new Vector2(32, 16); }
        }

        /// <summary>
        ///     Gets or sets the 2D tile set.
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
                        var textures = this.grid;
                        var tile = new Tile(textures, false, false, false);
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
            get { return new Vector2(64, 32); }
        }
    }
}