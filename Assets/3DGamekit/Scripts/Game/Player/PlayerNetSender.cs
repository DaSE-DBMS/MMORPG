using UnityEngine;
using Gamekit3D;
using System.Collections;
using Gamekit3D.Network;
using System.Collections.Generic;
using Common;
using Common.Data;

namespace Gamekit3D
{
    public class PlayerNetSender : MonoBehaviour
    {
        public LayerMask damagedLayers;

        NetworkEntity m_entity;
        PlayerController m_controller;
        PlayerInput m_input;
        int m_moveStep = 0;
        int m_attackTarget;

        public void PlayerWantTakeWeapon(GameObject weapon)
        {
            if (m_controller.canAttack)
                return;

            NetworkEntity weaponEntity = weapon.GetComponent<NetworkEntity>();
            if (weaponEntity == null)
                return;

            CPlayerTake msg = new CPlayerTake();
            msg.byName = weaponEntity.canClone;
            msg.targetName = weapon.name;
            msg.playerId = m_entity.entityID;
            msg.targetId = weaponEntity.entityID;
            MyNetwork.instance.Send(msg);
        }

        private void Awake()
        {
            m_entity = GetComponent<NetworkEntity>();
            m_controller = GetComponent<PlayerController>();
            m_input = GetComponent<PlayerInput>();
        }

        private void FixedUpdate()
        {
            TestActionSending();
        }

        private void TestActionSending()
        {
            bool m_moving = m_input.IsMoveInput;
            if (m_moving && m_moveStep == 0)
            {
                SendMovingBegin();
            }
            else if (m_moving && m_moveStep > 0)
            {
                SendMovingStep();
            }
            else if (!m_moving && m_moveStep != 0)
            {
                SendMovingEnd();
                m_moveStep = 0;
            }

            if (m_controller.CanAttack && m_input.IsAttackInput)
            {
                SendAttackingAction(m_attackTarget);
            }
            else
            {
                //m_attackTarget = 0;
            }
            if (m_controller.IsReadyToJump && m_input.IsJumpInput)
            {
                SendJumpingAction();
            }
        }

        void SendJumpingAction()
        {
            CPlayerJump action = new CPlayerJump();
            action.player = m_entity.entityID;
            MyNetwork.instance.Send(action);
        }

        void SendAttackingAction(int targetID = 0)
        {
            CPlayerAttack action = new CPlayerAttack();
            action.player = m_entity.entityID;
            action.target = targetID;
            MyNetwork.instance.Send(action);
        }

        void InitMove(CPlayerMove action)
        {
            action.player = m_entity.entityID;
            action.move.x = m_input.MoveInput.x;
            action.move.y = m_input.MoveInput.y;
            action.pos.x = transform.position.x;
            action.pos.y = transform.position.y;
            action.pos.z = transform.position.z;
            action.rot.x = transform.rotation.x;
            action.rot.y = transform.rotation.y;
            action.rot.z = transform.rotation.z;
            action.rot.w = transform.rotation.w;
        }

        void SendMovingBegin()
        {
            CPlayerMove action = new CPlayerMove();
            action.state = MoveState.BEGIN;
            InitMove(action);
            MyNetwork.instance.Send(action);
            m_moveStep++;
        }

        void SendMovingStep()
        {
            CPlayerMove action = new CPlayerMove();
            action.state = MoveState.STEP;
            InitMove(action);
            MyNetwork.instance.Send(action);
            m_moveStep++;
        }

        void SendMovingEnd()
        {
            CPlayerMove action = new CPlayerMove();
            action.state = MoveState.END;
            InitMove(action);
            MyNetwork.instance.Send(action);
            m_moveStep = 0;
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((damagedLayers.value & 1 << other.gameObject.layer) == 0)
                return;

            NetworkEntity damager = other.gameObject.GetComponent<NetworkEntity>();
            if (damager == null)
                return;

            m_attackTarget = damager.entityID;
        }

        private void OnTriggerExit(Collider other)
        {
            if ((damagedLayers.value & 1 << other.gameObject.layer) == 0)
                return;

            NetworkEntity damager = other.gameObject.GetComponent<NetworkEntity>();
            if (damager == null)
                return;

            if (m_attackTarget == damager.entityID)
                m_attackTarget = 0;
        }
        void OnTriggerStay(Collider other)
        {
        }
    }
}
