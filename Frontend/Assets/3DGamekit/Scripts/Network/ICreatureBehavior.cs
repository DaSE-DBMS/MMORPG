using Common;
using UnityEngine;

namespace Gamekit3D.Network
{
    public interface ICreatureBehavior
    {
        void BeHit(int HP, ICreatureBehavior source);

        void Jump();

        void Attack(ICreatureBehavior target);

        void Die();

        Vector3 GetPosition();
    }
}

