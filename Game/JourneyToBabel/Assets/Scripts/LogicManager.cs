using UnityEngine;
using System.Collections;
using System.Linq;
using Assets.Scripts;
using UnityEngine.UI;

public class LogicManager : MonoBehaviour {
    public AudioClip DieAudioClip;
    public MapManager Map;
    public CharacterManager Player;
    public float MoveUpSpeed;
    public int AliveLayers;
    public GameObject[] Fading;
    public float UpdateGap = 10;
    public Text Score;

    private float _currentHeight;
    private float _updategap = 0;
    private bool _die;
    private int _score;
    private int _maxScore;

    void Awake() {
        _die = false;
        _currentHeight = 3;
        foreach (var o in Fading) {
            o.GetComponent<CanvasRenderer>().SetAlpha(0);
        }
    }

    void FixedUpdate() {
        _currentHeight += MoveUpSpeed * Time.timeScale;
        Map.GenerateMapBase(Mathf.RoundToInt(_currentHeight), AliveLayers);
        _updategap += Time.deltaTime;  //update difficulty
        if (_updategap > UpdateGap)
        {
            UpdateDifficulty();
            _updategap = 0;
        }

    }

    void Update() {
        _score = Mathf.Max(Mathf.RoundToInt((Player.Player.Object.transform.position + CharacterManager.CharacterYOffest).y), 0);
        if (_score > _maxScore) {
            _maxScore = _score;
        }
        if (!_die) {
            Score.text = _score.ToString();
        }
    }

    public void Die() {
        GetComponent<AudioSource>().PlayOneShot(DieAudioClip);
        int historyScore = 0;
        if (PlayerPrefs.HasKey("HighScore")) {
            historyScore = PlayerPrefs.GetInt("HighScore");
            if (_maxScore > historyScore) {
                historyScore = _maxScore;
            }
        }
        else {
            historyScore = _maxScore;
        }
        Time.timeScale = 0;
        foreach (var o in Fading) {
            if (o.GetComponent<Image>()) {
                o.GetComponent<Image>().CrossFadeAlpha(1, 3, true);
            }
            else {
                o.GetComponent<Text>().CrossFadeAlpha(1, 1, true);
                o.GetComponent<Text>().text = "Your score:      " + _maxScore + "\nHighest score: " + historyScore;
            }
        }
        _die = true;
        PlayerPrefs.SetInt("HighScore", historyScore);
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
