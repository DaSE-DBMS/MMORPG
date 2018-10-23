using Common.Data;
using UnityEngine;

namespace Gamekit3D.Network
{
    public interface ICreatureBehavior
    {
        void UnderAttack(int HP, ICreatureBehavior source);

        void Jump();

        void Attack(ICreatureBehavior target);

        void Die();

        void MoveBegin(
            V3 move,
            V3 pos,
            V4 rot);

        void MoveStep(
            V3 move,
            V3 pos,
            V4 rot);

        void MoveEnd(V3 move,
            V3 pos,
            V4 rot);

        Vector3 GetPosition();
    }
}

