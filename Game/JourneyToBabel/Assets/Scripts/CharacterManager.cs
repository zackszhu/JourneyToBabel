using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.StateMachines;
using JetBrains.Annotations;


/// <summary>
/// 管理角色的创建
/// </summary>

public class CharacterManager : MonoBehaviour {


    public GameObject CharacterCubePrefab;
    public GameObject InputManager;
    public GameObject MapManager;
    public int ComputerNumber = 10;
    public float CharacterYOffest = 1.0f - 0.455f;  //人的坐标和地图坐标不一样！
    // Use this for initialization
    private MapManager _mapManager;
    public float AIComputedeltaTime;  //每隔多少时间算一下ai

    private List<Character> _computers;
    public Character Player { get; private set; }


    void Awake () {
        _mapManager = MapManager.GetComponent<MapManager>();
        _computers = new List<Character>();

    }


    void Start() {


        CreatePlayer(1);
        //造了小电脑测试一下
        CreateComputer(2);
    }



    private Character CreateCharacter(int layerNum) {
        var c = _mapManager.getStartCube(layerNum);
        var temp = c.OriginPostion;
        temp.y -= CharacterYOffest;
        //Debug.Log("startPos" + temp);
        var o = Instantiate(CharacterCubePrefab, temp, Quaternion.identity) as GameObject;
        
        return new Character(o,c);

    }

    public void CreatePlayer(int layerNum) {
        var character = CreateCharacter(layerNum);
        character.Type = Character.TYPE.Player;
        InputManager.GetComponent<CharacterInput>().SetCharacterObject(character.Object);
    }
    
    //创建一个电脑
    public void CreateComputer(int layerNum) {
        var character = CreateCharacter(layerNum);
        character.Type = Character.TYPE.Player;
        _computers.Add(character);
    }

    //
    public void RemoveCharacter(int layerNum) {
        
    }

	// Update is called once per frame
    void Update() {
        foreach (var computers in _computers) {
            var currPos = computers.Object.transform.position;
            currPos.y += CharacterYOffest;

            var gap = computers.ExpectedCube.OriginPostion - currPos;
            Debug.Log("checkGap"+ gap);
            if (gap.magnitude < 0.1) {
                //finish move, find a new target cube
               
                computers.Animator.SetBool("isWalking", false);
                computers.FlagMachine.Action(CharacterCommand.MoveEnd);
                var temp = _mapManager.GetTargetSuggestionByCharacterCube(computers);
                

                if (temp == null) {
                    return;
                }
                else {
                    computers.ExpectedCube = temp;
                    computers.CubePath.Add(temp.Id);
                }
            }
            else {
                if (gap.y > 0.2) {
                    computers.Animator.SetTrigger("jump");
                    computers.FlagMachine.Action(CharacterCommand.Jump);
                }
                gap.y = 0;
                computers.Animator.SetBool("isWalking", true);
                computers.FlagMachine.Action(CharacterCommand.MoveBegin, gap);


            }


        }
    }

    /*
	    for (int i = 0; i < _computers.Count; i++) {
	        var temp = _computers[i];


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
    */
    
}

public class Character {

    public enum TYPE {
        Player = 0,
        Friend = 1,
        Enemy = 2,
        UnKnown = 3     //待分配的意思
    }

    private static int _characterCount = 0;
    
    public GameObject Object;
    public TYPE Type;
    public int Id;

    //public float AITime;
    public Vector3 Suggestion;
    public CharacterFlagMachine FlagMachine;
    public Animator Animator;
   
    //public Vector3 Preposition;

    public Cube ExpectedCube;
    //public List<Cube> CubePath;
    public HashSet<int> CubePath;


    public Character(GameObject o, Cube c) {
        Object = o;
        Type = TYPE.UnKnown;
        FlagMachine = o.GetComponent<CharacterFlagMachine>();
        Animator = o.GetComponent<Animator>();
        ExpectedCube = c;
        //Preposition = new Vector3(0,0,0);
        CubePath = new HashSet<int>();
        Id = _characterCount;
        _characterCount ++;
    }

    public void Restart(Cube startCube) {
        CubePath.Clear();
        CubePath.Add(startCube.Id);
    }
}
