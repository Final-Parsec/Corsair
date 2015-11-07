namespace FinalParsec.Corsair.Maps
{
    using UnityEngine.UI;

    /// <summary>
    ///     Represents the components which make up a tile texture button the in the tile texture palette.
    /// </summary>
    public class TileTextureButton
    {
        /// <summary>
        ///     Gets or sets the button which triggers tile texture selection events.
        /// </summary>
        public Button Button { get; set; }

        /// <summary>
        ///     Gets or sets the preview of the tile texture displayed in the button.
        /// </summary>
        public RawImage Image { get; set; }

        /// <summary>
        ///     Gets or sets the unique name of the tile texture.
        /// </summary>
        public Text Name { get; set; }
    }
}