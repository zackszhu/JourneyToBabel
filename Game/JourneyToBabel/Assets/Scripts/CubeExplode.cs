using UnityEngine;
using System.Collections;

public class CubeExplode : MonoBehaviour {
    public GameObject Particle;
    private bool _explode;

    private float _startTime;
    private bool _worked;

    void Awake() {
        _explode = false;
        _worked = false;
    }

    void Update() {
        if (_explode) {
            if (_startTime == 0) {
                _startTime = Time.time;
            }
            var rate = 1 - (Time.time - _startTime);
            //Debug.Log(Mathf.Lerp(0, 1, rate));
            GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(1, Mathf.Lerp(0, 1, rate), Mathf.Lerp(0, 1, rate), 1));
            if (rate < 0) {
                Instantiate(Particle, transform.position, Quaternion.identity);
                gameObject.SetActive(false);
                //Destroy(gameObject);
            }
        }
    }

    public void Work() {
        _worked = true;
    }

    void OnTriggerEnter(Collider collision) {
        if (_worked && collision.gameObject.tag.Equals("Player")) {
            _explode = true;
        }
    }

}
