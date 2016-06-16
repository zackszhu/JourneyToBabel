using System;
using UnityEngine;
using System.Collections;
using System.Xml.Schema;
using Assets.Scripts;

public class MapManager : MonoBehaviour {
    public GameObject MapCubePrefab;

    public GameObject MapCubes;

    public int MapLength = 11;
    public int MapWidth = 11;
    //public int Layer = 10;
    public int MaxViewLayer = 20; //最大可见层数


    private MapData _mapData;
    // Use this for initialization
    void Awake() {
        //MapCubePrefab.transform.parent = MapCubes.transform;
        _mapData = new MapData(MapLength, MapWidth, MapCubePrefab, MapCubes);
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

    public Vector3 GetStartPosition(int layerNum) {
        return _mapData.GetStartCube(layerNum).OriginPostion;
    }

    public int[] GetIndexByPosition(Vector3 pos) {
        return _mapData.GetIndexByPosition(pos);
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
}
