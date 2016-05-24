using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine.Assertions.Must;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Assets.Scripts {


    class Cube {
        public bool IsValid;
        public bool IsEdge;  //optimize performance
        public HashSet<byte> FromPath;  //以该方块为目标的路径
        public HashSet<byte> ToPath;  //以该方块为起点的路径 ， 路径指的是从下方到上方运动的方向,就是0-7

        public Cube() {
            IsValid = true;
            IsEdge = false;
            FromPath = new HashSet<byte>();

            ToPath = new HashSet<byte>();

        }

        public void InValid(bool isEdge = true) {
            IsValid = false;
            FromPath.Clear();
            ToPath.Clear();
            this.IsEdge = isEdge;
        }

        public bool IsAccessiable() {
            return FromPath.Count > 0;
        }

        public bool IsHole() {
            return !IsValid && IsEdge;
        }
    }


    class MapManager {
        private int _mapLength;
        private int _mapWidth;
        private int _layerCount = 1;
        private List<Cube[,]> _mapData;

        private readonly int[,] _round4 = { { 0, 1, 10 }, { 1, 0, 10 }, { 0, -1, -1 }, { -1, 0, -1 } };
        private readonly int[,] _round8 = { { 0, 1 }, { 1, 1 }, { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, -1 }, { -1, 0 }, { -1, 1 } };

        public MapManager(int length, int width) {
            this._mapLength = length;
            this._mapWidth = width;
            this.LayerCount = 1;
            _mapData = new List<Cube[,]> {GetDefaultLayer()};
            foreach (var cube in _mapData.Last()) {
                cube.FromPath.Add(0);
            }

        }

        public int LayerCount {
            get { return _layerCount; }
            set { _layerCount = value; }
        }

        private Cube[,] GetDefaultLayer() {
            var layer = new Cube[_mapLength + 2 , _mapWidth +2];
            for (int i = 0; i < _mapLength + 2; i++) {
                for (int j = 0; j < _mapWidth + 2; j++) {
                    layer[i, j] = new Cube();
                    if (i == 0 || j == 0 || i == _mapLength + 1 || j == _mapWidth+1) {
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
            var InQueue = new bool[_mapLength+2,_mapWidth+2];
            InQueue.Initialize();*/
            
            for (var x = 1; x <= _mapLength; x++) {
                for (var y = 1; y <= _mapWidth; y++) {
                    if (IsEdge(x, y, top)) {
                        edge.Add(new int[] {x,y});
                        //InQueue[x, y] = true;
                        top[x, y].IsEdge = true; 
                    }
                }
            }

            //GenerateByPath(top,edge);
            GenerateByHole(top,edge,_mapLength+_mapWidth);

            _mapData.Add(top);
            LayerCount++;

        }


        public Cube[,] GetLayer(int layerNum) {
            return _mapData[layerNum];
        }

        public Cube[,] GetTopLayer(int layerNum) {
            return _mapData[LayerCount];
        }


        //---------------help functions -----------------
        //可以想办法给无路可走的地方设置机遇门

        private bool IsEdge(int x, int y, Cube[,] sample)
        {
            if (OutOfRange(x, y)) return true;
            //Cube[,] temp = _mapData.Last();

            for (int i = 0; i < 8; i++)
            {
                if (sample[x + _round8[i, 0], y + _round8[i, 1]].IsValid == false)
                {
                    return true;
                }
            }
            return false;
        }

        public void GenerateByHole(Cube[,] top, List<int[]> edge, int HoleCount = 20, int midRate = 70 )
        {
            var lastLayer = _mapData.Last();

 
            var lowPriority = new List<int[]>();
            

            while (HoleCount >0 && edge.Count > 0)
            {
                int index = Random.Range(0, edge.Count);

                var x = edge[index][0];
                var y = edge[index][1];

                if (IsHoleAround(x, y, lastLayer)) {
                    if (!lastLayer[x, y].IsHole() || Random.Range(0, 100) <= midRate) {
                        top[x, y].InValid(); //high priority
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


            while (HoleCount>0 && lowPriority.Count > 0) {
                int index = Random.Range(0, lowPriority.Count);
                var x = lowPriority[index][0];
                var y = lowPriority[index][1];
                top[x,y].InValid();
                HoleCount --;
                lowPriority.RemoveAt(index);

            }
        }


        public bool IsHoleAround(int x, int y, Cube[,] layer) {
            for (int i = x-1; i <= x+1; i++ )
                for (int j = y-1; j <= y+1; j++)
                    if (layer[i, j].IsHole()) {
                        return true;
                    }
            return false;
        }

        // 一个测试用的根据路劲删除方块的算法,但是效果不好，现在用上方那个空洞衍生法。
        public void GenerateByPath(Cube[,] top, List<int[]> edge, int minPath = 5)
        {
            int pathCount = 0;
            var lastLayer = _mapData.Last();
            while (pathCount < minPath && edge.Count > 0)
            {
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
                if (pathCountAddition >= 0)
                {  //bfs  could remove
                    pathCount += pathCountAddition;

                    // remove a cube on top layer
                    var target = top[x, y];
                    foreach (byte dir in top[x, y].FromPath)
                    {
                        var tx = x - _round8[dir, 0];
                        var ty = y - _round8[dir, 1];
                        lastLayer[tx, ty].ToPath.Remove(dir);
                    }
                    target.InValid();

                    // update paths from last layer to top layer
                    lastLayer[x, y].ToPath = paths;

                    foreach (byte dir in paths)
                    {
                        var tx = x + _round8[dir, 0];
                        var ty = y + _round8[dir, 1];
                        top[tx, ty].FromPath.Add(dir);
                    }

                    //insert new edge cube into edge-queue
                    /*for (byte dir = 0; dir < 4; dir++)
                    {
                        var tx = x + _round4[dir, 0];
                        var ty = y + _round4[dir, 1];
                        var roundCube = lastLayer[tx, ty];
                        
                        if (roundCube.IsValid && InQueue[tx, ty] == false) {
                            edge.Add(new int[] {tx,ty});
                            InQueue[tx, ty] = true;
                        }
                    }*/
                }
            }
        }


        public bool OutOfRange(int x, int y) {
            return (x > _mapLength || x < 1 || y > _mapWidth || y < 1);
        }


        //从下层方块x,y走到上层的路径数量
        private HashSet<byte> GetPath(int x, int y , Cube[,] from , Cube[,] to) {

            var result = new HashSet<byte>();
            var origin = from[x, y];
            if (!origin.IsValid || !origin.IsAccessiable())
                return result;

            for (byte dir = 0; dir < 8; dir++) {
                var tx = x + _round8[dir, 0];
                var ty = y + _round8[dir, 1];
                //Debug.Log("coord" + x + " " + y);
                var roundCube = to[tx,ty];
                if (roundCube.IsValid && IsEdge(tx,ty,to)) {
                    result.Add(dir);
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
