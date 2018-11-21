using System;
using System.Collections.Generic;
using Common;
using GeometRi;
namespace Backend.Game
{
    public class Sprite : Creature
    {
        enum ChaseState
        {
            IDLE,
            CHASING_ENEMY,
            ATTACKING,
            BACK_TO_HOME
        }

        // for sprite search path
        private ChaseState m_chaseState = ChaseState.IDLE;
        private int m_targetID;
        private LinkedList<Point3d> m_routeSteps = new LinkedList<Point3d>();
        // target position when I find my path to it last time
        private Point3d m_targetPos = new Point3d();
        DateTime m_lastMoveTS = DateTime.UnixEpoch;

        const float DistanceEpsilon = 3.0f;
        const float LongDistance = 100.0f;


        public void EnemyClosing(Creature creature)
        {
            if (Position.DistanceTo(creature.Position) > 3.0)
            {
                return;
            }
            m_targetPos = creature.Position;
            m_targetID = creature.entityId;
            m_chaseState = ChaseState.CHASING_ENEMY;
            UpdateActive = true;
        }
        // the enemy is null if not exists one
        public override void OnHit(Creature enemy, int hpDec)
        {
            if (currentHP == 0)
                return;

            if (IsInvulnerable())
                return;

            // TODO calculate hit point decrease by creature's attribute
            hpDec = currentHP - hpDec < 0 ? currentHP : hpDec;
            SHit hit = new SHit();
            hit.decHP = hpDec;
            hit.sourceId = enemy != null ? enemy.entityId : 0;
            hit.targetId = this.entityId;
            Broadcast(hit);
            currentHP = currentHP - hpDec;
            if (currentHP == 0)
            {
                OnDie();
                World.Instance.DelayInvoke(20, OnReSpawn);
            }
            else
            {
                EnemyClosing(enemy);
            }
        }

        public override void Update()
        {
            ChaseEnemy();
        }

        private void ChaseEnemy()
        {
            switch (m_chaseState)
            {
                case ChaseState.IDLE:
                    {
                        UpdateActive = false;
                        return;
                    }
                case ChaseState.CHASING_ENEMY:
                    {
                        Creature target = m_targetID == 0 ? null : (Creature)World.Instance.GetEntity(m_targetID);
                        if (target == null)
                        {
                            StartBackToHome();
                            return;
                        }
                        // the distance to target
                        float distance = (float)Position.DistanceTo(target.Position);
                        if (distance > LongDistance)
                        {
                            // too far away, I cannot catch up my target, so I give up
                            StartBackToHome();
                            return;
                        }

                        if (m_targetPos.DistanceTo(target.Position) > DistanceEpsilon)
                        {
                            // the target is moving
                            // target deviate path which I calculate last time, so I must re-calculate
                            if (!FindPath(target.Position, m_routeSteps))
                            {
                                StartBackToHome();
                                return;
                            }
                            m_targetPos = target.Position;
                            SendMove(MoveState.BEGIN, Position, m_targetID);
                            return;
                        }

                        if (distance < DistanceEpsilon)
                        {
                            // reach the destination
                            m_routeSteps.Clear();
                        }

                        if (m_routeSteps.Count == 0)
                        {
                            this.Position = target.Position;
                            SendMove(MoveState.END, target.Position, m_targetID);
                            m_chaseState = ChaseState.ATTACKING;
                        }
                        else
                        {
                            Point3d pos = m_routeSteps.First.Value;
                            m_routeSteps.RemoveFirst();
                            SendMove(MoveState.STEP, pos, m_targetID);
                        }
                    }
                    return;
                case ChaseState.BACK_TO_HOME:
                    {
                        if (m_routeSteps.Count == 0)
                        {
                            m_chaseState = ChaseState.IDLE;
                            Position = V3ToPoint3d(DefaultData.pos);
                            SendMove(MoveState.END, Position);
                        }
                        else
                        {
                            Point3d pos = m_routeSteps.First.Value;
                            m_routeSteps.RemoveFirst();
                            SendMove(MoveState.STEP, pos);
                        }
                    }
                    return;
                case ChaseState.ATTACKING:
                    {
                        Creature target = m_targetID == 0 ? null : (Creature)World.Instance.GetEntity(m_targetID);
                        if (target == null || target.currentHP == 0)
                        {
                            StartBackToHome();
                            return;
                        }

                        if (Position.DistanceTo(target.Position) < DistanceEpsilon)
                        {
                            OnAttack(target);
                            target.OnHit(this, 1);
                        }
                        else
                        {
                            m_chaseState = ChaseState.CHASING_ENEMY;
                            if (!FindPath(target.Position, m_routeSteps))
                            {
                                StartBackToHome();
                                return;
                            }

                            m_targetPos = target.Position;
                            SendMove(MoveState.BEGIN, Position, m_targetID);
                        }
                    }
                    return;
            }
        }

        private void StartBackToHome()
        {
            Point3d spawnPoint = V3ToPoint3d(DefaultData.pos);
            m_chaseState = ChaseState.BACK_TO_HOME;
            m_targetID = 0;
            if (!FindPath(spawnPoint, m_routeSteps))
            {
                // cannot find a way , something was wrong ???
                // fly to spawn point
            }
            SendMove(MoveState.BEGIN, Position);
        }

        public override void OnReSpawn()
        {
            SSpawn spawn = new SSpawn();
            Reset();
            spawn.isMine = false;
            spawn.entity = ToDEntity();
            Broadcast(spawn);
        }

        public override void OnDie()
        {

        }
        private void SendMove(MoveState state, Point3d position, int targetId = 0)
        {
            m_lastMoveTS = DateTime.Now;
            SSpriteMove message = new SSpriteMove();
            message.ID = entityId;
            message.pos = Entity.Point3dToV3(position);
            message.state = state;
            message.targetId = targetId;
            Broadcast(message);
        }
    }
}
