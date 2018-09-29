using Common.Data;
using UnityEngine;

namespace Gamekit3D.Network
{
    public interface ICreatureBehavior
    {
        void ReHited(int HP, ICreatureBehavior source);

        void ActionJump();

        void ActionAttack(ICreatureBehavior target);

        void ActionMoveBegin(
            V2 move,
            V3 pos,
            V4 rot);

        void ActionMoveStep(
            V2 move,
            V3 pos,
            V4 rot);

        void ActionMoveEnd(V2 move,
            V3 pos,
            V4 rot);

        Vector3 GetPosition();
    }
}

