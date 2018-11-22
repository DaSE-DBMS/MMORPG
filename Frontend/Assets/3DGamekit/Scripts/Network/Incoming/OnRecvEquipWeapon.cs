using System;
using UnityEngine;
using Common;

namespace Gamekit3D.Network
{
    public partial class Incoming
    {
        private void OnRecvEquipWeapon(IChannel channel, Message message)
        {
            SEquipWeapon msg = message as SEquipWeapon;
            NetworkEntity player = networkEntities[msg.playerID];
            PlayerController controller = player.gameObject.GetComponent<PlayerController>();
            if (controller == null)
                return;

            NetworkEntity weapon;
            if (networkEntities.ContainsKey(msg.itemID))
            {
                weapon = networkEntities[msg.itemID];
            }
            else
            {
                GameObject go = CloneGameObject(msg.itemName, msg.itemID);
                weapon = go.GetComponent<NetworkEntity>();
                if (weapon == null)
                    return;
                go.SetActive(false);
                controller.TakeItem(weapon);
            }
            weapon.gameObject.SetActive(false);
            if (!controller.CanAttack)
            {
                weapon.gameObject.SetActive(true);
                controller.EquipWeapon(weapon);
            }
        }
    }
}
