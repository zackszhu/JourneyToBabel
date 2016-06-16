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
    public static Vector3 CharacterYOffest = new Vector3(0,1.0f - 0.455f,0);  //人的坐标和地图坐标不一样！
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
        CreateComputer(3);
        CreateComputer(4);
        CreateComputer(3);
    }



    private Character CreateCharacter(int layerNum) {
        var c = _mapManager.GetStartCube(layerNum);
        var temp = c.OriginPostion - CharacterYOffest;
        //Debug.Log("startPos" + temp);
        var o = Instantiate(CharacterCubePrefab, temp, Quaternion.identity) as GameObject;
        
        return new Character(o,c);

    }

    public void CreatePlayer(int layerNum) {
        var character = CreateCharacter(layerNum);     
        character.Type = Character.TYPE.Player;
        Player = character;

        //InputManager.GetComponent<CharacterInput>().Player = character;
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
        HandlePlayer();
        HandleAI();
    }

    void HandlePlayer() {
        
        //Player.ExpectedCube = _mapManager.GetCubeByPosition(Player.Object.transform + CharacterYOffest);
    }


    void HandleAI() {
        foreach (var computer in _computers) {
            if (computer.FlagMachine.Flags[(int) CharacterProcessState.Transfer]) continue ; 

            var currPos = computer.Object.transform.position + CharacterYOffest;

            var gap = computer.ExpectedCube.OriginPostion - currPos;
            //Debug.Log("Now" + currPos + "Expected" + computer.ExpectedCube.OriginPostion);
            if (gap.magnitude < 0.1)  //到达了预期方格
            {
                var temp = _mapManager.GetTargetSuggestionByCharacter(computer);
                if (temp == null) //没有好的路可走
                {
                    Cube targetCube = SearchRestartCube(computer);  //寻找下一个出生点
                    computer.Transfer(targetCube);
                    computer.StopWalk();
                    return;
                }
                else {
                    computer.ExpectedCube = temp;
                    computer.CubePath.Add(temp.Id);
                }
            }
            else {
                if (gap.y > 0.2)
                {
                    /*
                    computer.Animator.SetTrigger("jump");
                    computer.FlagMachine.Action(CharacterCommand.Jump);*/
                    computer.Jump();
                }
                gap.y = 0;
                computer.Walk(gap);

//              computers.Animator.SetBool("isWalking", true);
//              computers.FlagMachine.Action(CharacterCommand.MoveBegin, gap);
            }
        }
    }






    public Cube SearchRestartCube(Character character) {
        var LayerNum = character.ExpectedCube.Index[1];
        //TODO 动态区域判断

        return _mapManager.GetStartCube(LayerNum);
    }


    public void PlayerTransfer() {
        var targetCube = SearchRestartCube(Player);
        Player.Transfer(targetCube);
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

