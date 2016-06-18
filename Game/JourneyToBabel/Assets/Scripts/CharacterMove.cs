using System.Reflection;
using Assets.Scripts;
using Assets.Scripts.StateMachines;
using UnityEngine;

public class CharacterMove : MonoBehaviour {
    public float Speed; //移动速度
    public float JumpSpeed; //起跳速度
    public float TransferTime; //传送需要准备的时间
    public bool Moveable;
    
    //public static float CharacterYOffest = 1.0f - 0.455f;



    private CharacterController _characterController; //角色控制用rigidbody会出现很奇怪的问题

    private CharacterFlagMachine _flagMachine;
    private Animator _animator;
    private Vector3 previous;
    private float _transferRemainTime;
    private Vector3 _moveVelocity;
    private Vector3 _tempTagertPosition;
   
    //private Cube TransferTargetCube { get; set; }

    //  private Vector3 

    // Use this for initialization
    private void Awake() {
        //_characteRigidbody = GetComponent<Rigidbody>();
        //MapManager = GameObject.Find("MapManager");
        _characterController = GetComponent<CharacterController>();
        _flagMachine = GetComponent<CharacterFlagMachine>();
        _animator = GetComponent<Animator>();
        _moveVelocity = Vector3.zero;
        _tempTagertPosition = Vector3.zero;

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

        if (_flagMachine.Flags[(int) CharacterProcessState.Transfer]) {
            HandleTransfer(_flagMachine.TransferReg);
        }
        //Debug.Log("move:"+_moveVelocity);

        _characterController.Move(_moveVelocity*Time.deltaTime);
    }

    private void HandleTransfer(Vector3 tragetPosition) {
        if (_tempTagertPosition.Equals(Vector3.zero)) {  //设定了位置，还没有开始传送
            _transferRemainTime = TransferTime;  //传送计时开始
            _tempTagertPosition = tragetPosition;

        }
        else {
            _transferRemainTime -= Time.deltaTime;
            if (_transferRemainTime < 0) {   //传送准备时间完成，直接传送
                gameObject.transform.position = _tempTagertPosition;
                _flagMachine.Action(CharacterCommand.TransferEnd);
                _tempTagertPosition = Vector3.zero;
               
                //_flagMachine.TransferReg = Vector
            }
        }
    }

    private void HandleMovement(Vector3 direction) {
        if (Moveable) {
            Turn(direction);
            Move(direction);
        }
    }

    private Vector3 GetCubePosition() {
        return gameObject.transform.position + CharacterManager.CharacterYOffest;
    }

    private void HandleJump() {
        if (!_flagMachine.Flags[(int) CharacterProcessState.SJump]) {    
            _moveVelocity.y = JumpSpeed;
        }


        _flagMachine.Action(CharacterCommand.Jumped);
    }


    private void Turn(Vector3 direction) {
        if (direction.magnitude > 0.1) {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void Move(Vector3 direction) {
        Vector3 horiV = direction.normalized*Speed * Time.timeScale;
        _moveVelocity.x = horiV.x;
        _moveVelocity.z = horiV.z;
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "GameMap") {
            _flagMachine.Action(CharacterCommand.Grounded);
        }
    }
}
