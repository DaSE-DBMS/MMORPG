using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using Gamekit3D;
using Common.Data;

namespace Gamekit3D.Network
{
    public class Incomming
    {
        public Dictionary<string, GameObject> networkObjects = new Dictionary<string, GameObject>();
        public Dictionary<int, GameObject> gameObjects = new Dictionary<int, GameObject>();
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

            // DEBUG ...
            register.Register(Command.S_PATH_FINDING, RecvPathFinding);
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
            SEntitySpawn m = (SEntitySpawn)message;
            GameObject go;
            bool found = networkObjects.TryGetValue(m.entity.name, out go);
            if (!found)
            {
                return;
            }
            if (m.entity.type == (int)EntityType.PLAYER)
            {
                go = GameObject.Instantiate(go);
            }

            NetworkEntity entity = go.GetComponent<NetworkEntity>();
            if (entity == null)
            {
                return;
            }

            // Do not use transform.position.Set(x, y, z)
            go.transform.position = new Vector3(m.entity.pos.x, m.entity.pos.y, m.entity.pos.z);
            go.transform.rotation = new Quaternion(m.entity.rot.x, m.entity.rot.y, m.entity.rot.z, m.entity.rot.w);
            Damageable damageable = go.GetComponent<Damageable>();
            if (damageable == null)
            {
                return;
            }

            damageable.currentHitPoints = m.entity.HP;
            damageable.maxHitPoints = m.entity.maxHP;
            entity.id = m.entity.id;

            go.SetActive(true);


            // if this creature is a player ... TODO type test
            PlayerController controller = go.GetComponent<PlayerController>();
            PlayerInput input = go.GetComponent<PlayerInput>();
            if (!gameObjects.ContainsKey(m.entity.id))
            {
                gameObjects.Add(m.entity.id, go);
            }
            if (controller == null || input == null || entity == null)
            {
                return;
            }

            if (m.isMine && m.entity.type == (int)EntityType.PLAYER)
            {
                thisPlayer = controller;
                channel.SetContent(go);
                controller.InitLocalPlayer();
            }
            else
            {
                damageable.enabled = false;
                input.enabled = false;
            }
        }

        private void RecvActionAttack(IChannel channel, Message message)
        {
            SActionAttack msg = (SActionAttack)message;
            GameObject attacker;
            if (!gameObjects.TryGetValue(msg.id, out attacker))
            {
                return;
            }
            /*
            GameObject target;
            if (!gameObjects.TryGetValue(msg.target, out target))
            {
                return;
            }
            */
            IAttackable attackable = attacker.GetComponent<IAttackable>();
            if (attackable == null)
            {
                return;
            }
            attackable.RecvActionAttack();
        }

        private void RecvActionJump(IChannel channel, Message message)
        {
            SActionMove msg = (SActionMove)message;
            GameObject go;
            if (!gameObjects.TryGetValue(msg.id, out go))
            {
                return;
            }
            IJumpable jumpable = go.GetComponent<IJumpable>();
            if (jumpable == null)
            {
                return;
            }
            jumpable.RecvActionJump();
        }

        private void RecvActionMove(IChannel channel, Message message)
        {
            SActionMove msg = (SActionMove)message;
            GameObject go;
            if (!gameObjects.TryGetValue(msg.id, out go))
            {
                return;
            }
            IMoveable moveable = go.GetComponent<IMoveable>();
            switch (msg.state)
            {
                case MoveState.BEGIN:
                    {
                        moveable.RecvActionMoveBegin(msg.move, msg.pos, msg.rot);
                        break;
                    }
                case MoveState.STEP:
                    {
                        moveable.RecvActionMoveStep(msg.move, msg.pos, msg.rot);
                    }
                    break;
                case MoveState.END:
                    {
                        moveable.RecvActionMoveEnd(msg.move, msg.pos, msg.rot);
                    }
                    break;
                default:
                    break;
            }
        }

        private void RecvEntityDestory(IChannel channel, Message message)
        {
            SEntityDestory msg = (SEntityDestory)message;
            GameObject go;
            if (gameObjects.TryGetValue(msg.id, out go))
            {
                GameObject.Destroy(go);
            }
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

        private void RecvBeHit(IChannel channel, Message message)
        {
            BeHit msg = (BeHit)message;
            GameObject go = gameObjects[msg.id];
            GameObject source = gameObjects[msg.source];
            IHitable hitable = go.GetComponent<IHitable>();
            if (hitable == null)
                return;

            hitable.RecvHit(msg.HP, source);
        }
    }
}
