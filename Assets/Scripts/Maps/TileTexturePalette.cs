namespace FinalParsec.Corsair.Maps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.UI;

    public class TileTexturePalette : MonoBehaviour
    {
        /// <summary>
        ///     Used to keep track of the current page / batch of textures being displayed.
        /// </summary>
        private int currentPage;

        /// <summary>
        ///     The text displaying the unique name of the currently selected turret.
        /// </summary>
        private Text selectedTileTextureName;

        /// <summary>
        ///     A preview image of the currently selected tile texture.
        /// </summary>
        private RawImage selectedTileTexturePreview;

        /// <summary>
        ///     All the textures to use. Configured in the Unity editor.
        /// </summary>
        public TileTexture[] textures;

        /// <summary>
        ///     Reference to all the buttons which can be used to select a tile texture.
        /// </summary>
        private List<TileTextureButton> tileTextureButtons;

        /// <summary>
        ///     Called on the frame when a script is enabled before any of the Update methods are called the first time.
        ///     Use this for initialization.
        /// </summary>
        protected void Start()
        {
            this.selectedTileTextureName = GameObject.FindGameObjectWithTag(Tags.SelectedTileTexturePanel)
                .transform.Find("Name")
                .GetComponent<Text>();

            this.selectedTileTexturePreview = GameObject.FindGameObjectWithTag(Tags.SelectedTileTexturePanel)
                .transform.Find("Preview Image")
                .GetComponent<RawImage>();

            if (this.selectedTileTexturePreview == null)
            {
                Debug.Log("that shit is null, bruh");
            }

            var buttonLookup =
                from tileTextureButton in GameObject.FindGameObjectsWithTag(Tags.TileTextureButton)
                select new TileTextureButton
                {
                    Button = tileTextureButton.GetComponent<Button>(),
                    Image = tileTextureButton.transform.Find("Preview Image").GetComponent<RawImage>(),
                    Name = tileTextureButton.transform.Find("Name").GetComponent<Text>()
                };
            this.tileTextureButtons = buttonLookup
                .OrderByDescending(ttb => ttb.Button.transform.position.y)
                .ToList();

            foreach (var tileTextureButton in this.tileTextureButtons)
            {
                var buttonName = tileTextureButton.Name;
                tileTextureButton.Button.onClick.AddListener(() => { SelectTileTexture(buttonName); });
            }

            this.SetPage(0);
            this.SelectTileTexture(this.tileTextureButtons.Select(ttb => ttb.Name).First());

            // Disable the menu until it is needed.
            this.gameObject.SetActive(false);
        }

        /// <summary>
        ///     Selects a tile texture given its unique name.
        /// </summary>
        /// <param name="tileTextureName">
        ///     The unique name of the tile texture.
        /// </param>
        private void SelectTileTexture(Text tileTextureName)
        {
            this.selectedTileTextureName.text = tileTextureName.text;
            var selectedTexture = textures.First(t => t.name == tileTextureName.text).image;
            this.selectedTileTexturePreview.texture = selectedTexture;
        }

        /// <summary>
        ///     Sets the current page of the tile texture palette.
        /// </summary>
        /// <param name="pageNumber">
        ///     The page to set.
        ///     Numbering of pages starts at 0.
        /// </param>
        private void SetPage(int pageNumber)
        {
            var maxPagesAllowed = textures.Count() % this.tileTextureButtons.Count == 0
                ? textures.Count() / this.tileTextureButtons.Count
                : textures.Count() / this.tileTextureButtons.Count + 1;
            if (pageNumber < 0 ||
                pageNumber > maxPagesAllowed - 1)
            {
                throw new ArgumentException("Page number is not valid.");
            }

            currentPage = pageNumber;
            var texturesForThisPage = textures
                .Skip(currentPage * this.tileTextureButtons.Count)
                .Take(this.tileTextureButtons.Count)
                .ToList();
            var textureQueue = new Queue<TileTexture>(texturesForThisPage);

            foreach (var tileTextureButton in this.tileTextureButtons)
            {
                if (textureQueue.Count > 0)
                {
                    tileTextureButton.Button.gameObject.SetActive(true);
                    var tileTexture = textureQueue.Dequeue();
                    tileTextureButton.Image.texture = tileTexture.image;
                    tileTextureButton.Name.text = tileTexture.name;
                }
                else
                {
                    tileTextureButton.Image.texture = null;
                    tileTextureButton.Name.text = "";
                    tileTextureButton.Button.gameObject.SetActive(false);
                }
            }
        }
    }
}