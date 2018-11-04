using Common;
using UnityEngine;

namespace Gamekit3D.Network
{
    public interface IPlayerBehavior
    {
        void EquipWeapon(NetworkEntity weapon);
        void TakeItem(NetworkEntity item);

        void MoveBegin(
            Vector2 move,
            Vector3 position,
            Quaternion rotation);

        void MoveStep(
            Vector2 move,
            Vector3 position,
            Quaternion rotation);

        void MoveEnd(
            Vector2 move,
            Vector3 position,
            Quaternion rotation);
    }
}
