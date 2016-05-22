using UnityEngine;
using System.Collections;

public class CharacterInput : MonoBehaviour {
    [HideInInspector]
    public CharacterMove CharacterMove;

	// Use this for initialization
	void Awake () {

	}
	
	// Update is called once per frame
	void FixedUpdate () {
	    CharacterMove.Action(new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")));
	    if (Input.GetKeyDown(KeyCode.Space)) {
	        CharacterMove.Jump();
	    }
	}
}
