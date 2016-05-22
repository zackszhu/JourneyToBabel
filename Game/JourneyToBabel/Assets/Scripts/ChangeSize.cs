using UnityEngine;
using System.Collections;

public class ChangeSize : MonoBehaviour {
    public void Bigger() {
        Debug.Log("B");
        GetComponent<Renderer>().enabled = false;
    }

    public void Smaller() {
        Debug.Log("S");
        GetComponent<Renderer>().enabled = true;
    }
}
