using UnityEngine;
using System.Collections;

public class EnemyHurt : MonoBehaviour {
    public float AlteredSpeed;

    void OnTriggerEnter(Collider collision) {
//        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.name.Equals("human(Clone)")) {
            Debug.Log("Enemy");
            collision.gameObject.GetComponent<CharacterMove>().Speed = AlteredSpeed;
        }
    }

    void OnTriggerExit(Collider collision) {
        if (collision.gameObject.name.Equals("human(Clone)")) {
            collision.gameObject.GetComponent<CharacterMove>().Speed = 2;
        }
    }


}
