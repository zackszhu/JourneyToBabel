namespace Assets.Scripts.StateMachines {
    public enum CharacterProcessState {
        Walk = 0,
        Jump,
        FJump,
        SJump,
        Drag,
        Transfer,
    }

    public enum CharacterCommand {
        MoveBegin,
        MoveEnd,
        Jump,
        Jumped,
        Grounded,
        DragBegin,
        DragEnd,
        TransferBegin,   //准备传送
        TransferEnd      //传送完成
    }

//    public class CharacterTransition {
//        readonly CharacterProcessState _processState;
//        readonly CharacterCommand _command;
//
//        public CharacterTransition(CharacterProcessState currentState, CharacterCommand command) {
//            _processState = currentState;
//            _command = command;
//        }
//
//        public override int GetHashCode() {
//            return 17 + 31 * _processState.GetHashCode() + 31 * _command.GetHashCode();
//        }
//
//        public override bool Equals(object obj) {
//            CharacterTransition other = obj as CharacterTransition;
//            return other != null && this._processState == other._processState
//                && this._command == other._command;
//        }
//    }
}
