using UnityEngine;
using System.Collections;

public class SpeedAlter : MonoBehaviour {
    public float AlteredSpeed;

    private bool _worked;

    void Awake() {
        _worked = false;
    }

    public void Work() {
        if (AlteredSpeed < 2) {
            GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(0.5f, 0.5f, 0.5f));
        }
        else if (AlteredSpeed > 2) {
            GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(1f, 1f, 0f));
        }
        else {
            GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(1, 1, 1));
        }
        _worked = true;
    }

    void OnTriggerEnter(Collider collision) {
        if (_worked && collision.gameObject.tag.Equals("Player")) {
            collision.gameObject.GetComponent<CharacterMove>().Speed = AlteredSpeed;
        }
    }

    void OnTriggerExit(Collider collision) {
        if (_worked && collision.gameObject.tag.Equals("Player")) {
            collision.gameObject.GetComponent<CharacterMove>().Speed = 2;
        }
    }
}
