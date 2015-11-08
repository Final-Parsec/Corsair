namespace FinalParsec.Corsair
{
    /// <summary>
    ///     Defines tags used inside the Unity editor which decorate game objects.
    /// </summary>
    public class Tags
    {
        /// <summary>
        ///     Object decorated with this tag is a panel which contains UI elements describing the currently selected tile
        ///     texture.
        /// </summary>
        public const string SelectedTileTexturePanel = "SelectedTileTexturePanel";

        /// <summary>
        ///     Object decorated with this tag is a panel which contains UI elements describing the currently selected turret.
        /// </summary>
        public const string SelectedTurretPanel = "SelectedTurretPanel";

        /// <summary>
        ///     Objects decorated with this tag are buttons used to select a tile texture (map editing).
        /// </summary>
        public const string TileTextureButton = "TileTextureButton";

        /// <summary>
        ///     Objects decorated with this tag are buttons used to upgrade turrets (turret focus menu).
        /// </summary>
        public const string UpgradeButton = "UpgradeButton";

        /// <summary>
        ///     Object decorated with this tag is panel containing turret upgrade buttons (turret focus menu).
        /// </summary>
        public const string UpgradePanel = "UpgradePanel";
    }
}