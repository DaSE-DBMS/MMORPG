using Common;
using UnityEngine;

namespace Gamekit3D.Network
{
    public interface ICreatureBehavior
    {
        void UnderAttack(int HP, ICreatureBehavior source);

        void Jump();

        void Attack(ICreatureBehavior target);

        void Die();

        Vector3 GetPosition();
    }
}

