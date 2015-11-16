namespace FinalParsec.Corsair.Maps
{
    /// <summary>
    ///     Contract for a class which is capable of loading and storing maps to and from a data store.
    /// </summary>
    public interface IMapLoader
    {
        /// <summary>
        ///     Gets the currently selected map.
        /// </summary>
        IMapData CurrentMap
        {
            get;
        }

        /// <summary>
        ///     Loads the map identified by the <see cref="mapName" /> from a data store into the <see cref="CurrentMap" /> property.
        /// </summary>
        /// <param name="mapName">
        ///     The unique name of the map to be loaded.
        /// </param>
        void LoadMap(string mapName);

        /// <summary>
        ///     Persists the <see cref="CurrentMap" /> to a data store.
        /// </summary>
        /// <param name="mapName">
        ///     A name for the map to be saved.
        ///     If a map with the same name already exists, it will be overwritten.
        /// </param>
        void SaveCurrentMap(string mapName);
    }
}