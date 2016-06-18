using UnityEngine;
using System.Collections;

public class Manager : MonoBehaviour {
    private float _lastTime;
    public float IdleTime;

    // Use this for initialization
    void Awake () {
        _lastTime = 0;
    }
	
	// Update is called once per frame
	void Update () {
        CalcIdle();
    }

    private void CalcIdle() {
        if (!Input.anyKey) {
            var delta = Time.time - _lastTime;
            if (delta > IdleTime) {
                Time.timeScale = 0.2f;
            }
        }
        else {
            _lastTime = Time.time;
            Time.timeScale = 1f;
        }
    }

}
