using UnityEngine;
using System.Collections;
using Assets.Scripts.StateMachines;

public class CharacterInput : MonoBehaviour {
    public GameObject CharacterObject;

    private CharacterFlagMachine _flagMachine;
    private Animator _animator;
    // Use this for initialization
    void Awake () {

	}

    public void SetCharacterObject(GameObject characterObject) {
        CharacterObject = characterObject;
        _flagMachine = CharacterObject.GetComponent<CharacterFlagMachine>();
        _animator = CharacterObject.GetComponent<Animator>();
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
            _flagMachine.Action(CharacterCommand.TransferBegin);
        }

        //throw new System.NotImplementedException();
    }

    void HandleArrows() {
        var direction = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        if (direction.magnitude > 0.01) {
            _animator.SetBool("isWalking", true);
            _flagMachine.Action(CharacterCommand.MoveBegin, direction);
        }
        else {
            _animator.SetBool("isWalking", false);
            _flagMachine.Action(CharacterCommand.MoveEnd);
        }
    }

    void HandleJump() {
        var isJump = Input.GetKeyDown(KeyCode.Space);
        if (isJump) {
            _animator.SetTrigger("jump");
            _flagMachine.Action(CharacterCommand.Jump);
        }
    }
}
