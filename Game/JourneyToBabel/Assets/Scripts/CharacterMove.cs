using UnityEngine;
using System.Collections;
using Assets.Scripts.StateMachines;

public class CharacterMove : MonoBehaviour {
    public float Speed;
    public float JumpSpeed;

    private Rigidbody _characteRigidbody;
    private CharacterFlagMachine _flagMachine;
    private Vector3 _moveDirection;
    private Vector3 previous;

    // Use this for initialization
    void Awake () {
        _characteRigidbody = GetComponent<Rigidbody>();
        _flagMachine = GetComponent<CharacterFlagMachine>();
    }
	
	// Update is called once per frame
	void Update () {
	    if (_flagMachine.Flags[(int) CharacterProcessState.Walk]) {
	        HandleMovement(_flagMachine.MoveReg);
	    }
	    if (_flagMachine.Flags[(int) CharacterProcessState.Jump]) {
	        HandleJump();
	    }
    }

    void HandleMovement(Vector3 direction) {
        Turn(direction);
        Move(direction);
    }

    void HandleJump() {
        if (!_flagMachine.Flags[(int) CharacterProcessState.SJump]) {
            _characteRigidbody.AddForce(Vector3.up * JumpSpeed);
        }
        _flagMachine.Action(CharacterCommand.Jumped);
    }

    void Turn(Vector3 direction) {
        if (direction.magnitude > 0.1) {
            _characteRigidbody.MoveRotation(Quaternion.LookRotation(direction));
        }
    }

    void Move(Vector3 direction) {
        var movement = direction.normalized * Speed * Time.timeScale;
        _characteRigidbody.MovePosition(transform.position + movement);
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "GameMap") {
            _flagMachine.Action(CharacterCommand.Grounded);
        }
    }
}
