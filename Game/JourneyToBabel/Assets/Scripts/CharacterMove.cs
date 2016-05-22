using UnityEngine;
using System.Collections;

public class CharacterMove : MonoBehaviour {
    public float Speed;
    public float JumpSpeed;

    private Rigidbody _characteRigidbody;
    private Vector3 _moveDirection;
    private Vector3 previous;

    // Use this for initialization
    void Awake () {
        _characteRigidbody = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {

        //	    if (_characteRigidbody.velocity.magnitude > 0.1) {
        //            Debug.Log(_characteRigidbody.velocity);
        //        }
    }

    public void Action(Vector3 direction) {
        Turn(direction);
        Move(direction);
    }

    public void Jump() {
//        _characteRigidbody.velocity += new Vector3(0, JumpSpeed, 0);
        _characteRigidbody.AddForce(Vector3.up * JumpSpeed);
    }

    private void Turn(Vector3 direction) {
        if (direction.magnitude > 0.1) {
            _characteRigidbody.MoveRotation(Quaternion.LookRotation(direction));
        }
    }

    private void Move(Vector3 direction) {
        var movement = direction.normalized * Speed * Time.timeScale;
        _characteRigidbody.MovePosition(transform.position + movement);
    }
}
