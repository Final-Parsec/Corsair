namespace FinalParsec.Corsair.Maps
{
    using System;
    using UnityEngine;

    /// <summary>
    ///     Maps a tile texture image to a friendly unique name.
    /// </summary>
    [Serializable]
    public struct TileTexture
    {
        public string name;
        public Texture2D image;
    }
}