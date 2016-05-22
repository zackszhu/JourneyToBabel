using UnityEngine;
using System.Collections;
using Debug = System.Diagnostics.Debug;

public class SimpleMapProduce : MonoBehaviour {
    public GameObject MapCubePrefab;
    public GameObject CharacterCubePrefab;
    public GameObject InputManager;

	// Use this for initialization
	void Awake () {
	    for (int layer = 5; layer > 0; layer--) {
	        for (int x = -layer + 1; x < layer; x++) {
	            for (int y = -layer + 1; y < layer; y++) {
                    Instantiate(MapCubePrefab, new Vector3(x, 5 - layer, y), Quaternion.identity);
                }
            }
	    }

	    var charactGameObject = Instantiate(CharacterCubePrefab, new Vector3(4, 2, 4), Quaternion.identity) as GameObject;
	    InputManager.GetComponent<CharacterInput>().CharacterMove = charactGameObject.GetComponent<CharacterMove>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
