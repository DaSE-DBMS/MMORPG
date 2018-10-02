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
    public class SItemSpawn : Message
    {
        public SItemSpawn() : base(Command.S_ENTITY_SPAWN) { }
        public int id;
        public float positionX;
        public float positionY;
        public float positionZ;
        public float rotationX;
        public float rotationY;
        public float rotationZ;
        public float rotationW;
        public string objectName;
    }


    [Serializable]
    public class SEntitySpawn : Message
    {
        public SEntitySpawn() : base(Command.S_ENTITY_SPAWN) { }
        public bool isMine;
        public DEntity entity;
    }


    public enum MoveState
    {
        BEGIN,
        STEP,
        END,
        //Add your user defined message type after
    }

    [Serializable]
    public class SActionMove : Message
    {
        public SActionMove() : base(Command.S_ACTION_MOVE) { }
        public int id;
        public MoveState state;
        // target is valid when action is ATTACK/MOVE
        // x, y, z is valid when action is MOVE
        public V2 move;
        public V3 pos;
        public V4 rot;
    }

    [Serializable]
    public class SActionJump : Message
    {
        public SActionJump() : base(Command.S_ACTION_JUMP) { }
        public int id;
    }

    [Serializable]
    public class UnderHit : Message
    {
        public UnderHit() : base(Command.S_BE_HITTED) { }
        public int sourceID;
        public int targetID;
        public int HP;
    }

    [Serializable]
    public class SActionAttack : Message
    {
        public SActionAttack() : base(Command.S_ACTION_ATTACK) { }
        public int id;
        public int target;
    }

    [Serializable]
    public class SEquipWeapon : Message
    {
        public SEquipWeapon() : base(Command.S_EQUIP_WEAPON) { }
        public int itemID;
    }

    [Serializable]
    public class SEntityDestory : Message
    {
        public SEntityDestory() : base(Command.S_ENTITY_DESTORY) { }
        public int playerID;
    }

    [Serializable]
    public class STakeItem : Message
    {
        public STakeItem() : base(Command.S_TAKE_ITEM) { }
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
}
