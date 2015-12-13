namespace FinalParsec.Corsair
{
    using Maps;
    using UnityEngine;

    [RequireComponent(typeof(Renderer))]
    public class RunTextureGenerator : MonoBehaviour
    {
        public Texture2D[] grid;

        public void GetTexture()
        {
            IMapData mapData = new WashingtonDCMapData(grid);

            var textureGenerator = new TextureGenerator();
            var gridTextures = textureGenerator.Generate(mapData);

            GetComponent<Renderer>().sharedMaterial.mainTexture = gridTextures[0];
        }
    }
}