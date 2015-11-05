namespace FinalParsec.Corsair
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof (RunTextureGenerator))]
    public class RunTextureGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var myScript = (RunTextureGenerator) target;
            if (GUILayout.Button("Build Texture"))
            {
                myScript.GetTexture();
            }
        }
    }
}