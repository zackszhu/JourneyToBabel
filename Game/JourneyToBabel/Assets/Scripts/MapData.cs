using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SocialPlatforms.Impl;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Assets.Scripts {
    class Cube {
        public bool IsValid;
        public bool IsEdge; //optimize performance
//        public HashSet<byte> FromPath; //以该方块为目标的路径
//        public HashSet<byte> ToPath; //以该方块为起点的路径 ， 路径指的是从下方到上方运动的方向,就是0-7
        public GameObject Object; //对应的游戏方块对象。
        public int Score; //某个点能往上走就能加分，往旁边和往上都不行了，就负分表示此路不通，0表示未探索过的。
        public bool Pattern;
        public bool GoodHole;
        public HashSet<int> PasserBy;

        public Cube() {
            IsValid = true;
            IsEdge = false;
            PasserBy = new HashSet<int>();
            // FromPath = new HashSet<byte>();
            Object = null;
            //ToPath = new HashSet<byte>();
            Score = 0;
            GoodHole = false;
        }

        public void InValid(bool isEdge = true) {
            IsValid = false;
            //FromPath.Clear();
            //ToPath.Clear();
            this.IsEdge = isEdge;
        }

        /*       public bool IsAccessiable() {
            return FromPath.Count > 0;
        }*/

        public bool IsHole() {
            return !IsValid && IsEdge;
        }
    }

    class Layer {
        public GameObject LayerObj;
        public Cube[,] LayerData;
        public List<int[]> Hole;
        public int LayNum;


        public Layer(int layNum, Cube[,] layerData, GameObject parent, List<int[]> hole) {
            LayerObj = new GameObject(layNum.ToString());
            LayerObj.transform.parent = parent.transform;
            foreach (var cube in layerData) {
                if (cube.Object != null)
                    cube.Object.transform.parent = LayerObj.transform;
            }
            LayerData = layerData;
            LayNum = layNum;
            Hole = hole;
        }


        public Layer(GameObject layerObj, Cube[,] layerData, List<int[]> hole, int layNum) {
            LayerObj = layerObj;
            LayerData = layerData;
            Hole = hole;
            LayNum = layNum;
        }

        public Cube this[int index1, int index2] {
            get { return LayerData[index1, index2]; }
        }
    }

    class MapData : ScriptableObject {
        public int MapLength;
        public int MapWidth;
        public GameObject MapCubePrefab;
        public GameObject MapCubeParent;
        //public GameObject
        //private GameObject _mapCubePrefabs;


        private int _layerCount = 1;
        private List<Layer> _mapData;


        public int LayerCount {
            get { return _layerCount; }
            set { _layerCount = value; }
        }

        private readonly int _halfLength;
        private readonly int _halfWidth;
        private readonly int[,] _round4 = {{0, 1, 10}, {1, 0, 10}, {0, -1, -1}, {-1, 0, -1}};
        private readonly int[,] _round8 = {{0, 1}, {1, 1}, {1, 0}, {1, -1}, {0, -1}, {-1, -1}, {-1, 0}, {-1, 1}};

        public MapData(int length, int width, GameObject mapCubePrefab, GameObject mapCubeParent) {
            this.MapLength = length;
            this.MapWidth = width;
            this.LayerCount = 1;
            this.MapCubePrefab = mapCubePrefab;
            this.MapCubeParent = mapCubeParent;
            this._halfLength = MapLength/2;
            this._halfWidth = MapWidth/2;

            InitBottomLayer();
        }

        private void InitBottomLayer() {
            var bottom = GetDefaultLayer();
            CreateLayerObject(bottom, 0);
            _mapData = new List<Layer> {new Layer(0, bottom, MapCubeParent, new List<int[]>())};
        }


        private Cube[,] GetDefaultLayer() {
            var layer = new Cube[MapLength, MapWidth];
            for (int i = 0; i < MapLength; i++) {
                for (int j = 0; j < MapWidth; j++) {
                    layer[i, j] = new Cube();
                    if (i == 0 || j == 0 || i == MapLength - 1 || j == MapWidth - 1) {
                        layer[i, j].InValid(false);
                    }
                }
            }
            return layer;
        }


        public void AppendLayer() {
            var top = GetDefaultLayer();

            var edge = new List<int[]>();

            /*
            var InQueue = new bool[MapLength+2,MapWidth+2];
            InQueue.Initialize();*/

            for (var x = 1; x < MapLength - 1; x++) {
                for (var y = 1; y < MapWidth - 1; y++) {
                    if (IsEdge(x, y, top)) {
                        edge.Add(new int[] {x, y});
                        //InQueue[x, y] = true;
                        top[x, y].IsEdge = true;
                    }
                }
            }

            //GenerateByPath(top,edge);
            List<int[]> hole = GenerateByHole(top, edge, MapLength + MapWidth);

            //GenerateObjects();


            CreateLayerObject(top, LayerCount);
            _mapData.Add(new Layer(LayerCount, top, MapCubeParent, hole));


            LayerCount++;

            //ShowAroundLayer(20);
        }

        //动态创建最上面一层的方块对象
        private void CreateLayerObject(Cube[,] top, int layerNum) {
            var scale = MapCubePrefab.transform.localScale;

            for (int i = 1; i < MapLength - 1; i++)
                for (int j = 1; j < MapWidth - 1; j++) {
                    if (top[i, j].IsValid) {
                        top[i, j].Object =
                            (GameObject) Instantiate(
                                MapCubePrefab,
                                //new Vector3((i - Lmid) , layerNum , (j - Wmid)),
                                new Vector3((i - _halfLength)*scale.x, layerNum*scale.y, (j - _halfLength)*scale.z),
                                Quaternion.identity);
                    }
                }
        }


        // 动态显示某些连续的层
        public void ShowAroundLayer(int startLayerNum, int endLayerNum) {
            for (int l = startLayerNum; l < endLayerNum && l < LayerCount; l++) {
                _mapData[l].LayerObj.SetActive(true);
                //ShowLayer(t, Mathf.Abs(l - centerLayer) <= areaSize);
            }
            int i = startLayerNum - 1;
            while (i >= 0 && _mapData[i].LayerObj.activeSelf) {
                _mapData[i].LayerObj.SetActive(false);
                i--;
            }

            i = endLayerNum;
            while (i < LayerCount && _mapData[i].LayerObj.activeSelf) {
                _mapData[i].LayerObj.SetActive(false);
                i++;
            }
        }


        public Layer GetLayer(int layerNum) {
            return _mapData[layerNum];
        }

        public Layer GetTopLayer() {
            return _mapData.Last();
        }


        //---------------help functions -----------------
        //可以想办法给无路可走的地方设置机遇门

        private bool IsEdge(int x, int y, Cube[,] sample) {
            if (OutOfRange(x, y)) return true;
            //Cube[,] temp = _mapData.Last();

            for (int i = 0; i < 8; i++) {
                if (sample[x + _round8[i, 0], y + _round8[i, 1]].IsValid == false) {
                    return true;
                }
            }
            return false;
        }


        //通过空洞法生成新的一层，返回空洞集合
        public List<int[]> GenerateByHole(Cube[,] top, List<int[]> edge, int HoleCount = 20, int midRate = 70) {
            List<int[]> holes = new List<int[]>();

            var lastLayer = _mapData.Last().LayerData;


            var lowPriority = new List<int[]>();


            while (HoleCount > 0 && edge.Count > 0) {
                int index = Random.Range(0, edge.Count);

                var x = edge[index][0];
                var y = edge[index][1];

                if (IsHoleAround(x, y, lastLayer)) {
                    if (!lastLayer[x, y].IsHole() || Random.Range(0, 100) <= midRate) {
                        holes.Add(edge[index]);

                        top[x, y].GoodHole = lastLayer[x, y].IsValid; //这个空的下方他不是空
                        top[x, y].InValid();
                        HoleCount--;
                    }
                    else {
                        lowPriority.Add(edge[index]);
                    }
                }
                else {
                    lowPriority.Add(edge[index]);
                }

                edge.RemoveAt(index);
            }


            while (HoleCount > 0 && lowPriority.Count > 0) {
                int index = Random.Range(0, lowPriority.Count);
                var x = lowPriority[index][0];
                var y = lowPriority[index][1];
                holes.Add(lowPriority[index]);
                top[x, y].GoodHole = lastLayer[x, y].IsValid; //这个空的下方他不是空
                top[x, y].InValid();
                HoleCount --;

                lowPriority.RemoveAt(index);
            }

            return holes;
        }


        public bool IsHoleAround(int x, int y, Cube[,] layer) {
            for (int i = x - 1; i <= x + 1; i++)
                for (int j = y - 1; j <= y + 1; j++)
                    if (layer[i, j].IsHole()) {
                        return true;
                    }
            return false;
        }

        // 一个测试用的根据路劲删除方块的算法,但是效果不好，现在用上方那个空洞衍生法。
        /*
        public void GenerateByPath(Cube[,] top, List<int[]> edge, int minPath = 5) {
            int pathCount = 0;
            var lastLayer = _mapData.Last().LayerData;
            while (pathCount < minPath && edge.Count > 0) {
                //push queue head
                int index = Random.Range(0, edge.Count);

                var x = edge[index][0];
                var y = edge[index][1];

                edge.RemoveAt(index);

                // compute path addtion
                var paths = GetPath(x, y, lastLayer, top);
                int pathCountAddition = paths.Count - top[x, y].FromPath.Count;
                //Debug.Log("remove " +" "+ LayerCount + " " +  x + " " + y + " " + pathCountAddition);

                // update path
                if (pathCountAddition >= 0) {
                    //bfs  could remove
                    pathCount += pathCountAddition;

                    // remove a cube on top layer
                    var target = top[x, y];
                    foreach (byte dir in top[x, y].FromPath) {
                        var tx = x - _round8[dir, 0];
                        var ty = y - _round8[dir, 1];
                        lastLayer[tx, ty].ToPath.Remove(dir);
                    }
                    target.InValid();

                    // update paths from last layer to top layer
                    lastLayer[x, y].ToPath = paths;

                    foreach (byte dir in paths) {
                        var tx = x + _round8[dir, 0];
                        var ty = y + _round8[dir, 1];
                        top[tx, ty].FromPath.Add(dir);
                    }

                    //insert new edge cube into edge-queue
                    for (byte dir = 0; dir < 4; dir++)
                    {
                        var tx = x + _round4[dir, 0];
                        var ty = y + _round4[dir, 1];
                        var roundCube = lastLayer[tx, ty];
                        
                        if (roundCube.IsValid && InQueue[tx, ty] == false) {
                            edge.Add(new int[] {tx,ty});
                            InQueue[tx, ty] = true;
                        }
                    }
                }
            }
        }*/


        public bool OutOfRange(int x, int y) {
            return (x > MapLength || x < 1 || y > MapWidth || y < 1);
        }


        //从下层方块x,y走到上层的路径数量
        /*
        private HashSet<byte> GetPath(int x, int y, Cube[,] from, Cube[,] to) {
            var result = new HashSet<byte>();
            var origin = from[x, y];
            if (!origin.IsValid || !origin.IsAccessiable())
                return result;

            for (byte dir = 0; dir < 8; dir++) {
                var tx = x + _round8[dir, 0];
                var ty = y + _round8[dir, 1];
                //Debug.Log("coord" + x + " " + y);
                var roundCube = to[tx, ty];
                if (roundCube.IsValid && IsEdge(tx, ty, to)) {
                    result.Add(dir);
                }
            }

            return result;
        }*/

        // 从底层开始删除
        private void DeleteBottomLayer() {
            if (_mapData.Count > 0)
                _mapData.RemoveAt(0);
        }

        //在某一层得到一个空着的能站的位置
        public Vector3 GetStandPostion(int layerNum) {
            var currentLayer = _mapData[layerNum];
            var lastLayer = _mapData[layerNum - 1];

            var hole = currentLayer.Hole;

            //TODO 从某一个随机点开始

            int start = Random.Range(0, hole.Count);

            for (int i = 0; i < hole.Count; i++) {
                var t = (i + start)%hole.Count;
                var cube = lastLayer[hole[t][0], hole[t][1]];
                if (!cube.IsHole()) {
                    return cube.Object.transform.position;
                }
            }
            throw new Exception("no holes");
        }


        private Vector3 GetPostionByIndex(int x, int y, int z) {
            var scale = MapCubePrefab.transform.localScale;
            return new Vector3((x - _halfLength)*scale.x, y*scale.y, (z - _halfLength)*scale.z);
            //return new Vector3(x*scale.x, y* scale.y, z*scale.z);
        }

        public int[] GetIndexByPosition(Vector3 pos) {
            var index = new int[3];
            var scale = MapCubePrefab.transform.localScale;
            //Debug.Log(pos);
            index[0] = (Mathf.RoundToInt(pos.x/scale.x)) + _halfLength;
            index[1] = Mathf.RoundToInt(pos.y/scale.y + 0.54f);
            index[2] = Mathf.RoundToInt(pos.z/scale.z) + _halfWidth;
            return index;
        }

        //根据玩家的所处的位置，给他推荐行走方向。
        public Vector3 GetDirectionSuggestion(Vector3 pos, int characterId) {
            int[] index = GetIndexByPosition(pos);
            var LayerNum = index[1];
            if (LayerNum >= LayerCount - 1) return new Vector3(0, 0, 0);

            int x = index[0];
            int y = index[2];
            //Debug.Log("index:" + x + ":" + y + ":" +"layer:"+ LayerNum);

            var currLayer = _mapData[LayerNum];
            currLayer[x, y].PasserBy.Add(characterId);


            // jump and move
            var upLayer = _mapData[LayerNum + 1];
            int dir = 0;
            if (!upLayer[x, y].IsValid) {
                //上方是空 可以跳
                dir = FindJumpWay(x, y, upLayer.LayerData, characterId);
                if (dir != -1) {
                    Debug.Log("JUMP");
                    currLayer[x, y].Score++;
                    return new Vector3(_round8[dir, 0], 1, _round8[dir, 1]);
                }
            }


            // move 同一层移动
            dir = FindMoveWay(x, y, currLayer.LayerData, characterId);
            if (dir != -1) {
                Debug.Log("MOVE");
                return new Vector3(_round4[dir, 0], 0, _round4[dir, 1]);
            }

            currLayer[x, y].Score -= 10;

            // slide down 走投无路往下走
            dir = FindDownWay(x, y, currLayer.LayerData, characterId);
            if (dir != -1) {
                Debug.Log("DOWN");
                return new Vector3(_round4[dir, 0], 0, _round4[dir, 1]);
            }

            //stand 站着不动
            return new Vector3(0, 0, 0);
        }

        public int FindJumpWay(int x, int y, Cube[,] layerData, int characterId) {
            int result = -1;
            int maxScore = -1;
            for (byte dir = 0; dir < 8; dir++) {
                var temp = (characterId + dir)%8;

                var tx = x + _round8[temp, 0];
                var ty = y + _round8[temp, 1];

                //Debug.Log("coord" + x + " " + y);
                var cube = layerData[tx, ty];
                if (cube.GoodHole && cube.Score > maxScore && !cube.PasserBy.Contains(characterId)) {
                    maxScore = cube.Score;
                    result = temp;
                }
            }
            return result;
        }


        public int FindMoveWay(int x, int y, Cube[,] layerData, int characterId)
        {
            int result = -1;
            int maxScore = -1;
            for (byte dir = 0; dir < 4; dir++)
            {
                var temp = (characterId + dir) % 4;

                var tx = x + _round4[temp, 0];
                var ty = y + _round4[temp, 1];

                //Debug.Log("coord" + x + " " + y);
                var cube = layerData[tx, ty];
                if (cube.GoodHole && cube.Score > maxScore && !cube.PasserBy.Contains(characterId))
                {
                    maxScore = cube.Score;
                    result = temp;
                }
            }
            return result;
        }



        public int FindDownWay(int x, int y, Cube[,] layerData, int characterId) {
            int result = -1;
            int maxScore = -100;
            for (byte dir = 0; dir < 4; dir++) {
                var temp = (characterId + dir)%4;

                var tx = x + _round4[temp, 0];
                var ty = y + _round4[temp, 1];

                //Debug.Log("coord" + x + " " + y);
                var cube = layerData[tx, ty];
                if (!cube.GoodHole && cube.IsHole() && cube.Score > maxScore) {
                    maxScore = cube.Score;
                    result = temp;
                }
            }
            return result;
        }


        /*
        private int ComputePathDecrease(int x, int y, Cube from ,Cube[,] to) {
            var result = 0;
            for (int dir = 0; dir < 8; dir ++) {
                var roundCube = to[x + _round8[dir, 0], y + _round8[dir, 1]];
                if (roundCube.IsValid) {
                    result ++;
                }
            }
            return result;
        }*/
    }
}
