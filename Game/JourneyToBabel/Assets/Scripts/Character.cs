using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.StateMachines;
using UnityEngine;

namespace Assets.Scripts {
    public class Character {
        public enum TYPE {
            Player = 0,
            Friend = 1,
            Enemy = 2,
            UnKnown = 3 //待分配的意思
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

        public Cube ExpectedCube;   //将要去的方格，可能以后还要再多一个currCube变量
        //public List<Cube> CubePath;
        public HashSet<int> CubePath;


        public Character(GameObject o, Cube c) {
            Object = o;
            Type = TYPE.UnKnown;
            FlagMachine = o.GetComponent<CharacterFlagMachine>();
            Animator = o.GetComponent<Animator>();
            CubePath = new HashSet<int>();
            Id = _characterCount;
            _characterCount++;
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
