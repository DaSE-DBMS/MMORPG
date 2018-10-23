using System;
using System.Collections.Generic;
using Common;
using GeometRi;
namespace Backend.Game
{
    public class Sprite : Creature
    {
        enum SearchEnemyState
        {
            IDLE,
            CHASING_ENEMY,
            BACK_TO_HOME
        }

        // for sprite search path
        private SearchEnemyState m_searchEnemyState = SearchEnemyState.IDLE;
        private int m_targetID;
        private LinkedList<Point3d> m_routeSteps = new LinkedList<Point3d>();
        // target position when I find my path to it last time
        private Point3d m_targetPos = new Point3d();
        private Point3d m_spawnPoint = new Point3d();
        private Point3d m_nextPos = new Point3d();
        DateTime m_lastMoveTS = DateTime.UnixEpoch;

        const float DistanceEpsilon = 0.1f;
        const float LongDistance = 100;


        public void EnemyNear(Creature creature)
        {
            if (Position.DistanceTo(creature.Position) > 3.0)
            {
                return;
            }
            m_targetPos = creature.Position;
            m_targetID = creature.entityID;
            FindPath(creature, m_routeSteps);
            m_searchEnemyState = SearchEnemyState.CHASING_ENEMY;
            m_nextPos = this.Position;
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
            SearchEnemy();
        }
        private void SearchEnemy()
        {
            if (m_searchEnemyState == SearchEnemyState.IDLE)
            {
                //UpdateActive = false;
                return;
            }
            else if (m_searchEnemyState == SearchEnemyState.CHASING_ENEMY)
            {
                this.Position = m_nextPos;
                Creature target = (Creature)World.Instance().GetEntity(m_targetID);
                Point3d targetPos = target.Position;
                float distance = (float)Position.DistanceTo(targetPos);
                if (distance > LongDistance)
                {
                    // too far away, I cannot catch up my enemy, so I give up
                    StartBackToSpawnPoint();
                    return;
                }
                else if (distance < DistanceEpsilon)
                {
                    // reach the destination
                    SendActionMove(MoveState.END, this.Position, this.Position);
                    return;
                }

                if (targetPos.DistanceTo(m_targetPos) > DistanceEpsilon)
                {
                    // the target is moving
                    //the route I found last time was behind the time...
                    ReFindPath(m_searchEnemyState, targetPos);
                    return;
                }
            }

            /*m_nextPos = m_routeSteps.First.Value;
            double walkDis = (double)speed * (double)(DateTime.Now - m_lastMoveTS).TotalMilliseconds / 1000.0f;
            if (walkDis < Position.DistanceTo(m_nextPos))
            {
                // I had walked some steps last frames,
                // but my speed was too slow to walk such a long way in one frame time
                //return;
            }*/

            if (m_routeSteps.Count == 0)
            {
                Point3d position = m_searchEnemyState == SearchEnemyState.BACK_TO_HOME ?
                    m_spawnPoint : this.Position;
                SendActionMove(MoveState.END, position, position);
            }
            else
            {
                m_nextPos = m_routeSteps.First.Value;
                m_routeSteps.RemoveFirst();
                SendActionMove(MoveState.STEP, this.Position, m_nextPos);
            }
        }

        private void ReFindPath(SearchEnemyState state, Point3d target)
        {
            if (!FindPath(target, m_routeSteps))
            {
                // cannot find a way , something was wrong ???
                // fly to spawn point
                if (state == SearchEnemyState.BACK_TO_HOME)
                {
                    // cannot find a way , something was wrong ???
                    // fly to spawn point
                    SendActionMove(MoveState.END, this.Position, this.Position);
                }
                else
                {
                    StartBackToSpawnPoint();
                }
                return;
            }
            else
            {
                m_targetPos = target;
                m_nextPos = m_routeSteps.First.Value;
                m_routeSteps.RemoveFirst();
                SendActionMove(MoveState.BEGIN, this.Position, m_nextPos);
            }
        }

        private void StartBackToSpawnPoint()
        {
            m_searchEnemyState = SearchEnemyState.BACK_TO_HOME;
            m_targetID = 0;
            ReFindPath(SearchEnemyState.BACK_TO_HOME, m_spawnPoint);
        }

        public override void Spawn()
        {
            m_spawnPoint = Position;
        }

        private void SendActionMove(MoveState state, Point3d movement, Point3d position)
        {
            if (state == MoveState.END)
            {
                m_routeSteps.Clear();
                // attack enemy if current search enemy state is CHASING_ENEMY...
                if (m_searchEnemyState == SearchEnemyState.BACK_TO_HOME)
                {
                    m_searchEnemyState = SearchEnemyState.IDLE;
                }
            }
            m_lastMoveTS = DateTime.Now;
            SMove message = new SMove();
            message.ID = entityID;
            message.move = Entity.Point3dToV3(movement);
            message.pos = Entity.Point3dToV3(position);
            message.state = state;
            Broadcast(message);
        }
    }
}
