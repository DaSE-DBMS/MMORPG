/*
 * message send from Frontend to Backend
 * if you are changing this file, mind to rebuild both Frontend and Backend later
 */

using System;
using System.Collections.Generic;
using Common.Data;

namespace Common
{
    [Serializable]
    public class CLogin : Message
    {
        public CLogin() : base(Command.C_LOGIN) { }
        public string user;
        public string password;
    }

    [Serializable]
    public class CEnterSceneDone : Message
    {
        public CEnterSceneDone() : base(Command.C_ENTER_SCENE_DONE) { }
    }
    public enum MoveStep
    {
        JUMP,
        ATTACK,
        MOVE_BEGIN,
        MOVE_STEP,
        MOVE_END,
    }

    [Serializable]
    public class CPlayerMove : Message
    {
        public CPlayerMove() : base(Command.C_PLAYER_MOVE) { }
        // target is valid when action is ATTACK/MOVE
        // x, y, z is valid when action is MOVE
        public int player;
        public MoveState state;
        public V3 move;
        public V3 pos;
        public V4 rot;
    }

    [Serializable]
    public class CPlayerJump : Message
    {
        public CPlayerJump() : base(Command.C_PLAYER_JUMP) { }
        public int player;
    }

    [Serializable]
    public class CPlayerAttack : Message
    {
        public CPlayerAttack() : base(Command.C_PLAYER_ATTACK) { }
        public int player;
        public int target;
    }

    // FOR debug ...
    [Serializable]
    public class CPathFinding : Message
    {
        public CPathFinding() : base(Command.C_PATH_FINDING)
        {
            pos = new V3();
        }

        public V3 pos;
    }


    public enum ItemType
    {
        WEAPON,
    }
    [Serializable]
    public class CPlayerTake : Message
    {
        public CPlayerTake() : base(Command.C_PLAYER_TAKE)
        {
        }
        public ItemType itemType;
        public bool byName;
        public int playerId;
        public int targetId;
        public string targetName;
    }
}
