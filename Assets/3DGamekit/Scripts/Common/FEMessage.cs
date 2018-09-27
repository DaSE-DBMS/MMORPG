using System;
using System.Collections.Generic;
using Common.Data;

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
public enum PlayerActionCode
{
    JUMP,
    ATTACK,
    MOVE_BEGIN,
    MOVE_STEP,
    MOVE_END,
}

[Serializable]
public class CPlayerAction : Message
{
    public CPlayerAction() : base(Command.C_PLAYER_ACTION) { }
    public int player;
    public PlayerActionCode code;
}

[Serializable]
public class CPlayerMove : CPlayerAction
{
    public CPlayerMove()
    {
        code = PlayerActionCode.MOVE_BEGIN;
    }
    // target is valid when action is ATTACK/MOVE
    // x, y, z is valid when action is MOVE
    public V2 move;
    public V3 pos;
    public V4 rot;
}

[Serializable]
public class CPlayerJump : CPlayerAction
{
    public CPlayerJump()
    {
        code = PlayerActionCode.JUMP;
    }
}

[Serializable]
public class CPlayerAttack : CPlayerAction
{
    public CPlayerAttack()
    {
        code = PlayerActionCode.ATTACK;
    }
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
