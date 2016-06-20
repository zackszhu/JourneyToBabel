using System;
using UnityEngine;
using System.Collections;

public class FreezeTrap : MonoBehaviour {
    public float FreezeDuration;

    private Color _freezingColor;
    private bool _worked;

    void Awake() {
        _freezingColor = new Color(76/255f, 114/255f, 188/255f);
        GetComponent<MeshRenderer>().material.SetColor("_Color", _freezingColor);
        _worked = false;
    }

    public void Work() {
        _worked = true;
    }

    void OnTriggerEnter(Collider collision) {
        if (_worked && collision.gameObject.tag.Equals("Player")) {
            StartCoroutine(Freeze(collision.GetComponent<CharacterMove>()));
        }
    }

    IEnumerator Freeze(CharacterMove cm) {
        foreach (var componentInChild in cm.GetComponentsInChildren<SkinnedMeshRenderer>()) {
            componentInChild.material.SetColor("_Color", _freezingColor);
        }
        cm.Moveable = false;
        yield return new WaitForSeconds(FreezeDuration);
        try {
            cm.Moveable = true;
            foreach (var componentInChild in cm.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                componentInChild.material.SetColor("_Color", cm.GetComponent<ColorSelf>().Color);
            }
        }
        catch (Exception) {
            Debug.Log("Freeze Error");
        }

    }
}
