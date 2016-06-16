using System;
using UnityEngine;
using Assets.Scripts.StateMachines;

public class CharacterFlagMachine : MonoBehaviour {

    [HideInInspector]
    public bool[] Flags { get; private set; }
    [HideInInspector]
    public Vector3 MoveReg { get; private set; }
    [HideInInspector]
    public Vector3 DragReg { get; private set; }
    [HideInInspector]
    public Vector3 TransferReg { get; private set; }

    void Awake () {
	    Flags = new bool[Enum.GetNames(typeof(CharacterProcessState)).Length];
        Flags.Initialize();
	}

    void LateUpdate() {
      //  MoveReg = Vector3.zero;
      //  DragReg = Vector3.zero;
    }

    public void Action(CharacterCommand command, Vector3 direction) {
        Action<Vector3>[] functions = { MoveBegin, MoveEnd, Jump, Jumped, Grounded, DragBegin, DragEnd ,TransferBegin, TransferEnd};
        functions[(int) command](direction);
    }

    public void Action(CharacterCommand command) {
        Action<Vector3>[] functions = { MoveBegin, MoveEnd, Jump, Jumped, Grounded, DragBegin, DragEnd ,TransferBegin ,TransferEnd};
        //Debug.Log(command);
        functions[(int)command](Vector3.zero);
    }

    private void MoveBegin (Vector3 direction) {
	    Flags[(int) CharacterProcessState.Walk] = true;
        MoveReg = direction;
    }

    private void MoveEnd(Vector3 direction) {
        Flags[(int) CharacterProcessState.Walk] = false;
        MoveReg = Vector3.zero;
    }

    private void Jump(Vector3 direction) {
        Flags[(int) CharacterProcessState.Jump] = true;
    }

    private void Jumped(Vector3 direction) {
        Flags[(int) CharacterProcessState.Jump] = false;
        Flags[(int) CharacterProcessState.SJump] = Flags[(int) CharacterProcessState.FJump] && true;
        Flags[(int) CharacterProcessState.FJump] = true;
    }

    private void Grounded(Vector3 direction) {
        Flags[(int) CharacterProcessState.SJump] = Flags[(int) CharacterProcessState.FJump] = false;
    }

    private void DragBegin(Vector3 direction) {
        Flags[(int) CharacterProcessState.Drag] = true;
        DragReg = direction;
    }

    private void DragEnd(Vector3 direction) {
        Flags[(int) CharacterProcessState.Drag] = false;
        DragReg = Vector3.zero;
    }

    private void TransferBegin(Vector3 postition) {
        Flags[(int) CharacterProcessState.Transfer] = true;
        TransferReg = postition;

    }

    private void TransferEnd(Vector3 direction) {
        Flags[(int) CharacterProcessState.Transfer] = false;
        TransferReg = Vector3.zero;
    }
}
