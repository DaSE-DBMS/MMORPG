using Common.Data;
using UnityEngine;

namespace Gamekit3D.Network
{
    public interface IPlayerBehavior
    {
        void EquipWeapon(NetworkEntity weapon);
        void TakeItem(NetworkEntity item);

    }
}
