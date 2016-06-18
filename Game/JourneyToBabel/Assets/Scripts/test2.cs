using UnityEngine;
using System.Collections;

public class test2 : MonoBehaviour {
    public GameObject Particle;
    public float SpeedUp;
    public float SpeedDown;
    public float FreezingDuration;

    void Awake() {
        //        switch (CubeType) {
        //            case EXPLODE:
        //                gameObject.AddComponent<CubeExplode>();
        //                GetComponent<CubeExplode>().Particle = Particle;
        //                GetComponent<CubeExplode>().Work();
        //                break;
        //            case SPEEDUP:
        //                gameObject.AddComponent<SpeedAlter>();
        //                GetComponent<SpeedAlter>().AlteredSpeed = SpeedUp;
        //                GetComponent<SpeedAlter>().Work();
        //                break;
        //            case SPEEDDOWN:
        //                gameObject.AddComponent<SpeedAlter>();
        //                GetComponent<SpeedAlter>().AlteredSpeed = SpeedDown;
        //                GetComponent<SpeedAlter>().Work();
        //                break;
        //            case FROZEN:
        //                gameObject.AddComponent<FreezeTrap>();
        //                GetComponent<FreezeTrap>().FreezeDuration = FreezingDuration;
        //                GetComponent<FreezeTrap>().Work();
        //                break;
        //        }
    }
}
