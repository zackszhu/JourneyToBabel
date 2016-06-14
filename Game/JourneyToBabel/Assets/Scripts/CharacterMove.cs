using UnityEngine;
using System.Collections;
using Assets.Scripts.StateMachines;
using UnityEditor.Animations;

public class CharacterMove : MonoBehaviour {
    public float Speed;      //移动速度
    public float JumpSpeed;  //起跳速度

    private Rigidbody _characteRigidbody;
    private CharacterFlagMachine _flagMachine;
    private Animator _animator;
    private Vector3 _moveDirection;
    private Vector3 previous;

    // Use this for initialization
    void Awake () {

        _characteRigidbody = GetComponent<Rigidbody>();
        _flagMachine = GetComponent<CharacterFlagMachine>();
        _animator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
       // Debug.Log("start:" + Time.time + ":" + _characteRigidbody.velocity.y);
        if (_flagMachine.Flags[(int) CharacterProcessState.Walk]) {
	        HandleMovement(_flagMachine.MoveReg);
	    }
	    if (_flagMachine.Flags[(int) CharacterProcessState.Jump]) {
	        HandleJump();
	    }
        _characteRigidbody.velocity -= new Vector3(0, 9.8f * Time.deltaTime, 0);
        //Debug.Log("final:"+Time.time+":" + _characteRigidbody.velocity.y);
    }

    void HandleMovement(Vector3 direction) {
        Turn(direction);
        Move(direction);
        
    }

    void HandleJump() {
        if (!_flagMachine.Flags[(int) CharacterProcessState.SJump]) {
            //_characteRigidbody.AddForce(Vector3.up * JumpSpeed);

            var v = _characteRigidbody.velocity;  //Vector3的等于是会复制对象，而不是引用对象。

            _characteRigidbody.velocity = new Vector3(v.x, JumpSpeed, v.z);
        }
        _flagMachine.Action(CharacterCommand.Jumped);
    }

    void Turn(Vector3 direction) {
        if (direction.magnitude > 0.1) {
            _characteRigidbody.MoveRotation(Quaternion.LookRotation(direction));
        }
    }

    void Move(Vector3 direction) {
        /*var movement = direction.normalized * Speed*1000;
        _characteRigidbody.AddForce(movement);*/
        
        //var v = direction.normalized*Speed;
        var v = _characteRigidbody.velocity;
       // Debug.Log("direction:" + direction);
        //Debug.Log("Speed:" + Speed);
        var horiV = direction.normalized * 1;
        //Debug.Log("horiV:" +horiV);
        
        _characteRigidbody.velocity = new Vector3(horiV.x, v.y, horiV.z);
        //v.Set(horiV.x, v.y, horiV.z);            //修改水平运动
       
        


        /*
        var movement = direction.normalized * Speed  * Time.timeScale;
        _characteRigidbody.MovePosition(transform.position + movement);*/
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "GameMap") {
            _flagMachine.Action(CharacterCommand.Grounded);
        }
    }
}
