using System;
using UnityEngine;
using System.Collections;
using System.Xml.Schema;
using Assets.Scripts;



[Serializable]
public class CubeTypeConfig {
    public float[] CubeTypeRatio = new float[4] {0.1f,0.1f,0.1f,0.1f} ;  //各个特殊种类的比例

    public GameObject Particle;
    public float SpeedUp = 3;
    public float SpeedDown = 1;
    public float FreezingDuration = 1;

}


public class MapManager : MonoBehaviour {


    public GameObject MapCubePrefab;

    public GameObject MapCubes;

    public int MapLength = 11;
    public int MapWidth = 11;


    //public int Layer = 10;
    public int MaxViewLayer = 20; //最大可见层数

    //每种方块出现的比率，四者之和必须小于等于0
    public CubeTypeConfig CubeConfig;


    private MapData _mapData;
    // Use this for initialization
    void Awake() {
        //MapCubePrefab.transform.parent = MapCubes.transform;
        _mapData = new MapData(MapLength, MapWidth, MapCubePrefab, MapCubes, CubeConfig);
        //SetCubeTypeRatio(CubeExplodeRate,CubeSpeedUpRate,CubeSpeedDownRate,CubeFrozenRate);

        
        initMap();
    }


    // 根据玩家位置自动创建地图
    void Update() {
        _mapData.ShowAroundLayer();
    }

    /*public SetValidLayer(int PlayerLayNum) {
         
    }*/

    private void initMap() {
        
        for (int k = 0; k < MaxViewLayer; k++) {
            _mapData.AppendLayer();
        }
    }
    /*
    public void SetCubeTypeRatio(params float[] CubeTypeRatio) {
        _mapData.SetCubeTypeRatio(CubeTypeRatio);
    }
    */

    public void GenerateMapBase(int index, int layers) {
        _mapData.StartLayerNum = Mathf.Max(index - layers,0);
    }

    public void GenerateMapUpper(int index) {
        _mapData.EndLayerNum = Mathf.Max(index + 8,_mapData.EndLayerNum);
    }

    public Vector3 GetStartPosition(int layerNum) {
        return _mapData.GetStartCube(layerNum).OriginPostion;
    }

    public int[] GetIndexByPosition(Vector3 pos) {
        return _mapData.GetIndexByPosition(pos);
    }

    public Cube GetCubeByPosition(Vector3 pos) {
        return _mapData.GetCubeByPosition(pos);
    }

    public Cube GetStartCube(int layerNum) {
        return _mapData.GetStartCube(layerNum);
    }

    public Vector3 GetDirectionSuggestion(Vector3 pos, int characterId) {
        return _mapData.GetDirectionSuggestionByRealTimePos(pos, characterId);
    }

    public Cube GetTargetSuggestionByCharacter(Character character) {
        return _mapData.GetTargetSuggestionByCharacterCube(character);
    }

    public int GetEndLayerNum() {
        return _mapData.EndLayerNum;
    }

    public int GetStartLayerNum() {
        return _mapData.StartLayerNum;
    }
}
