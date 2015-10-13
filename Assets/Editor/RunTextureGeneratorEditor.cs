using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RunTextureGenerator))]
public class RunTextureGeneratorEditor: Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		RunTextureGenerator myScript = (RunTextureGenerator)target;
		if(GUILayout.Button("Build Texture"))
		{
			myScript.GetTexture();
		}
	}
}