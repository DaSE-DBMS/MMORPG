using System;
using System.Collections.Generic;
using Common.Data;

namespace Backend.Game
{
    public class Creature : Entity
    {
        public int currentHP;
        public int maxHP;
        public int level;
        public int speed;
        public float invTime;
        public float hitAngle;
        public bool dead = false;
        public bool aggressive = false;
        public string scene;
        private DateTime m_lastHitTimestamp = DateTime.UnixEpoch;
        public void FindPath(Entity target, Queue<V3> steps)
        {
            GetScene().FindPath(pos, target.pos, steps);
        }

        virtual public void BeHit(Creature enemy)
        {
            if (currentHP == 0)
                return;

            if (invTime * 1000 > (DateTime.Now - m_lastHitTimestamp).TotalMilliseconds)
                return;

            // TODO calculate hit point decrease by creature's attribute
            int dec = 0;
            currentHP = currentHP - dec < 0 ? 0 : currentHP - dec;

            UnderHit hit = new UnderHit();
            hit.HP = currentHP;
            hit.id = enemy.id;
            hit.source = this.id;
            Broundcast(hit);
        }

        override public DEntity ToDEntity()
        {
            DEntity entity = base.ToDEntity();
            entity.currentHP = currentHP;
            entity.maxHP = maxHP;
            entity.level = level;
            entity.speed = speed;

            return entity;
        }

        override public void FromDEntity(DEntity entity)
        {
            currentHP = entity.currentHP;
            maxHP = entity.maxHP;
            level = entity.level;
            speed = entity.speed;
            aggressive = entity.aggressive;
            invTime = entity.invTime;
            hitAngle = entity.hitAngle;
            base.FromDEntity(entity);
        }
    }
}
