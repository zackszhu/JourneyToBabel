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
        _mapData.ShowAroundLayer(0, 20);
    }

    public void initMap() {
        
        for (int k = 0; k < MaxViewLayer; k++) {
            _mapData.AppendLayer();
        }
    }

    public Vector3 getStartPosition(int layerNum) {
        return _mapData.GetStandPostion(layerNum);
    }

    public Vector3 GetDirectionSuggestion(Vector3 pos, int characterId) {
        return _mapData.GetDirectionSuggestion(pos, characterId);
    }
}
