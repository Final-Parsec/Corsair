namespace FinalParsec.Corsair.Maps.MapEditor
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text.RegularExpressions;
    using UnityEngine;

    /// <summary>
    ///     Loads and stores maps to and from .mapdata files.
    /// </summary>
    internal class MapLoader : IMapLoader
    {
        /// <summary>
        ///     The file extension for the map data files stored on the file system.
        /// </summary>
        private const string MapDataFileExtension = ".mapdata";

        /// <summary>
        ///     The singlteton instance of the <see cref="MapLoader" /> class.
        /// </summary>
        private static IMapLoader instance = new MapLoader();

        private MapLoader()
        {
            this.CurrentMap = new MapData(null);
        }

        /// <summary>
        ///     Gets the singleton instance of the <see cref="MapLoader" /> class.
        /// </summary>
        public static IMapLoader Instance
        {
            get { return MapLoader.instance; }
            private set { MapLoader.instance = value; }
        }

        /// <summary>
        ///     Gets the currently selected map.
        /// </summary>
        public IMapData CurrentMap { get; private set; }

        /// <summary>
        ///     Loads the map identified by the <see cref="mapName" /> from a data store into the <see cref="CurrentMap" />
        ///     property.
        /// </summary>
        /// <param name="mapName">
        ///     The unique name of the map to be loaded.
        /// </param>
        public void LoadMap(string mapName)
        {
            var mapFilePath = GetPathForMap(mapName);
            if (!File.Exists(mapFilePath))
            {
                throw new ArgumentOutOfRangeException("mapName", @"Map with this map was not found.");
            }

            var binaryFormatter = new BinaryFormatter();
            var file = File.Open(mapFilePath, FileMode.Open);
            this.CurrentMap = (MapData) binaryFormatter.Deserialize(file);
            file.Close();
        }

        /// <summary>
        ///     Persists the <see cref="CurrentMap" /> to a data store.
        /// </summary>
        /// <param name="mapName">
        ///     A name for the map to be saved.
        ///     If a map with the same name already exists, it will be overwritten.
        /// </param>
        public void SaveCurrentMap(string mapName)
        {
            this.CurrentMap.MapName = mapName;

            var binaryFormatter = new BinaryFormatter();
            var file = File.Create(GetPathForMap(mapName));
            binaryFormatter.Serialize(file, this.CurrentMap);
            file.Close();
        }

        /// <summary>
        ///     Constructs and returns a full path for the map based on the map name specified.
        ///     This method also performs validation of the map name.
        /// </summary>
        /// <param name="mapName">
        ///     The name of the map.
        /// </param>
        /// <returns>
        ///     The full path for the map.
        /// </returns>
        private string GetPathForMap(string mapName)
        {
            Debug.Log(mapName);
            if (mapName == null)
            {
                throw new ArgumentNullException("mapName", @"You must supply a map name.");
            }

            var containsABadCharacter = new Regex("[" + Regex.Escape(new string(Path.GetInvalidFileNameChars())) + "]");
            if (string.IsNullOrEmpty(mapName) ||
                containsABadCharacter.IsMatch(mapName))
            {
                throw new ArgumentException(@"The map name is not valid.", "mapName");
            }

            Debug.Log(Application.persistentDataPath + Path.DirectorySeparatorChar + mapName + MapDataFileExtension);
            return Application.persistentDataPath + Path.DirectorySeparatorChar + mapName + MapDataFileExtension;
        }
    }
}