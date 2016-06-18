using System;
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
    public GameObject EnemyCubePrefab;
    public GameObject FriendCubePerfab;
    public GameObject InputManager;
    public GameObject MapManager;
    public CharacterTypeConifg CharacterConifg;
    public static Vector3 CharacterYOffest = new Vector3(0,1.0f - 0.455f,0);  //人的坐标和地图坐标不一样！
    // Use this for initialization
    private MapManager _mapManager;
    private float _playerOriginalSpeed;

    private List<Character> _computers;
    public Character Player { get; private set; }

    private int _friendCount = 0; //目前地图上的友军个数
    private int _enemyCount = 0; //目前地图上的敌军个数

    private float _timeGap = 0; 

    void Awake () {
        _mapManager = MapManager.GetComponent<MapManager>();
        _computers = new List<Character>();

    }


    void Start() {


        CreatePlayer(5);
        //造了小电脑测试一下
    }

    //随机分配一个角色(友军或者敌人)，但是会根据人数比例调整概率
    private Character.TYPE GetTypeRandomly() {
        int friendSurplus = CharacterConifg.FriendSum - _friendCount;
        int enemySurplus = CharacterConifg.Sum - CharacterConifg.FriendSum - _enemyCount;
       
        int cursor = UnityEngine.Random.Range(0,friendSurplus+enemySurplus);
        return cursor < friendSurplus ? Character.TYPE.Friend : Character.TYPE.Enemy;

    }


    private Character CreateCharacter(int layerNum, Character.TYPE characterType) {
        var c = _mapManager.GetStartCube(layerNum);
        var temp = c.OriginPostion - CharacterYOffest;
        //Debug.Log("startPos" + temp);


        GameObject o = null;
        switch (characterType) {
            case Character.TYPE.Player:
                o = Instantiate(CharacterCubePrefab, temp, Quaternion.identity) as GameObject;
                break;
            case Character.TYPE.Enemy:
                _enemyCount++;
                o = Instantiate(EnemyCubePrefab, temp, Quaternion.identity) as GameObject;
                break;
            case Character.TYPE.Friend:
                _friendCount++;
                o = Instantiate(FriendCubePerfab, temp, Quaternion.identity) as GameObject;
                break;
            default:
                throw new ArgumentOutOfRangeException("characterType", characterType, null);
        }
        return new Character(o, c, characterType);
    }


    public void CreatePlayer(int layerNum) {
        var character = CreateCharacter(layerNum, Character.TYPE.Player);
        Player = character;
        _playerOriginalSpeed = Player.Object.GetComponent<CharacterMove>().Speed;

        //InputManager.GetComponent<CharacterInput>().Player = character;
    }

    public void CreateComputer(int layerNum) {
        var character = CreateCharacter(layerNum, GetTypeRandomly());
        _computers.Add(character);
    }


    //创建一个电脑
    public void SupplyComputer() {
        if (_computers.Count >= CharacterConifg.Sum) return;
        _timeGap += Time.deltaTime;
        if (_timeGap < CharacterConifg.AppendTimeGap) return;


        CreateComputer(_mapManager.GetEndLayerNum() - UnityEngine.Random.Range(1,4));
        _timeGap = 0;
    }

    //

    // Update is called once per frame
    void Update() {
        HandlePlayer();
        HandleAI();
        SupplyComputer();
    }

    private void CreateAI() {
    }

    void HandlePlayer() {
        SetCurrentCube(Player);
        if (Player.IsDead) {
            Debug.Log("Player is Dead");
            Time.timeScale = 0; //死亡时候游戏暂停

            return;
        }
        try {
            _mapManager.GenerateMapUpper(Mathf.RoundToInt(Player.CurrentCube.OriginPostion.y));
        }
        catch (Exception) {
            Player.IsDead = true;
            return;
        }
        
        
    
        //Player.ExpectedCube = _mapManager.GetCubeByPosition(Player.Object.transform.position + CharacterYOffest);
    }


    bool SetCurrentCube(Character character) {
        //设置失败一般就是死了
        try {
            character.CurrentCube = _mapManager.GetCubeByPosition(character.Object.transform.position + CharacterYOffest);
            if (character.CurrentCube.Index[1] <= _mapManager.GetStartLayerNum()) {
                character.IsDead = true;
                character.CurrentCube = null;
            }
        }
        catch (Exception) {
            //死了
            character.IsDead = true;
            character.CurrentCube = null;
            return false;
        }
        return true;
    }

    void DestoryCharacter(Character character) {
        if (character.Object == null) return;
        //character.Object.SetActive(false);

        Destroy(character.Object);
        character.Object = null;
        switch (character.Type) {
            case Character.TYPE.Player:
                break;
            case Character.TYPE.Friend:
                _friendCount--;
                break;
            case Character.TYPE.Enemy:
                _enemyCount--;
                break;
            case Character.TYPE.UnKnown:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    void HandleAI() {
        var playerCollided = false;
        foreach (var computer in _computers) {
            if (computer.FlagMachine.Flags[(int) CharacterProcessState.Transfer]) continue;
            SetCurrentCube(computer);

            if (computer.IsDead) {
                DestoryCharacter(computer);
                continue;
            }
            
            if (Player.CurrentCube!=null && computer.CurrentCube.Id == Player.CurrentCube.Id) {
                //Debug.Log("In");
                playerCollided = true;
            }



            var currPos = computer.Object.transform.position + CharacterYOffest;
            var gap = computer.ExpectedCube.OriginPostion - currPos;

            
           // Debug.Log("gap" + gap + "tempgap" + (computer.CurrentCube.OriginPostion- currPos) + "index:" + (computer.CurrentCube.Index[1]));
            //Debug.Log("Now" + currPos + "Expected" + computer.ExpectedCube.OriginPostion);
            if (gap.magnitude < 0.1) //到达了预期方格
            {
                
                var temp = _mapManager.GetTargetSuggestionByCharacter(computer);
                if (temp == null) //没有好的路可走
                {
                    if (UnityEngine.Random.value < CharacterConifg.TransferRate) {
                        //回头走
                        Cube targetCube = SearchRestartCube(computer); //寻找下一个出生点                  
                        computer.Transfer(targetCube);
                    }
                    else {  //原地直接从头开始，不传送
                        computer.Restart(computer.CurrentCube);
                    }
                    computer.StopWalk();
                    
                    return;
                }
                else {
                    computer.ExpectedCube = temp;
                    computer.CubePath.Add(temp.Id);
                }
            }
            else {
                //Debug.Log("gap" + gap + "tempgap" + (computer.CurrentCube.OriginPostion - currPos) + "index:" + (computer.CurrentCube.Index[1]));
                if (gap.y > 0.2) {
                    computer.Jump();
                }
                gap.y = 0;
                computer.Walk(gap);
            }
        }
        if (playerCollided) {
            //Debug.Log("Enemy");
            Player.Object.GetComponent<CharacterMove>().Speed = 1;
        }
        else {
            //Debug.Log("Out");
            Player.Object.GetComponent<CharacterMove>().Speed = _playerOriginalSpeed;
        }

        _computers.RemoveAll(character => character.IsDead); //真正删掉所有死的
    }


    public Cube SearchRestartCube(Character character) {
        var LayerNum = character.CurrentCube.Index[1];
        return _mapManager.GetStartCube(LayerNum);
    }


    public void PlayerTransfer() {
        var targetCube = SearchRestartCube(Player);
        Player.Transfer(targetCube);
    }
}

