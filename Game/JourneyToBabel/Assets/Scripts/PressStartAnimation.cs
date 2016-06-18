using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PressStartAnimation : MonoBehaviour {
    [Range(0, 1)]
    public float FadeSpeed;

	void Update () {
	    Color v4 = GetComponent<Text>().color;
        GetComponent<Text>().color = new Color(v4.r, v4.g, v4.b, Mathf.PingPong(Time.time / 2, 1));
	    if (Input.GetKey(KeyCode.Return)) {
	        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	    }
	}
}
