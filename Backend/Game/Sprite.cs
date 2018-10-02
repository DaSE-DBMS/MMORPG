using System;
using System.Collections.Generic;
using Common;
using Common.Data;

namespace Backend.Game
{
    public class Sprite : Creature
    {

        private int attackTarget;
        private float targetDistance;
        private Queue<V3> steps = new Queue<V3>();
        bool m_moving = true;
        public void EnemyNear(Creature creature)
        {
            float dis = Distance(creature);
            if (dis > 3)
            {
                return;
            }
            targetDistance = dis;
            attackTarget = creature.entityID;
        }

        override public void BeHit(Creature creature)
        {
            if (currentHP <= 0)
            {
                return;
            }
            update = true;
            base.BeHit(creature);
            EnemyNear(creature);
            if (currentHP == 0)
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
            if (diff > 3 || steps.Count == 0)
            {
                steps.Clear();
                FindPath(enemy, steps);
            }

            if (steps.Count > 0)
            {
                V3 pos = steps.Dequeue();
                this.pos = pos;
                SActionMove message = new SActionMove();
                message.id = entityID;
                message.pos = pos;
                message.state = m_moving ? MoveState.STEP : MoveState.BEGIN;
                Broundcast(message);
                m_moving = true;
                if (steps.Count == 0)
                {
                    steps.Enqueue(enemy.pos);
                }
            }
            else if (m_moving)
            {
                m_moving = false;
                this.pos = enemy.pos;
                SActionMove message = new SActionMove();
                message.id = entityID;
                message.pos = enemy.pos;
                message.state = MoveState.END;
                Broundcast(message);
            }
        }
    }
}
