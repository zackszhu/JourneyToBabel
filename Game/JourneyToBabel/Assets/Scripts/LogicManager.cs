using UnityEngine;
using System.Collections;
using System.Linq;
using Assets.Scripts;
using UnityEngine.UI;

public class LogicManager : MonoBehaviour {
    public MapManager Map;
    public CharacterManager Player;
    public float MoveUpSpeed;
    public int AliveLayers;
    public GameObject[] Fading;
    public float UpdateGap = 10;

    private float _currentHeight;
    private float _updategap = 0;

    void Awake() {
        _currentHeight = 3;
        foreach (var o in Fading) {
            o.GetComponent<CanvasRenderer>().SetAlpha(0);
        }
    }

    void FixedUpdate() {
        _currentHeight += MoveUpSpeed * Time.timeScale;
        Map.GenerateMapBase(Mathf.RoundToInt(_currentHeight), AliveLayers);
        if ((Player.Player.Object.transform.position + CharacterManager.CharacterYOffest).y < _currentHeight - AliveLayers) {
            Debug.Log("Die");
        }

        _updategap += Time.deltaTime;  //update difficulty
        if (_updategap > UpdateGap)
        {
            UpdateDifficulty();
            _updategap = 0;
        }

    }

    public void Die() {
        Time.timeScale = 0;
        foreach (var o in Fading) {
            if (o.GetComponent<Image>()) {
                o.GetComponent<Image>().CrossFadeAlpha(1, 1, true);
            }
            else {
                o.GetComponent<Text>().CrossFadeAlpha(1, 1, true);
                o.GetComponent<Text>().text = "Your score: " + Mathf.RoundToInt((Player.Player.Object.transform.position + CharacterManager.CharacterYOffest).y);
            }
        }

    }


    private void UpdateDifficulty()
    {
        Debug.Log("Difficulty Update");
        MoveUpSpeed += 0.001f;

        //友军减少
        var characterConfig = Player.CharacterConifg;
        if (characterConfig.FriendSum > 0)
        {
            characterConfig.FriendSum--;
        }

        //敌军增加
        var CubeTypeConfig = Map.CubeConfig.CubeTypeRatio;
        var sum = CubeTypeConfig.Aggregate(0, (current, s) => (int)(current + s));
        if (sum < 1.0f)
        {
            CubeTypeConfig[0] += 0.01f; //explode 
            CubeTypeConfig[3] += 0.01f; //freeze
        }

    }

}
