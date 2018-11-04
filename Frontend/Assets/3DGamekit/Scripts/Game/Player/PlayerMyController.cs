using UnityEngine;
using Gamekit3D;
using System.Collections;
using Gamekit3D.Network;
using System.Collections.Generic;
using Common;


namespace Gamekit3D
{
    public class PlayerMyController : MonoBehaviour
    {
        public PlayerController Controller { get { return m_controller; } }
        public NetworkEntity Entity { get { return m_entity; } }

        public LayerMask damagedLayers;
        public static PlayerMyController Instance
        {
            get { return s_Instance; }
        }

        [HideInInspector]
        public bool playerControllerInputBlocked;
        NetworkEntity m_entity;
        PlayerController m_controller;
        int m_attackTarget;

        protected static PlayerMyController s_Instance;
        protected Vector2 m_Movement;
        protected Vector2 m_Camera;
        protected bool m_Jump;
        protected bool m_Attack;
        protected bool m_Pause;
        protected bool m_ExternalInputBlocked;

        public Vector2 MoveInput
        {
            get
            {
                if (playerControllerInputBlocked || m_ExternalInputBlocked)
                    return Vector2.zero;
                return m_Movement;
            }
        }

        public Vector2 CameraInput
        {
            get
            {
                if (playerControllerInputBlocked || m_ExternalInputBlocked)
                    return Vector2.zero;
                return m_Camera;
            }
        }

        public bool IsJumping
        {
            get { return m_Jump && !playerControllerInputBlocked && !m_ExternalInputBlocked; }
        }

        public bool IsAttacking
        {
            get { return m_Attack && !playerControllerInputBlocked && !m_ExternalInputBlocked; }
        }

        public bool IsMoving
        {
            get
            {
                return (m_lastPosition.sqrMagnitude > 0.001
                    && (m_lastPosition - transform.position).sqrMagnitude > 0.0001f)
                  || IsMoveInput;
            }
        }

        public bool IsPause
        {
            get { return m_Pause; }
        }

        public bool IsMoveInput
        {
            get { return !Mathf.Approximately(MoveInput.sqrMagnitude, 0f); }
        }

        protected bool Move
        {
            get { return !Mathf.Approximately(MoveInput.sqrMagnitude, 0f); }
        }

        WaitForSeconds m_AttackInputWait;
        Coroutine m_AttackWaitCoroutine;

        const float k_AttackInputDuration = 0.03f;

        Vector3 m_lastPosition = Vector3.zero;
        void Awake()
        {
            m_AttackInputWait = new WaitForSeconds(k_AttackInputDuration);
            s_Instance = this;
            m_entity = GetComponent<NetworkEntity>();
            m_controller = GetComponent<PlayerController>();
        }


        void Update()
        {
            m_Movement.Set(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            //m_Camera.Set(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            m_Jump = Input.GetButton("Jump");

            if (Input.GetButtonDown("Fire1"))
            {
                if (m_AttackWaitCoroutine != null)
                    StopCoroutine(m_AttackWaitCoroutine);

                m_AttackWaitCoroutine = StartCoroutine(AttackWait());
            }

            m_Pause = Input.GetButtonDown("Pause");

            m_lastPosition = transform.position;
        }

        IEnumerator AttackWait()
        {
            m_Attack = true;

            yield return m_AttackInputWait;

            m_Attack = false;
        }

        public bool HaveControl()
        {
            return !m_ExternalInputBlocked;
        }

        public void ReleaseControl()
        {
            m_ExternalInputBlocked = true;
        }

        public void GainControl()
        {
            m_ExternalInputBlocked = false;
        }

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
            msg.playerId = m_entity.entityId;
            msg.targetId = weaponEntity.entityId;
            MyNetwork.Send(msg);
        }

        public void SendJumpingAction()
        {
            CPlayerJump action = new CPlayerJump();
            action.player = m_entity.entityId;
            MyNetwork.Send(action);
        }

        public void SendAttackingAction()
        {
            CPlayerAttack action = new CPlayerAttack();
            action.player = m_entity.entityId;
            action.target = m_attackTarget;
            MyNetwork.Send(action);
        }

        void InitMove(CPlayerMove action)
        {
            action.player = m_entity.entityId;
            action.move.x = MoveInput.x;
            action.move.y = MoveInput.y;
            action.pos.x = transform.position.x;
            action.pos.y = transform.position.y;
            action.pos.z = transform.position.z;
            action.rot.x = transform.rotation.x;
            action.rot.y = transform.rotation.y;
            action.rot.z = transform.rotation.z;
            action.rot.w = transform.rotation.w;
        }

        public void SendMovingBegin()
        {
            CPlayerMove action = new CPlayerMove();
            action.state = MoveState.BEGIN;
            InitMove(action);
            MyNetwork.Send(action);
        }

        public void SendMovingStep()
        {
            CPlayerMove action = new CPlayerMove();
            action.state = MoveState.STEP;
            InitMove(action);
            MyNetwork.Send(action);
        }

        public void SendMovingEnd()
        {
            CPlayerMove action = new CPlayerMove();
            action.state = MoveState.END;
            InitMove(action);
            MyNetwork.Send(action);
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((damagedLayers.value & 1 << other.gameObject.layer) == 0)
                return;

            NetworkEntity damager = other.gameObject.GetComponent<NetworkEntity>();
            if (damager == null)
                return;

            m_attackTarget = damager.entityId;
        }

        private void OnTriggerExit(Collider other)
        {
            if ((damagedLayers.value & 1 << other.gameObject.layer) == 0)
                return;

            NetworkEntity damager = other.gameObject.GetComponent<NetworkEntity>();
            if (damager == null)
                return;

            if (m_attackTarget == damager.entityId)
                m_attackTarget = 0;
        }
        void OnTriggerStay(Collider other)
        {
        }
    }
}
