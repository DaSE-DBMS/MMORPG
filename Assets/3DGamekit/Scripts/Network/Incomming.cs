using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using Gamekit3D;
using Common.Data;

namespace Frontend.Network
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
            register.Register(Command.S_CREATURE_SPAWN, RecvCreatureSpawn);
            register.Register(Command.S_PLAYER_ACTION, RecvPlayerAction);
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

        private void RecvCreatureSpawn(IChannel channel, Message message)
        {
            SCreatureSpawn m = (SCreatureSpawn)message;
            GameObject toClone;
            bool found = networkObjects.TryGetValue(m.objectName, out toClone);
            if (!found)
            {
                return;
            }

            GameObject clone = GameObject.Instantiate(toClone);
            Damageable damageable = clone.GetComponent<Damageable>();
            NetworkEntity entity = clone.GetComponent<NetworkEntity>();
            if (damageable == null || entity == null)
            {
                return;
            }

            // Do not use transform.position.Set(x, y, z), because position return a cloned Vector3 object
            clone.transform.position = new Vector3(m.positionX, m.positionY, m.positionZ);
            clone.transform.rotation = new Quaternion(m.rotationX, m.rotationY, m.rotationZ, m.rotationW);
            damageable.currentHitPoints = m.hitPoints;
            damageable.maxHitPoints = m.maxHitPoints;
            entity.id = m.id;

            clone.SetActive(true);


            // if this creature is a player ... TODO type test
            PlayerController controller = clone.GetComponent<PlayerController>();
            PlayerInput input = clone.GetComponent<PlayerInput>();
            if (!gameObjects.ContainsKey(m.id))
            {
                gameObjects.Add(m.id, clone);
            }
            if (controller == null || input == null || entity == null)
            {
                return;
            }

            if (m.isMine)
            {
                thisPlayer = controller;
                channel.SetContent(clone);
                controller.InitLocalPlayer();
            }
            else
            {
                damageable.enabled = false;
                input.enabled = false;
            }
        }

        private void RecvPlayerAction(IChannel channel, Message message)
        {
            SPlayerAction action = (SPlayerAction)message;

            GameObject player;
            if (!gameObjects.TryGetValue(action.player, out player))
            {
                return;
            }

            PlayerController controller = player.GetComponent<PlayerController>();
            if (controller == null)
            {
                return;
            }
            switch (action.code)
            {
                case PlayerActionCode.MOVE_BEGIN:
                    controller.RecvMoveBegin();
                    break;
                case PlayerActionCode.MOVE_STEP:
                    {
                        SPlayerMove m = (SPlayerMove)message;
                        controller.RecvMoveStep(m.movementX, m.movementY,
                            m.positionX, m.positionY, m.positionZ,
                            m.rotationX, m.rotationY, m.rotationZ, m.rotationW);
                    }
                    break;
                case PlayerActionCode.MOVE_END:
                    {
                        SPlayerMove m = (SPlayerMove)message;
                        controller.RecvMoveEnd(m.positionX, m.positionY, m.positionZ);
                    }
                    break;
                case PlayerActionCode.ATTACK:
                    controller.RecvAttack();
                    break;
                case PlayerActionCode.JUMP:
                    controller.RecvJump();
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
            foreach (Pos p in msg.path)
            {
                Vector3 v = new Vector3(p.x, p.y, p.z);
                Debug.DrawLine(v, v + Vector3.up * 10, Color.red, 60);
            }
        }
    }
}
