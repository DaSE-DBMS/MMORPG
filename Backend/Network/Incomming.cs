﻿using System;
using System.Collections.Generic;
using System.Text;

using Backend.Game;
using Common.Data;
using Backend.AI;

namespace Backend.Network
{
    public class Incomming
    {
        IRegister register;
        public Incomming(IRegister register)
        {
            this.register = register;
            RegisterAll();
        }

        private void RegisterAll()
        {
            register.Register(Command.C_LOGIN, RecvLogin);
            register.Register(Command.C_ENTER_SCENE_DONE, RecvEnterSceneDone);
            register.Register(Command.C_PLAYER_MOVE, RecvPlayerMove);
            register.Register(Command.C_PLAYER_JUMP, RecvPlayerJump);
            register.Register(Command.C_PLAYER_ATTACK, RecvPlayerAttack);
            register.Register(Command.C_PLAYER_TAKE, RecvPlayerTake);


            // DEBUG ..
            register.Register(Command.C_PATH_FINDING, RecvPathFinding);

        }

        private void RecvLogin(IChannel channel, Message message)
        {
            CLogin request = (CLogin)message;
            SPlayerEnter response = new SPlayerEnter();
            response.user = request.user;
            response.token = request.user;
            channel.Send(response);
            Player player = new Player(channel);
            DEntity dentity;
            // TODO .. read from database if exists
            if (!World.Instance().InitialData.TryGetValue("Ellen", out dentity))
            {
                return;
            }
            player.FromDEntity(dentity);

            // player will be added to scene when receive client's CEnterSceneDone message
        }

        private void RecvEnterSceneDone(IChannel channel, Message message)
        {
            CEnterSceneDone request = (CEnterSceneDone)message;
            SEntitySpawn response = new SEntitySpawn();
            Player player = (Player)channel.GetContent();
            if (player == null)
                return;

            Scene scene = World.Instance().GetScene(player.scene);
            if (scene == null)
                return;

            // add the player to the scene
            player.Spawn();
            scene.AddEntity(player);
        }

        private void RecvPlayerJump(IChannel channel, Message message)
        {
            CPlayerJump request = (CPlayerJump)message;
            Player player = (Player)World.Instance().GetEntity(request.player);
            if (player == null)
            {
                return;
            }
            SActionJump response = new SActionJump();
            response.id = request.player;
            player.Broundcast(response);
        }

        private void RecvPlayerAttack(IChannel channel, Message message)
        {
            CPlayerAttack request = (CPlayerAttack)message;
            Player player = (Player)World.Instance().GetEntity(request.player);
            if (player == null)
                return;

            Entity target = (Player)World.Instance().GetEntity(request.target);
            if (target == null)
                return;


            if (target.GetType() == typeof(Sprite))
            {
                Sprite sprite = (Sprite)target;
                sprite.BeHit(player);
            }
            SActionAttack response = new SActionAttack();
            response.id = request.player;
            response.target = request.target;
            player.Broundcast(response);
        }

        private void RecvPlayerMove(IChannel channel, Message message)
        {
            CPlayerMove request = (CPlayerMove)message;
            Player player = (Player)World.Instance().GetEntity(request.player);
            if (player == null)
            {
                return;
            }
            player.pos = request.pos;
            SActionMove response = new SActionMove();
            response.id = request.player;
            response.state = request.state;
            response.pos = request.pos;
            response.rot = request.rot;
            response.state = request.state;
            player.Broundcast(response);
        }

        private void RecvPathFinding(IChannel channel, Message message)
        {
            CPathFinding request = (CPathFinding)message;
            Player player = (Player)channel.GetContent();
            V3 start = player.pos;
            V3 end = request.pos;
            Queue<V3> path = new Queue<V3>();
            if (player.GetScene().FindPath(start, end, path))
            {
                SPathFinding response = new SPathFinding();
                foreach (V3 v in path)
                {
                    response.path.Add(v);
                }
                channel.Send(response);
            }
        }

        private void RecvPlayerTake(IChannel channel, Message message)
        {
            CPlayerTake request = (CPlayerTake)message;
            Player player = (Player)channel.GetContent();

            Entity target = World.Instance().GetEntity(request.targetId);
            if (target == null)
                return;
            Item item = (Item)target;
            player.TakeItem(item);
            if (request.itemType == ItemType.WEAPON)
            {
                player.EquipWeapon(item);
            }
        }
    }
}
