using UnityEngine;
using Common;

namespace Gamekit3D.Network
{
    public partial class Incoming
    {
        private void OnRecvSpawn(IChannel channel, Message message)
        {
            SSpawn msg = message as SSpawn;
            GameObject go = null;
            if (msg.entity.type == (int)EntityType.PLAYER)
            {
                go = CloneGameObject(msg.entity.name, msg.entity.entityID);
            }
            else if (networkObjects.TryGetValue(msg.entity.name, out go))
            {
                NetworkEntity entity = go.GetComponent<NetworkEntity>();
                entity.entityId = msg.entity.entityID;
                if (!networkEntities.ContainsKey(entity.entityId))
                {
                    networkEntities.Add(entity.entityId, entity);
                }
            }
            if (go == null)
            {
                return;
            }
            // Do not use transform.position.Set(x, y, z)
            go.transform.position = new Vector3(msg.entity.pos.x, msg.entity.pos.y, msg.entity.pos.z);
            go.transform.rotation = new Quaternion(msg.entity.rot.x, msg.entity.rot.y, msg.entity.rot.z, msg.entity.rot.w);
            if (!msg.entity.forClone)
            {
                go.SetActive(true);
            }

            if (msg.entity.type == (int)EntityType.PLAYER
                || msg.entity.type == (int)EntityType.SPRITE)
            {
                Damageable damageable = go.GetComponent<Damageable>();
                if (damageable == null)
                {
                    return;
                }

                damageable.currentHitPoints = msg.entity.currentHP;
                damageable.maxHitPoints = msg.entity.maxHP;
                if (msg.entity.type == (int)EntityType.PLAYER)
                {
                    PlayerController controller = go.GetComponent<PlayerController>();
                    PlayerMyController mine = go.GetComponent<PlayerMyController>();
                    if (controller == null || mine == null)
                    {
                        return;
                    }

                    if (msg.isMine)
                    {
                        thisPlayer = controller;
                        channel.SetContent(go);
                        controller.InitMine();
                        HealthUI healthUI = GameObject.FindObjectOfType<HealthUI>();
                        healthUI.InitUI(damageable);
                    }
                    else
                    {
                        damageable.enabled = false;
                        mine.enabled = false;
                    }
                }
            }
        }
    }
}
