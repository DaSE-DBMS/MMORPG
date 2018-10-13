using System;
using System.Collections.Generic;
using Common;
using Common.Data;
using GeometRi;

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

        public void FindPath(Entity target, LinkedList<Point3d> steps)
        {
            GetScene().FindPath(Position, target.Position, steps);
        }

        public bool FindPath(Point3d target, LinkedList<Point3d> steps)
        {
            return GetScene().FindPath(Position, target, steps);
        }

        // the enemy is null if not exists one
        virtual public void Attack(Creature enemy)
        {
            if (currentHP == 0)
                return;

            SAttack attack = new SAttack();
            attack.ID = this.entityID;
            attack.targetID = enemy != null ? enemy.entityID : 0;
            Broadcast(attack);
        }

        // the enemy is null if not exists one
        virtual public void UnderAttack(Creature enemy)
        {
            if (currentHP == 0)
                return;

            if (invTime * 1000 > (DateTime.Now - m_lastHitTimestamp).TotalMilliseconds)
                return;

            // TODO calculate hit point decrease by creature's attribute
            int dec = 0;
            currentHP = currentHP - dec < 0 ? 0 : currentHP - dec;
            if (currentHP == 0)
            {
                Die();
            }

            SUnderAttack hit = new SUnderAttack();
            hit.HP = currentHP;
            hit.sourceID = enemy != null ? enemy.entityID : 0;
            hit.ID = this.entityID;
            Broadcast(hit);
        }

        virtual public void Die()
        {
            currentHP = 0;
            UpdateActive = false;
            SDie die = new SDie();
            die.ID = this.entityID;
            Broadcast(die);
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
