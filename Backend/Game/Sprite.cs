using System;
using System.Collections.Generic;
using Common.Data;

namespace Backend.Game
{
    public class Sprite : Creature
    {

        private int attackTarget;
        private float targetDistance;
        private Queue<V3> steps = new Queue<V3>();

        public void EnemyNear(Creature creature)
        {
            float dis = Distance(creature);
            if (dis > 3)
            {
                return;
            }
            targetDistance = dis;
            attackTarget = creature.id;
        }

        override public void BeHit(Creature creature)
        {
            update = true;
            if (hitPoints <= 0)
            {
                return;
            }
            base.BeHit(creature);
            EnemyNear(creature);
            if (hitPoints == 0)
            {// DEAD

            }
        }

        public override void Update()
        {
            if (attackTarget == 0)
                return;

            Creature enemy = (Creature)World.Instance().GetEntity(attackTarget);
            if (enemy == null)
                return;

            float distance = Distance(enemy);
            if (distance > 100)
            {
                return;
            }
            float diff = Math.Abs(distance - targetDistance);
            if (diff > 50 || steps.Count == 0)
            {
                steps.Clear();
                FindPath(enemy, steps);
            }

            if (steps != null && steps.Count > 0)
            {
                V3 pos = steps.Dequeue();
                SActionMove message = new SActionMove();
                message.pos = pos;
            }
        }

        override public DEntity ToDEntity()
        {
            DEntity entity = base.ToDEntity();
            entity.HP = hitPoints;
            entity.maxHP = maxHitPoints;
            entity.level = level;
            entity.speed = speed;
            return entity;
        }

        override public void FromDEntity(DEntity entity)
        {
            hitPoints = entity.HP;
            maxHitPoints = entity.maxHP;
            level = entity.level;
            speed = entity.speed;
            aggressive = entity.aggressive;
            base.FromDEntity(entity);
        }
    }
}
