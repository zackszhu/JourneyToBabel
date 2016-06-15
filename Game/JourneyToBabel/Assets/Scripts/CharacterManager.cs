using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.StateMachines;


/// <summary>
/// 管理角色的创建
/// </summary>

public class CharacterManager : MonoBehaviour {


    public GameObject CharacterCubePrefab;
    public GameObject InputManager;
    public GameObject MapManager;
    public GameObject AINumber;
    public float CharacterYOffest = 1.0f - 0.455f;  //人的坐标和地图坐标不一样！
    // Use this for initialization
    private MapManager _mapManager;
    public float AIComputedeltaTime;  //每隔多少时间算一下ai

    private List<Character> _computers;


    void Awake () {
        _mapManager = MapManager.GetComponent<MapManager>();
        _computers = new List<Character>();

    }


    void Start() {

        var temp = _mapManager.getStartPosition(1);
        temp.y -= CharacterYOffest;
        //Debug.Log(temp);
        var charactGameObject = Instantiate(CharacterCubePrefab, temp, Quaternion.identity) as GameObject;
        //Debug.Log(charactGameObject.transform.position);
        InputManager.GetComponent<CharacterInput>().SetCharacterObject( charactGameObject);

        //造了小电脑测试一下
        CreateComputer(2);
    }
    


    
    //创建一个电脑
    public void CreateComputer(int layerNum) {

        Cube cube = _mapManager.getStartCube(layerNum);

        var charactGameObject = Instantiate(CharacterCubePrefab, _mapManager.getStartPosition(layerNum) + new Vector3(0, 0.5f, 0), Quaternion.identity) as GameObject;
        _computers.Add(new Character(charactGameObject,0));
    }

    //
    public void RemoveCharacter(int layerNum) {
        
    }

	// Update is called once per frame
	void Update () {
	    for (int i = 0; i < _computers.Count; i++) {
	        var temp = _computers[i];
	        temp.AITime -= Time.deltaTime;  //TODO 通过是否到达来判断，而不是定时


	        if (temp.AITime <= 0) {




	            temp.Suggestion = _mapManager.GetDirectionSuggestion(temp.Object.transform.position, i);
                //Debug.Log("sugg:"+temp.Suggestion);
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
	            if (temp.Suggestion.Equals(Vector3.zero)) {
	                // 使用闪烁    
	            }


                //保证一次探索不走重复的路，每次探索给地图添加信息素

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

    public Cube CurrentTarget;
    //public List<Cube> CubePath;
    public HashSet<Cube> CubePath;


    public Character(GameObject o, int type) {
        Object = o;
        this.Type = type;
        FlagMachine = o.GetComponent<CharacterFlagMachine>();
        Animator = o.GetComponent<Animator>();
        Preposition = new Vector3(0,0,0);
        CubePath = new HashSet<Cube>();
    }

    public void Restart(Cube startCube) {
        CubePath.Clear();
        CubePath.Add(startCube);
    }
}
