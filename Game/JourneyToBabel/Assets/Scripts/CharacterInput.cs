using UnityEngine;
using System.Collections;
using Assets.Scripts.StateMachines;

public class CharacterInput : MonoBehaviour {
    public GameObject CharacterObject;

    [HideInInspector]
    private CharacterFlagMachine _flagMachine;

	// Use this for initialization
	void Awake () {
	    _flagMachine = CharacterObject.GetComponent<CharacterFlagMachine>();
	}
	
	// Update is called once per frame
	void Update () {
        HandleArrows();
        HandleJump();
	}

    void HandleArrows() {
        var direction = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        if (direction.magnitude > 0.01) {
            Debug.Log(direction);
            _flagMachine.Action(CharacterCommand.MoveBegin, direction);
        }
        else {
            _flagMachine.Action(CharacterCommand.MoveEnd);
        }
    }

    void HandleJump() {
        var isJump = Input.GetKeyDown(KeyCode.Space);
        if (isJump) {
            _flagMachine.Action(CharacterCommand.Jump);
        }
    }
}
