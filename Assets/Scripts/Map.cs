using UnityEngine;
using System.Collections;

public class Map : MonoBehaviour
{
	public GameObject enemyDeathInt;
    
	public Texture healthTexture;

    // Grid/Node
    private ObjectManager objectManager;
    
	[HideInInspector]
	public GoogleMobileAdsScript ad;

	void Awake()
	{
	    this.objectManager = ObjectManager.GetInstance ();
	    this.ScaleAndPlaceMap ();
	}
	
	// Use this for initialization
	void Start () {
	    this.ad = GameObject.FindGameObjectWithTag("Ad").GetComponent<GoogleMobileAdsScript>();
	    this.ad.RequestInterstitial ();

	    this.LoadMapTexture ();
	    this.PlaceDoodads ();
	}
    
	void OnGUI ()
	{
		foreach (EnemyBase Gob in this.objectManager.ThingsWithHealthBars()) {
			//Health Bar
			float healthRatio = (((float)Gob.Health) / ((float)Gob.maxHealth));
			if (healthRatio != 1) {
				Vector2 objSize = Gob.GetPixelSize ();
				float width = Gob.healthBarSize.x;
				float height = Gob.healthBarSize.y;
	
				width = width * healthRatio;
				Vector3 wantedPos = Camera.main.WorldToScreenPoint (Gob.transform.position);
	
				GUI.color = new Color (2 * (1 - healthRatio), 2 * healthRatio, 0);
				GUI.DrawTexture (new Rect (wantedPos.x - width / 2, Screen.height - wantedPos.y - objSize.y / 2, width, height), this.healthTexture);
			}
		}
	}

	public void ScaleAndPlaceMap()
	{

		Node firstNode = this.objectManager.NodeManager.nodes [0, 0];
		Node lastNode = this.objectManager.NodeManager.nodes [this.objectManager.NodeManager.size_x - 1, this.objectManager.NodeManager.size_y - 1];
		
		Vector3 center = new Vector3 ((lastNode.UnityPosition.x + firstNode.UnityPosition.x) / 2f,
		                              firstNode.UnityPosition.y - 10,
		                              (lastNode.UnityPosition.z + firstNode.UnityPosition.z) / 2f);

		int xIndex = -1;
		int yIndex = -1;
        float textureSizeX = this.objectManager.MapData.Tiles.GetLength(0);
        float textureSizeY = this.objectManager.MapData.Tiles.GetLength(1);
        for (int x = 0; x< textureSizeX; x++)
		{
			for(int y = 0; y< textureSizeY; y++)
			{
				if(xIndex == -1 && this.objectManager.MapData.Tiles[x,y].isNode){
					xIndex = x;
					yIndex = y;
					
					x = (int)textureSizeX;
					y = (int)textureSizeY;
				}
			}
		}


		center.z = center.z + (((textureSizeY - this.objectManager.NodeManager.size_y) / 2f) - yIndex ) * this.objectManager.MapData.NodeSize.y;
		center.x = center.x + (((textureSizeX - this.objectManager.NodeManager.size_x) / 2f) - xIndex) * (this.objectManager.MapData.NodeSize.x / (this.objectManager.MapData.IsIsoGrid?2:1));

	    this.transform.position = center;
	    this.transform.localScale = new Vector3 (((textureSizeX + 1) * this.objectManager.MapData.NodeSize.x) / (this.objectManager.MapData.IsIsoGrid?2:1),
		                                    (textureSizeY + .5f) * this.objectManager.MapData.NodeSize.y,
		                                    1);
	}

	public void PlaceDoodads()
	{
		for(var x = 0; x < this.objectManager.MapData.Tiles.GetLength(0); x++)
		{
			for(var y = 0; y < this.objectManager.MapData.Tiles.GetLength(1); y++)
			{
				var tile = this.objectManager.MapData.Tiles[x, y];
			    if (tile.doodads.Count <= 0)
			    {
			        continue;
			    }

			    foreach(var doodad in tile.doodads)
			    {
			        var doodadPosition = this.objectManager.NodeManager.nodes[x - this.objectManager.MapData.TileGap.XInt(), y - this.objectManager.MapData.TileGap.YInt()].UnityPosition;
                    doodadPosition.y = doodadPosition.y + doodad.transform.position.y; 
                    doodadPosition = this.objectManager.NodeManager.CorrectInitialPosition(doodadPosition);
                    Instantiate(doodad, doodadPosition, doodad.transform.rotation);
			    }
			}
		}
	}

    public void LoadMapTexture()
    {
        Texture2D[] textures = Resources.LoadAll<Texture2D>(this.objectManager.MapData.MapName+"/mapTextures");
	    this.StartCoroutine (this.Animate(textures));
    }

	IEnumerator Animate(Texture2D[] gridTextures) {
		int itr = 0;
		while(true)
		{
		    this.GetComponent<Renderer>().sharedMaterial.mainTexture = gridTextures[itr%gridTextures.Length];
			itr++;
			yield return new WaitForSeconds(this.objectManager.MapData.AnimationSpeed);
		}
	}
}
