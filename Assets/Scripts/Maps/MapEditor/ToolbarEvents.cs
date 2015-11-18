namespace FinalParsec.Corsair.Maps.MapEditor
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    ///     Class for event handlers on UI elements in the map editor's toolbar.
    /// </summary>
    public class ToolbarEvents : MonoBehaviour
    {
        /// <summary>
        ///     A reference to the map name text field.
        /// </summary>
        /// <remarks>
        ///     This component is to be set in the Unity editor.
        /// </remarks>
        public GameObject mapNameTextField;

        /// <summary>
        ///     Retrieves the current value in the map name text field.
        /// </summary>
        private string MapName
        {
            get
            {
                return mapNameTextField.GetComponent<Text>().text;
            }
        }

        /// <summary>
        ///     Loads the map using the name specified in the map name text field.
        /// </summary>
        public void LoadMap()
        {
            var mapLoader = MapLoader.Instance;
            mapLoader.LoadMap(this.MapName);
        }

        /// <summary>
        ///     Saves the current map using the name specified in the map name text field.
        /// </summary>
        public void SaveMap()
        {
            var mapLoader = MapLoader.Instance;
            mapLoader.SaveCurrentMap(this.MapName);
        }
    }
}