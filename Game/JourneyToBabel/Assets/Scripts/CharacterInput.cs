using UnityEngine;
using System.Collections;
using Assets.Scripts.StateMachines;

public class CharacterInput : MonoBehaviour {


    public GameObject MapManager;
    public GameObject CharacterManager;



    private MapManager _mapManager;
    private CharacterManager _characterManager;



    // Use this for initialization
    void Awake () {
        //MapManager = GameObject.Find("MapManager");
        _mapManager = MapManager.GetComponent<MapManager>();
        _characterManager = CharacterManager.GetComponent<CharacterManager>();
    }

    
    // Update is called once per frame
	void Update () {
        HandleArrows();
        HandleJump();
	    HandleTransfer();
	}

    void HandleTransfer() {
        var isTransfer = Input.GetKeyDown(KeyCode.T);

        if (isTransfer)
        {
            _characterManager.PlayerTransfer();
        }

        //throw new System.NotImplementedException();
    }

    void HandleArrows() {
        var direction = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        if (direction.magnitude > 0.01) {
            _characterManager.Player.Walk(direction);
           // _animator.SetBool("isWalking", true);
           // _flagMachine.Action(CharacterCommand.MoveBegin, direction);
        }
        else {
            _characterManager.Player.StopWalk();
           // _animator.SetBool("isWalking", false);
           // _flagMachine.Action(CharacterCommand.MoveEnd);
        }
    }

    void HandleJump() {
        var isJump = Input.GetKeyDown(KeyCode.Space);
        if (isJump) {
            _characterManager.Player.Jump();
        }
    }
}
