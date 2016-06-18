using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.StateMachines;
using UnityEngine;

namespace Assets.Scripts {
    [Serializable]
    public class CharacterTypeConifg {
        public int Sum = 10; //总的上限人数
        public float AppendTimeGap = 0.5f; //ai是一个一个添加的，两个之间可以设置时间差，如果设置为0就是马上创建至上限。
        public int FriendSum = 6;  //盟军最大人数，Sum - friendSum 就是敌人最大人数
        public float TransferRate = 0.3f; //一条路走到底之后，执行传送的概率


    }



    public class Character {
        public enum TYPE {
            Player = 0,
            Friend = 1,
            Enemy = 2,
            UnKnown = 3 //待分配的意思
        }

        private static int _characterIdCount = 0;

  
        public GameObject Object;
        public TYPE Type;
        public int Id;
        public bool IsDead = false;
        
        //public float AITime;
        public Vector3 Suggestion;
        public CharacterFlagMachine FlagMachine;
        public Animator Animator;

        //public Vector3 Preposition;
        public Cube CurrentCube; //现在所在的方块
        public Cube ExpectedCube;   //将要去的方格，可能以后还要再多一个currCube变量
        //public List<Cube> CubePath;
        public HashSet<int> CubePath;



        public Character(GameObject o, Cube c, TYPE type) {
            Object = o;
            Type = type;
            FlagMachine = o.GetComponent<CharacterFlagMachine>();
            Animator = o.GetComponent<Animator>();
            CubePath = new HashSet<int>();
            Id = _characterIdCount;
            _characterIdCount++;
            Restart(c);
        }

        public void Restart(Cube startCube) {
            CubePath.Clear();
            CubePath.Add(startCube.Id);
            ExpectedCube = startCube;
        }

        public void SetNext() {
        }


        public void Walk(Vector3 direction) {
            Animator.SetBool("isWalking", true);
            FlagMachine.Action(CharacterCommand.MoveBegin, direction);
        }

        public void StopWalk() {
            Animator.SetBool("isWalking", false);
            FlagMachine.Action(CharacterCommand.MoveEnd);
        }

        public void Jump() {
            Animator.SetTrigger("jump");
            FlagMachine.Action(CharacterCommand.Jump);
        }

        public void Transfer(Cube target) {
            FlagMachine.Action(CharacterCommand.TransferBegin, target.OriginPostion - CharacterManager.CharacterYOffest);
            Restart(target);
        }


    }
}
