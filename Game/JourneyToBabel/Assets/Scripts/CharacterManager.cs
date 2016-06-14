using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.StateMachines;

public class CharacterManager : MonoBehaviour {


    public GameObject CharacterCubePrefab;
    public GameObject InputManager;
    public GameObject MapManager;
    // Use this for initialization
    private MapManager _mapManagerScript;
    public float AIComputedeltaTime;  //每隔多少时间算一下ai

    private List<Character> _characters;


    void Awake () {
        _mapManagerScript = MapManager.GetComponent<MapManager>();
        _characters = new List<Character>();

    }


    void Start() {
        var charactGameObject = Instantiate(CharacterCubePrefab, _mapManagerScript.getStartPosition(1), Quaternion.identity) as GameObject;
        InputManager.GetComponent<CharacterInput>().SetCharacterObject( charactGameObject);

        //造了小电脑测试一下
        CreateCharacter(2);
    }

    
    //创建一个电脑
    public void CreateCharacter(int layerNum) {
        var charactGameObject = Instantiate(CharacterCubePrefab, _mapManagerScript.getStartPosition(layerNum), Quaternion.identity) as GameObject;
        _characters.Add(new Character(charactGameObject,0));
    }

    //
    public void RemoveCharacter(int layerNum) {
        
    }

	// Update is called once per frame
	void Update () {
	    for (int i = 0; i < _characters.Count; i++) {
	        var temp = _characters[i];
	        temp.AITime -= Time.deltaTime;
	        if (temp.Animator.GetBool("isWalking")) {
	            var gap = temp.Object.transform.position - temp.Preposition;

                if (gap.magnitude < Time.deltaTime*0.5) {
                    temp.Animator.SetTrigger("jump");
                    temp.FlagMachine.Action(CharacterCommand.Jump);
                    
                }

	        }
	        temp.Preposition = temp.Object.transform.position;


	        if (temp.AITime <= 0) {




	            temp.Suggestion = _mapManagerScript.GetDirectionSuggestion(temp.Object.transform.position, i);
                //Debug.Log(temp.Suggestion);
	            if (temp.Suggestion.y > 0) {
                    //JUMP
	                //Debug.Log("Jump");
                    temp.Animator.SetTrigger("jump");
                    temp.FlagMachine.Action(CharacterCommand.Jump);
	                temp.Suggestion.y = 0;
	            }
	            temp.AITime = AIComputedeltaTime;
	        }

	        if (temp.Suggestion.magnitude > 0.1) {
                temp.Animator.SetBool("isWalking", true);
                temp.FlagMachine.Action(CharacterCommand.MoveBegin, temp.Suggestion);
	        }
	        else {
                temp.Animator.SetBool("isWalking", false);
                temp.FlagMachine.Action(CharacterCommand.MoveEnd);
            }
	    }


	}
}

internal class Character {
    public GameObject Object;
    public int Type;
    public float AITime;
    public Vector3 Suggestion;
    public CharacterFlagMachine FlagMachine;
    public Animator Animator;
    public Vector3 Preposition;


    public Character(GameObject o, int type) {
        Object = o;
        this.Type = type;
        AITime = 0;
        FlagMachine = o.GetComponent<CharacterFlagMachine>();
        Animator = o.GetComponent<Animator>();
        Preposition = new Vector3(0,0,0);
    }
}
