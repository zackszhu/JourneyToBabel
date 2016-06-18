using UnityEngine;
using System.Collections;

public class LogicManager : MonoBehaviour {
    public MapManager Map;
    public CharacterManager Player;
    public float MoveUpSpeed;
    public int AliveLayers;

    private float _currentHeight;

    void Awake() {
        _currentHeight = 3;
    }

    void FixedUpdate() {
        _currentHeight += MoveUpSpeed * Time.timeScale;
        Map.GenerateMapBase(Mathf.RoundToInt(_currentHeight), AliveLayers);
        if ((Player.Player.Object.transform.position + CharacterManager.CharacterYOffest).y < _currentHeight - AliveLayers) {
            Debug.Log("Die");
        }
    }
}
