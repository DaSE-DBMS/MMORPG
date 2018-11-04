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
        private Point3d m_spawnPoint = new Point3d();
        DateTime m_lastMoveTS = DateTime.UnixEpoch;

        const float DistanceEpsilon = 3.0f;
        const float LongDistance = 100.0f;


        public void EnemyNear(Creature creature)
        {
            if (Position.DistanceTo(creature.Position) > 3.0)
            {
                return;
            }
            m_targetPos = creature.Position;
            m_targetID = creature.entityId;
            m_chaseState = ChaseState.CHASING_ENEMY;
        }

        override public void UnderAttack(Creature creature)
        {
            if (currentHP <= 0)
            {
                return;
            }
            UpdateActive = true;
            base.UnderAttack(creature);
            EnemyNear(creature);
            if (currentHP == 0)
            {// DEAD

            }
        }

        public override void Update()
        {
            ChaseEnemy();
        }
        private void ChaseEnemy()
        {
            Creature target = m_targetID == 0 ? null : (Creature)World.Instance().GetEntity(m_targetID);
            if (m_chaseState == ChaseState.IDLE)
            {
                UpdateActive = false;
                return;
            }
            else if (m_chaseState == ChaseState.CHASING_ENEMY)
            {
                Point3d targetPos = target.Position;
                float distance = (float)Position.DistanceTo(targetPos);
                if (distance > LongDistance)
                {
                    // too far away, I cannot catch up my enemy, so I give up
                    StartBackToHome();
                    return;
                }
                if (targetPos.DistanceTo(m_targetPos) > DistanceEpsilon)
                {
                    // the target is moving
                    //the route I found last time was behind the time...
                    FindPath(m_chaseState, targetPos);
                    m_targetPos = targetPos;
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
                    Attack(target);
                }
                else
                {
                    Point3d pos = m_routeSteps.First.Value;
                    m_routeSteps.RemoveFirst();
                    SendMove(MoveState.STEP, pos, m_targetID);
                }
            }
            else if (m_chaseState == ChaseState.BACK_TO_HOME)
            {
                if (m_routeSteps.Count == 0)
                {
                    m_chaseState = ChaseState.IDLE;
                    this.Position = m_spawnPoint;
                    SendMove(MoveState.END, m_spawnPoint);
                }
                else
                {
                    Point3d pos = m_routeSteps.First.Value;
                    m_routeSteps.RemoveFirst();
                    SendMove(MoveState.STEP, pos);
                }
            }
            else if (m_chaseState == ChaseState.ATTACKING)
            {

                if (Position.DistanceTo(target.Position) < DistanceEpsilon)
                {
                    Attack(target);
                }
                else
                {
                    m_chaseState = ChaseState.CHASING_ENEMY;
                    FindPath(ChaseState.CHASING_ENEMY, target.Position);
                    m_targetPos = target.Position;
                    SendMove(MoveState.BEGIN, Position, m_targetID);
                }
            }
        }

        private void FindPath(ChaseState state, Point3d target)
        {
            if (!FindPath(target, m_routeSteps))
            {
                // cannot find a way , something was wrong ???
                // fly to spawn point
                if (state == ChaseState.BACK_TO_HOME)
                {
                    // cannot find a way , something was wrong ???
                    // fly to spawn point
                }
                else
                {
                    StartBackToHome();
                }
            }
        }

        private void StartBackToHome()
        {
            m_chaseState = ChaseState.BACK_TO_HOME;
            m_targetID = 0;
            FindPath(ChaseState.BACK_TO_HOME, m_spawnPoint);
            SendMove(MoveState.BEGIN, Position);
        }

        public override void Spawn()
        {
            m_spawnPoint = Position;
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
