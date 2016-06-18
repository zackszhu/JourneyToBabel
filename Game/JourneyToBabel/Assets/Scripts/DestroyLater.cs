using UnityEngine;
using System.Collections;

public class DestroyLater : MonoBehaviour {

    void Awake() {
        StartCoroutine(WaitAndDie());
    }

    IEnumerator WaitAndDie() {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }
}
