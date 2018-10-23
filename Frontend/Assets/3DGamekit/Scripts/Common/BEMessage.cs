/*
 * message send from Backend to Frontend
 * if you are changing this file, mind to rebuild both Frontend and Backend later
 */

using System;
using System.Collections.Generic;
using Common.Data;

namespace Common
{
    [Serializable]
    public class SPlayerEnter : Message
    {
        public SPlayerEnter() : base(Command.S_PLAYER_ENTER) { }
        public string user;
        public string token;
    }


    [Serializable]
    public class SSpawn : Message
    {
        public SSpawn() : base(Command.S_SPAWN) { }
        public bool isMine;
        public DEntity entity;
    }


    public enum MoveState
    {
        BEGIN,
        STEP,
        END
    }

    [Serializable]
    public class SMove : Message
    {
        public SMove() : base(Command.S_MOVE) { }
        public int ID;
        public MoveState state;
        public V3 move;
        public V3 pos;
        public V4 rot;
    }

    [Serializable]
    public class SJump : Message
    {
        public SJump() : base(Command.S_JUMP) { }
        public int ID;
    }

    [Serializable]
    public class SUnderAttack : Message
    {
        public SUnderAttack() : base(Command.S_UNDER_ATTACK) { }
        public int ID;
        public int sourceID;
        public int HP;
    }

    [Serializable]
    public class SAttack : Message
    {
        public SAttack() : base(Command.S_ATTACK) { }
        public int ID;
        public int targetID;
    }

    [Serializable]
    public class SEquipWeapon : Message
    {
        public SEquipWeapon() : base(Command.S_EQUIP_WEAPON) { }
        public int playerID;
        public string itemName;
        public int itemID;

    }

    [Serializable]
    public class SEntityDestory : Message
    {
        public SEntityDestory() : base(Command.S_ENTITY_DESTORY) { }
        public int entityID;
    }

    [Serializable]
    public class SPlayerTakeItem : Message
    {
        public SPlayerTakeItem() : base(Command.S_PLAYER_TAKE_ITEM) { }
        public bool clone;
        public string name;
        public int itemID;
    }

    [Serializable]
    public class SExit : Message
    {
        public SExit() : base(Command.S_EXIT) { }
    }


    [Serializable]
    public class SPathFinding : Message
    {
        public SPathFinding() : base(Command.S_PATH_FINDING)
        {
            path = new List<V3>();
        }

        public List<V3> path;
    }

    [Serializable]
    public class SDie : Message
    {
        public SDie() : base(Command.S_DIE)
        { }
        public int ID;
    }
}
