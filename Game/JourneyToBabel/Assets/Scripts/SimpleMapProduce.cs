using UnityEngine;
using System.Collections;
using System.Xml.Schema;
using Assets.Scripts;

public class SimpleMapProduce : MonoBehaviour {
    public GameObject MapCubePrefab;
    public GameObject CharacterCubePrefab;
    public GameObject InputManager;

    public int MapLength = 10;
    public int MapWidth = 10;
    public int Layer = 10;
    

    private MapManager _mapManager;
	// Use this for initialization
	void Awake () {
        _mapManager = new MapManager(MapLength,MapWidth);
        /*
	    for (int layer = 5; layer > 0; layer--) {
	        for (int x = -layer + 1; x < layer; x++) {
	            for (int y = -layer + 1; y < layer; y++) {
                    Instantiate(MapCubePrefab, new Vector3(x, 5 - layer, y), Quaternion.identity);
                }
            }
	    }*/
        for (int i=0; i < Layer; i++)
            _mapManager.AppendLayer();

        

	    for (int layer = 0; layer < _mapManager.LayerCount; layer++)
	    {
	        var layerData = _mapManager.GetLayer(layer);
            for (int i =1 ; i <= MapLength; i++)
                for (int j = 1; j <= MapWidth; j++)
                {
                    if (layerData[i,j].IsValid)
                        Instantiate(MapCubePrefab, new Vector3(i - MapLength / 2 -1 , layer, j - MapWidth / 2 - 1 ),Quaternion.identity);
                }
	    }

        
	    var charactGameObject = Instantiate(CharacterCubePrefab, new Vector3(4, 50, 4), Quaternion.identity) as GameObject;
	    GameObject.Find("InputManager").GetComponent<CharacterInput>().CharacterObject = charactGameObject;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
