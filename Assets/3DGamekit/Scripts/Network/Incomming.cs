using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using Gamekit3D;
using Common;
using Common.Data;

namespace Gamekit3D.Network
{
    public class Incomming
    {
        private Dictionary<string, GameObject> networkObjects = new Dictionary<string, GameObject>();
        private Dictionary<int, GameObject> gameObjects = new Dictionary<int, GameObject>();
        public Dictionary<int, NetworkEntity> networkEntities = new Dictionary<int, NetworkEntity>();
        public PlayerController thisPlayer;
        bool debug = true;
        IRegister register;
        public Incomming(IRegister register)
        {
            this.register = register;
            RegisterAll();
        }

        public void RegisterAll()
        {
            register.Register(Command.S_PLAYER_ENTER, RecvPlayerEnter);
            register.Register(Command.S_SPAWN, RecvSpawn);
            register.Register(Command.S_ATTACK, RecvAttack);
            register.Register(Command.S_JUMP, RecvJump);
            register.Register(Command.S_MOVE, RecvMove);
            register.Register(Command.S_ENTITY_DESTORY, RecvEntityDestory);
            register.Register(Command.S_EQUIP_WEAPON, RecvEquipWeapon);
            register.Register(Command.S_PLAYER_TAKE_ITEM, RecvTakeItem);
            register.Register(Command.S_UNDER_ATTACK, RecvUnderAttack);
            register.Register(Command.S_DIE, RecvDie);
            // DEBUG ...
            register.Register(Command.S_PATH_FINDING, RecvPathFinding);
        }

        public void InitNetworkEntity()
        {
            NetworkEntity[] all = GameObject.FindObjectsOfType<NetworkEntity>();
            foreach (NetworkEntity entity in all)
            {
                GameObject go = entity.gameObject;
                go.SetActive(false);
                if (networkObjects.ContainsKey(go.name))
                {
                    GameObject.Destroy(go);
                }
                else
                {
                    networkObjects[go.name] = go;
                }
            }
        }

        public GameObject CloneGameObject(string name, int entityID)
        {
            GameObject go = networkObjects[name];
            return CloneGameObject(go, entityID);
        }

        public GameObject CloneGameObject(GameObject gameObject, int entityID)
        {
            GameObject go = GameObject.Instantiate(gameObject);
            NetworkEntity entity = go.GetComponent<NetworkEntity>();
            if (entity == null)
            {
                GameObject.Destroy(go);
                return null;
            }
            entity.entityID = entityID;
            if (!networkEntities.ContainsKey(entity.entityID))
            {
                networkEntities.Add(entity.entityID, entity);
            }
            return go;
        }
        // message receive callback
        private void RecvPlayerEnter(IChannel channel, Message message)
        {
            if (debug)
            {// ignore enter scene message when debug mode
                return;
            }
            //Console.WriteLine("Receive Enter...");
            SPlayerEnter enter = (SPlayerEnter)message;
            SceneManager.LoadSceneAsync("Level1");
        }

        private void RecvSpawn(IChannel channel, Message message)
        {
            SSpawn msg = (SSpawn)message;
            GameObject go = null;
            if (msg.entity.type == (int)EntityType.PLAYER)
            {
                go = CloneGameObject(msg.entity.name, msg.entity.entityID);
            }
            else if (networkObjects.TryGetValue(msg.entity.name, out go))
            {
                NetworkEntity entity = go.GetComponent<NetworkEntity>();
                entity.entityID = msg.entity.entityID;
                if (!networkEntities.ContainsKey(entity.entityID))
                {
                    networkEntities.Add(entity.entityID, entity);
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
                    PlayerInput input = go.GetComponent<PlayerInput>();
                    PlayerNetSender sender = go.GetComponent<PlayerNetSender>();
                    if (controller == null || input == null || sender == null)
                    {
                        return;
                    }

                    if (msg.isMine)
                    {
                        thisPlayer = controller;
                        channel.SetContent(go);
                        controller.InitLocalPlayer();
                    }
                    else
                    {
                        damageable.enabled = false;
                        input.enabled = false;
                        sender.enabled = false;
                    }
                }
            }
        }

        private void RecvAttack(IChannel channel, Message message)
        {
            SAttack msg = (SAttack)message;
            NetworkEntity source = networkEntities[msg.ID];
            if (msg.targetID != 0)
            {
                NetworkEntity target = networkEntities[msg.targetID];
                source.creatureBehavior.Attack(target.creatureBehavior);
            }
            else
            {
                source.creatureBehavior.Attack(null);
            }
        }

        private void RecvJump(IChannel channel, Message message)
        {
            SJump msg = (SJump)message;
            NetworkEntity self = networkEntities[msg.ID];
            if (self.creatureBehavior == null)
                return;

            self.creatureBehavior.Jump();
        }

        private void RecvMove(IChannel channel, Message message)
        {
            SMove msg = (SMove)message;
            NetworkEntity self = networkEntities[msg.ID];
            if (self.creatureBehavior == null)
                return;

            switch (msg.state)
            {
                case MoveState.BEGIN:
                    {
                        self.creatureBehavior.MoveBegin(msg.move, msg.pos, msg.rot);
                        break;
                    }
                case MoveState.STEP:
                    {
                        self.creatureBehavior.MoveStep(msg.move, msg.pos, msg.rot);
                    }
                    break;
                case MoveState.END:
                    {
                        self.creatureBehavior.MoveEnd(msg.move, msg.pos, msg.rot);
                    }
                    break;
                default:
                    break;
            }
        }

        private void RecvUnderAttack(IChannel channel, Message message)
        {
            SUnderAttack msg = (SUnderAttack)message;

            NetworkEntity target = networkEntities[msg.ID];
            NetworkEntity source = null;
            if (msg.sourceID != 0)
            {
                source = networkEntities[msg.sourceID];
            }

            if (source.creatureBehavior == null || target.creatureBehavior == null)
                return;

            target.creatureBehavior.UnderAttack(msg.HP, source.creatureBehavior);

        }

        private void RecvEntityDestory(IChannel channel, Message message)
        {
            SEntityDestory msg = (SEntityDestory)message;
            GameObject go = gameObjects[msg.entityID];
            GameObject.Destroy(go);
        }

        private void RecvExit(IChannel channel, Message message)
        {
            MyNetwork.Close();
        }

        private void RecvPathFinding(IChannel channel, Message message)
        {
            SPathFinding msg = (SPathFinding)message;
            foreach (V3 p in msg.path)
            {
                Vector3 v = new Vector3(p.x, p.y, p.z);
                Debug.DrawLine(v, v + Vector3.up * 10, Color.red, 60);
            }
        }

        private void RecvEquipWeapon(IChannel channel, Message message)
        {
            SEquipWeapon msg = (SEquipWeapon)message;
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

                controller.TakeItem(weapon);
            }

            controller.EquipWeapon(weapon);
        }

        private void RecvTakeItem(IChannel channel, Message message)
        {
            SPlayerTakeItem msg = (SPlayerTakeItem)message;
            NetworkEntity item;
            if (msg.clone)
            {
                GameObject go = CloneGameObject(msg.name, msg.itemID);
                if (go == null)
                    return;
                item = networkEntities[msg.itemID];
            }
            else
            {
                item = networkEntities[msg.itemID];
            }
            thisPlayer.TakeItem(item);
        }

        private void RecvDie(IChannel channel, Message message)
        {
            SDie msg = (SDie)message;
        }
    }
}
