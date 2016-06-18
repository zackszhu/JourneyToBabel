using UnityEngine;
using System.Collections;

public class FriendTrace : MonoBehaviour {
    public float Duration;
    public float HelpDegree;




    void OnControllerColliderHit(ControllerColliderHit hit) {
        if (hit.gameObject.tag == "GameMap") {
            StartCoroutine(Trace(hit.collider.gameObject));
        }
    }

    IEnumerator Trace(GameObject cube) {
        cube.AddComponent<SpeedAlter>();
        cube.GetComponent<SpeedAlter>().AlteredSpeed = HelpDegree;
        cube.GetComponent<SpeedAlter>().Work();

        yield return new WaitForSeconds(Duration);


        cube.GetComponent<SpeedAlter>().AlteredSpeed = 2;
        cube.GetComponent<SpeedAlter>().Work();
    }
}
