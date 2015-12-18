namespace FinalParsec.Corsair
{
    using Maps;
    using UnityEngine;

    [RequireComponent(typeof(Renderer))]
    public class RunTextureGenerator : MonoBehaviour
    {
        public Texture2D[] tileTextures;

        public Texture2D grid;

        public void GetTexture()
        {
            IMapData mapData = new WashingtonDCMapData(tileTextures, grid);

            var textureGenerator = new TextureGenerator(mapData);
            var gridTextures = textureGenerator.Generate();

            GetComponent<Renderer>().sharedMaterial.mainTexture = gridTextures[0];
        }
    }
}