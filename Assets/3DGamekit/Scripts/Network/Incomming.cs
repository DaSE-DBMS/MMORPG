﻿using System;
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
            register.Register(Command.S_PLAYER_ENTER, RecvEnter);
            register.Register(Command.S_ENTITY_SPAWN, RecvEntitySpawn);
            register.Register(Command.S_ACTION_ATTACK, RecvActionAttack);
            register.Register(Command.S_ACTION_JUMP, RecvActionJump);
            register.Register(Command.S_ACTION_MOVE, RecvActionMove);
            register.Register(Command.S_ENTITY_DESTORY, RecvEntityDestory);
            register.Register(Command.S_EQUIP_WEAPON, RecvWeaponEquiped);
            register.Register(Command.S_TAKE_ITEM, RecvTakeItem);
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
        private void RecvEnter(IChannel channel, Message message)
        {
            if (debug)
            {// ignore enter scene message when debug mode
                return;
            }
            //Console.WriteLine("Receive Enter...");
            SPlayerEnter enter = (SPlayerEnter)message;
            SceneManager.LoadSceneAsync("Level1");
        }

        private void RecvEntitySpawn(IChannel channel, Message message)
        {
            SEntitySpawn msg = (SEntitySpawn)message;
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

        private void RecvActionAttack(IChannel channel, Message message)
        {
            SActionAttack msg = (SActionAttack)message;
            NetworkEntity self = networkEntities[msg.id];
            NetworkEntity target = networkEntities[msg.target];
            self.creatureBehavior.ActionAttack(target.creatureBehavior);
        }

        private void RecvActionJump(IChannel channel, Message message)
        {
            SActionJump msg = (SActionJump)message;
            NetworkEntity self = networkEntities[msg.id];
            if (self.creatureBehavior == null)
                return;

            self.creatureBehavior.ActionJump();
        }

        private void RecvActionMove(IChannel channel, Message message)
        {
            SActionMove msg = (SActionMove)message;
            NetworkEntity self = networkEntities[msg.id];
            if (self.creatureBehavior == null)
                return;

            switch (msg.state)
            {
                case MoveState.BEGIN:
                    {
                        self.creatureBehavior.ActionMoveBegin(msg.move, msg.pos, msg.rot);
                        break;
                    }
                case MoveState.STEP:
                    {
                        self.creatureBehavior.ActionMoveStep(msg.move, msg.pos, msg.rot);
                    }
                    break;
                case MoveState.END:
                    {
                        self.creatureBehavior.ActionMoveEnd(msg.move, msg.pos, msg.rot);
                    }
                    break;
                default:
                    break;
            }
        }

        private void RecvBeHit(IChannel channel, Message message)
        {
            UnderHit msg = (UnderHit)message;
            NetworkEntity self = networkEntities[msg.sourceID];
            NetworkEntity source = networkEntities[msg.targetID];
            if (self.creatureBehavior == null || source.creatureBehavior == null)
                return;

            self.creatureBehavior.ReHited(msg.HP, source.creatureBehavior);
        }

        private void RecvEntityDestory(IChannel channel, Message message)
        {
            SEntityDestory msg = (SEntityDestory)message;
            GameObject go = gameObjects[msg.playerID];
            GameObject.Destroy(go);
        }

        private void RecvExit(IChannel channel, Message message)
        {
            MyNetwork.instance.Close();
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

        private void RecvWeaponEquiped(IChannel channel, Message message)
        {
            SEquipWeapon msg = (SEquipWeapon)message;
            NetworkEntity weapon = networkEntities[msg.itemID];
            thisPlayer.EquipWeapon(weapon);
        }

        private void RecvTakeItem(IChannel channel, Message message)
        {
            STakeItem msg = (STakeItem)message;
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
    }
}
