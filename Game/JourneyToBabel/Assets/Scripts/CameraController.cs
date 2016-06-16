using UnityEngine;
using System.Collections;
using UnityEditor;

public class CameraController : MonoBehaviour {

    public GameObject CharacterManager;
    public float OffsetDistance = 10;
    public Vector3 HeadOffset;
    public float moveSpeed;  //摄像机移动速度，主要针对拐角突变


    private CharacterManager _characterManager;
    private Vector3 _viewdirection = Vector3.zero;
    private Vector3 _expectedViewDirection = Vector3.zero;
    private Vector3 _expectedViewPoint;
    private Vector3 _playerPosition;

    public Vector3 Viewdirection {
        get { return _viewdirection; }
    }


    // Use this for initialization
	void Awake () {
        HeadOffset = new Vector3(0,1,0);
	    _characterManager = CharacterManager.GetComponent<CharacterManager>();
	}
	
	// Update is called once per frame
	void Update () {
        _playerPosition = _characterManager.Player.Object.transform.position;
        SetExpectedViewPoint();
	    MoveCamera();
	}

    //渐变移动
    private void MoveCamera() {
        Vector3 dis = _expectedViewPoint - transform.position;
        float movedis = moveSpeed*Time.deltaTime;
        if (dis.magnitude < movedis) {
            transform.position = _expectedViewPoint;
            _viewdirection = _expectedViewPoint;
        }
        else {
            transform.position = transform.position + dis.normalized*movedis;
        }
        transform.LookAt(_playerPosition);
    }

    private void SetExpectedViewPoint() {

        _expectedViewDirection = new Vector3(0, 0, 0);
        if (Mathf.Abs(_playerPosition.x) > Mathf.Abs(_playerPosition.z))
        {
            _expectedViewDirection.x = _playerPosition.x < 0 ? -1 : 1;
        }
        else {
            _expectedViewDirection.z = _playerPosition.z < 0 ? -1 : 1;
        }

        _expectedViewPoint = _playerPosition + (_expectedViewDirection + HeadOffset * 0.5f) * OffsetDistance;

    }



}
