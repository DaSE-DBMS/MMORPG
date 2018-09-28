using System;
using System.Collections.Generic;
using Common.Data;

namespace Backend.Game
{
    public class Player : Creature
    {
        public IChannel connection;
        public string user;
        public string token;

        public Player(IChannel channel)
        {
            connection = channel;
            channel.SetContent(this);
        }
        override public void BeHit(Creature creature)
        {
            base.BeHit(creature);
        }

        override public DEntity ToDEntity()
        {
            DEntity entity = base.ToDEntity();
            entity.HP = hitPoints;
            entity.maxHP = maxHitPoints;
            entity.level = level;
            entity.speed = speed;
            entity.type = (int)EntityType.PLAYER;
            return entity;
        }

        override public void FromDEntity(DEntity entity)
        {
            scene = "Level1";
            pos = entity.pos;
            rot = entity.rot;
            hitPoints = entity.HP;
            maxHitPoints = entity.maxHP;
            level = 1;
            name = "Ellen";
            base.FromDEntity(entity);
        }

        virtual public void OnEnterScene(Scene scene)
        {

        }

        virtual public void OnLeaveScene(Scene scene)
        {

        }

        virtual public void OnEquiped(Item item)
        {

        }

        virtual public void OnUnEquiped(Item item)
        {

        }

        virtual public void OnDie()
        {

        }

        virtual public void OnBirth()
        {

        }

        public void SendSpawn(DEntity entity)
        {
            SEntitySpawn msg = new SEntitySpawn();
            msg.entity = entity;
            msg.isMine = entity.id == id;
            connection.Send(msg);
        }

        override public void Spawn()
        {
        }

        override public void Vanish()
        {

        }
    }
}
