using Assets.Scripts.StateMachines;
using UnityEngine;

public class CharacterMove : MonoBehaviour {
    public float Speed; //移动速度
    public float JumpSpeed; //起跳速度

    public GameObject MapManager;


    //private Rigidbody _characteRigidbody;
    private CharacterController _characterController; //角色控制用rigidbody会出现很奇怪的问题
    private MapManager _mapManager;
    private CharacterFlagMachine _flagMachine;
    private Animator _animator;
    private Vector3 previous;

    private Vector3 _moveVelocity;
    //  private Vector3 

    // Use this for initialization
    private void Awake() {
        //_characteRigidbody = GetComponent<Rigidbody>();
        MapManager = GameObject.Find("MapManager");
        _characterController = GetComponent<CharacterController>();
        _flagMachine = GetComponent<CharacterFlagMachine>();
        _animator = GetComponent<Animator>();
        _moveVelocity = Vector3.zero;
        _mapManager = MapManager.GetComponent<MapManager>();
    }

    // Update is called once per frame
    private void Update() {
        // Debug.Log("start:" + Time.time + ":" + _characteRigidbody.velocity.y);
        _moveVelocity.x = 0;
        _moveVelocity.z = 0;
        if (_characterController.isGrounded) {
            //可能不只是落地，也可能是碰上面
            _moveVelocity.y = 0;
            _flagMachine.Action(CharacterCommand.Grounded);
        }

        _moveVelocity.y -= 9.81f*Time.deltaTime;


        if (_flagMachine.Flags[(int) CharacterProcessState.Jump]) {
            HandleJump();
        }

        if (_flagMachine.Flags[(int) CharacterProcessState.Walk]) {
            HandleMovement(_flagMachine.MoveReg);
        }

        //Debug.Log("move:"+_moveVelocity);

        _characterController.Move(_moveVelocity*Time.deltaTime);
    }

    private void HandleMovement(Vector3 direction) {
        Turn(direction);
        Move(direction);
    }

    private void HandleJump() {
        if (!_flagMachine.Flags[(int) CharacterProcessState.SJump]) {
            //_characteRigidbody.AddForce(Vector3.up * JumpSpeed);           
            _moveVelocity.y = JumpSpeed;
            //var v = _characteRigidbody.velocity;
            // _characteRigidbody.velocity = new Vector3(v.x, JumpSpeed,v.z);
        }


        _flagMachine.Action(CharacterCommand.Jumped);
    }

    private void HandleTransfer() {
    }

    private void Turn(Vector3 direction) {
        if (direction.magnitude > 0.1) {
            transform.rotation = Quaternion.LookRotation(direction);
            //_characteRigidbody.MoveRotation(Quaternion.LookRotation(direction));
        }
    }

    private void Move(Vector3 direction) {
        Vector3 horiV = direction.normalized*Speed;
        _moveVelocity.x = horiV.x;
        _moveVelocity.z = horiV.z;
    }

    private void OnCollisionEnter(Collision collision) {
        //Debug.Log("coll:"+Time.time + _characteRigidbody.velocity);
        if (collision.gameObject.tag == "GameMap") {
            _flagMachine.Action(CharacterCommand.Grounded);
        }
    }
}
