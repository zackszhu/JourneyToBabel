using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class HandTrigger : MonoBehaviour {

    private GameObject _currentHanded;
    private GameObject _previousHanded;
    private HashSet<GameObject> _handedQueue;

	// Use this for initialization
	void Awake () {
	    _handedQueue = new HashSet<GameObject>();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
	    if (_currentHanded) {
            _currentHanded.GetComponent<ChangeSize>().Smaller();
        }
        var minDistance = float.MaxValue;
        foreach (var obj in _handedQueue) {
            var distance = Vector3.Distance(transform.position, obj.transform.position);
            if (distance < minDistance) {
                _currentHanded = obj;
                minDistance = distance;
            }
        }

	    if (_currentHanded) {
            _currentHanded.GetComponent<ChangeSize>().Bigger();
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag.Equals("GameMap")) {
            _handedQueue.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject.tag.Equals("GameMap")) {
            _handedQueue.Remove(other.gameObject);
        }
    }
}
