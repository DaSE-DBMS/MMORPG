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
            register.Register(Command.C_PLAYER_ACTION, RecvPlayerAction);
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
            player.scene = "Level1";
            player.pos.x = 23;
            player.pos.y = 2;
            player.pos.z = 64;
            player.hitPoints = 3;
            player.maxHitPoints = 5;
            player.level = 1;
            player.name = "Ellen";

            // player will be added to scene when receive client's CEnterSceneDone message
        }

        private void RecvEnterSceneDone(IChannel channel, Message message)
        {
            CEnterSceneDone request = (CEnterSceneDone)message;
            SCreatureSpawn response = new SCreatureSpawn();
            Player player = (Player)channel.GetContent();
            if (player == null)
                return;

            Scene scene = World.Instance().GetScene(player.scene);
            if (scene == null)
                return;

            // add the player to the scene
            player.Spawn();
            scene.AddEntity(player.id, player);
        }

        private void RecvPlayerAction(IChannel channel, Message message)
        {
            CPlayerAction request = (CPlayerAction)message;
            Player player = (Player)World.Instance().GetEntity(request.player);
            if (player == null)
            {
                return;
            }
            // TODO ... costum player actions
            switch (request.code)
            {
                case PlayerActionCode.MOVE_BEGIN:
                case PlayerActionCode.MOVE_STEP:
                case PlayerActionCode.MOVE_END:
                    {
                        CPlayerMove cmsg = (CPlayerMove)request;
                        SPlayerMove smsg = new SPlayerMove();
                        smsg.player = cmsg.player;
                        smsg.code = cmsg.code;
                        smsg.pos = cmsg.pos;
                        smsg.rot = cmsg.rot;
                        // TODO, we should not trust client's message data, players may cheat...
                        // TODO, server calculate position...
                        player.GetScene().Broundcast(smsg, player.id);
                    }

                    break;
                case PlayerActionCode.ATTACK:
                    {
                        CPlayerAttack cmsg = (CPlayerAttack)request;
                        SPlayerAttack smsg = new SPlayerAttack();
                        smsg.player = cmsg.player;
                        smsg.code = cmsg.code;
                        player.GetScene().Broundcast(smsg, player.id);
                    }
                    break;
                case PlayerActionCode.JUMP:
                    {
                        CPlayerJump cmsg = (CPlayerJump)request;
                        SPlayerJump smsg = new SPlayerJump();
                        smsg.player = cmsg.player;
                        smsg.code = cmsg.code;
                        player.GetScene().Broundcast(smsg, player.id);
                    }
                    break;
                default:
                    break;
            }
        }
        private void RecvPathFinding(IChannel channel, Message message)
        {
            CPathFinding request = (CPathFinding)message;
            Player player = (Player)channel.GetContent();
            V3 start = player.pos;
            V3 end = request.pos;
            List<V3> list;
            if (player.GetScene().FindPath(start, end, out list))
            {
                SPathFinding response = new SPathFinding();
                response.path = list;
                channel.Send(response);
            }
        }
    }
}
