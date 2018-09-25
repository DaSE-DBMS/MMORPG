using System;
using System.Collections.Generic;
using Common.Data;

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
    public SItemSpawn() : base(Command.S_CREATURE_SPAWN) { }
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
public class SCreatureSpawn : Message
{
    public SCreatureSpawn() : base(Command.S_CREATURE_SPAWN) { }
    public int id;
    public float positionX;
    public float positionY;
    public float positionZ;
    public float rotationX;
    public float rotationY;
    public float rotationZ;
    public float rotationW;
    public int hitPoints;
    public int maxHitPoints;
    public int level;
    public bool aggressive;
    public string objectName;
    public bool isMine;
}

[Serializable]
public class SPlayerAction : Message
{

    public SPlayerAction() : base(Command.S_PLAYER_ACTION) { }
    public int player;
    public PlayerActionCode code;
    // target is valid when action is ATTACK/MOVE
}

[Serializable]
public class SPlayerMove : SPlayerAction
{
    public SPlayerMove()
    {
        code = PlayerActionCode.MOVE_BEGIN;
    }
    // target is valid when action is ATTACK/MOVE
    // x, y, z is valid when action is MOVE
    public float movementX;
    public float movementY;
    public float positionX;
    public float positionY;
    public float positionZ;
    public float rotationX;
    public float rotationY;
    public float rotationZ;
    public float rotationW;
}

[Serializable]
public class SPlayerJump : SPlayerAction
{
    public SPlayerJump()
    {
        code = PlayerActionCode.JUMP;
    }
}

[Serializable]
public class SPlayerAttack : SPlayerAction
{
    public SPlayerAttack()
    {
        code = PlayerActionCode.ATTACK;
    }
    int target;
}


[Serializable]
public class AnimationInfo
{
    public enum Operation
    {
        SET_FLOAT,
        SET_INT,
        SET_BOOL,
        SET_TRIGGER,
        RESET_TRIGGER
    };

    public Operation operation;
    public float floatValue;
    public bool boolValue;
    public int intValue;
}


[Serializable]
public class SEntityDestory : Message
{
    public SEntityDestory() : base(Command.S_ENTITY_DESTORY) { }
    public int id;
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
        path = new List<Pos>();
    }

    public List<Pos> path;
}
