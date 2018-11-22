using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;
using Gamekit3D.Network;
using Common;

namespace Gamekit3D
{
    //this assure it's runned before any behaviour that may use it, as the animator need to be fecthed
    [DefaultExecutionOrder(-1)]
    public class EnemyController : MonoBehaviour, ICreatureBehavior, ISpriteBehavior
    {
        public bool interpolateTurning = false;
        public bool applyAnimationRotation = false;
        public LayerMask ColliderLayer;

        struct MoveMessage
        {
            public MsgType message;
            public Vector3 position;
        }

        public Animator animator { get { return m_Animator; } }
        public Vector3 externalForce { get { return m_ExternalForce; } }
        public ICreatureBehavior target { get { return m_target; } }
        //public NavMeshAgent navmeshAgent { get { return m_NavMeshAgent; } }
        //public bool followNavmeshAgent { get { return m_FollowNavmeshAgent; } }
        public bool grounded { get { return m_Grounded; } }
        public float maxSpeed = 0.3f;
        public float acceleration = 20.0f;
        //protected NavMeshAgent m_NavMeshAgent;
        //protected bool m_FollowNavmeshAgent;
        protected Animator m_Animator;
        protected bool m_UnderExternalForce;
        protected bool m_ExternalForceAddGravity = true;
        protected Vector3 m_ExternalForce;
        protected bool m_Grounded;

        protected Rigidbody m_Rigidbody;

        const float k_GroundedRayDistance = .8f;

        private NetworkEntity m_entity;
        private ICreatureBehavior m_target;
        private float m_desiredSpeed;
        private float m_currentSpeed;
        private Damageable m_damageable;

        private Queue<MoveMessage> m_moveMessage = new Queue<MoveMessage>();
        private IMessageReceiver m_receiver;
        private Vector3 m_destination = Vector3.zero;
        void Awake()
        {
            m_entity = GetComponent<NetworkEntity>();
            m_damageable = GetComponent<Damageable>();
            m_entity.behavior = this;
            m_desiredSpeed = 0;
        }

        void OnEnable()
        {
            //m_NavMeshAgent = GetComponent<NavMeshAgent>();
            m_Animator = GetComponent<Animator>();
            m_Animator.updateMode = AnimatorUpdateMode.AnimatePhysics;

            //m_NavMeshAgent.updatePosition = false;

            m_Rigidbody = GetComponentInChildren<Rigidbody>();
            if (m_Rigidbody == null)
                m_Rigidbody = gameObject.AddComponent<Rigidbody>();

            m_Rigidbody.isKinematic = true;
            m_Rigidbody.useGravity = false;
            m_Rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            m_Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

            //m_FollowNavmeshAgent = true;
        }

        private void FixedUpdate()
        {

            //animator.speed = PlayerInput.Instance != null && PlayerInput.Instance.HaveControl() ? 1.0f : 0.0f;

            CheckGrounded();

            if (m_UnderExternalForce)
                ForceMovement();
            else
                UpdateMovement();
        }

        void CheckGrounded()
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position + Vector3.up * k_GroundedRayDistance * 0.5f, -Vector3.up);
            m_Grounded = Physics.Raycast(ray, out hit, k_GroundedRayDistance, Physics.AllLayers,
                QueryTriggerInteraction.Ignore);
        }

        void ForceMovement()
        {
            if (m_ExternalForceAddGravity)
                m_ExternalForce += Physics.gravity * Time.deltaTime;

            RaycastHit hit;
            Vector3 movement = m_ExternalForce * Time.deltaTime;
            if (!m_Rigidbody.SweepTest(movement.normalized, out hit, movement.sqrMagnitude))
            {
                m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
            }

            //m_NavMeshAgent.Warp(m_Rigidbody.position);
        }

        // used to disable position being set by the navmesh agent, for case where we want the animation to move the enemy instead (e.g. Chomper attack)
        public void SetFollowNavmeshAgent(bool follow)
        {
            //if (!follow && m_NavMeshAgent.enabled)
            //{
            //    m_NavMeshAgent.ResetPath();
            //}
            //else if (follow && !m_NavMeshAgent.enabled)
            //{
            //    m_NavMeshAgent.Warp(transform.position);
            //}

            //m_FollowNavmeshAgent = follow;
            //m_NavMeshAgent.enabled = follow;
        }

        public void AddForce(Vector3 force, bool useGravity = true)
        {
            //if (m_NavMeshAgent.enabled)
            //    m_NavMeshAgent.ResetPath();

            m_ExternalForce = force;
            //m_NavMeshAgent.enabled = false;
            m_UnderExternalForce = true;
            m_ExternalForceAddGravity = useGravity;
        }

        public void ClearForce()
        {
            m_UnderExternalForce = false;
            //m_NavMeshAgent.enabled = true;
        }

        public void SetForward(Vector3 forward)
        {
            Quaternion targetRotation = Quaternion.LookRotation(forward);

            if (interpolateTurning)
            {
                //targetRotation = Quaternion.RotateTowards(transform.rotation, targetRotation,
                //    m_NavMeshAgent.angularSpeed * Time.deltaTime);
            }

            transform.rotation = targetRotation;
        }

        public void SetTarget(Vector3 position)
        {
            //m_NavMeshAgent.destination = position;
        }
        void PositionRevise()
        {
            if (m_target != null && m_target is PlayerController)
            {
                PlayerController controller = (PlayerController)(m_target);
                if (controller.IsMine)
                {
                    CPositionRevise msg = new CPositionRevise();
                    msg.entityId = m_entity.EntityId;
                    msg.pos.x = transform.position.x;
                    msg.pos.y = transform.position.y;
                    msg.pos.z = transform.position.z;
                    MyNetwork.Send(msg);
                }
            }
        }

        void UpdateMovement()
        {
            if (m_moveMessage.Count == 0)
            {
                m_desiredSpeed = 0;
                return;
            }

            // Adjust the forward speed towards the desired speed.
            m_desiredSpeed = maxSpeed;
            m_currentSpeed = Mathf.MoveTowards(m_currentSpeed, m_desiredSpeed, acceleration * Time.deltaTime);
            MoveMessage step = m_moveMessage.Peek();
            Vector3 position = step.position;
            float distance = Vector3.Distance(transform.position, position);
            if (distance > 3.0f)
            { // if the distance between current position and next position are not close
                transform.LookAt(position);
            }

            float t = (distance < float.Epsilon) ? 1.0f : (m_currentSpeed + 0.1f) * Time.deltaTime / distance;
            // cannot greater than 1
            t = t > 1.0f ? 1.0f : t;

            position = Vector3.Lerp(transform.position, position, t);

            if (step.message == MsgType.MOVE)
            {
                // prject this position on to ground
                Vector3 hit = HitGround(position);
                if ((hit - transform.position).sqrMagnitude > 0.1f)
                { // not too close, prevent dead loop
                    position = hit;
                }
            }

            DebugUtil.DrawLine(transform.position, position, Color.red);
            transform.position = position;

            // send to server to revise the sprite position
            PositionRevise();


            if (step.message == MsgType.END_BACK ||
                step.message == MsgType.BEGIN_BACK)
            {
                // end chase enemy
                m_target = null;
            }
            if (Mathf.Abs(1.0f - t) < 0.001)
            { // is nearly begin or end position
                m_receiver.OnReceiveMessage(step.message, this, step.position);
                m_moveMessage.Dequeue();
                if (step.message == MsgType.END_CHASE)
                {
                    DebugUtil.DrawLine(transform.position, transform.position + Vector3.up, Color.green);
                }
            }
        }

        Vector3 HitGround(Vector3 position)
        {
            RaycastHit hit;
            Vector3 p = position + Vector3.up * k_GroundedRayDistance * 0.5f;
            Ray down = new Ray(p, -Vector3.up);
            Ray up = new Ray(p, Vector3.up);
            if (Physics.Raycast(down, out hit, k_GroundedRayDistance, Physics.AllLayers,
                QueryTriggerInteraction.Ignore))
            {
                return hit.point;
            }
            else if (Physics.Raycast(up, out hit, k_GroundedRayDistance, Physics.AllLayers,
              QueryTriggerInteraction.Ignore))
            {
                return hit.point;
            }
            return position;
        }
        // set message receiver
        // receiver may be ChomperBehavior/GrenadierBehaviour/SpitterBehaviour
        public void SetReceiver(IMessageReceiver receiver)
        {
            m_receiver = receiver;
        }

        public void Jump()
        {
            //can't jump
        }

        public void ReSpawn(int hp, Vector3 postion, Quaternion rotation)
        {
            m_damageable.currentHitPoints = hp;
            transform.position = postion;
            transform.rotation = rotation;
        }

        public void Attack(ICreatureBehavior target)
        {
            m_receiver.OnReceiveMessage(MsgType.ATTACK, this, target);
        }

        public void BeginChase(Vector3 position, ICreatureBehavior target)
        {
            m_target = target;
            m_moveMessage.Clear();
            m_desiredSpeed = maxSpeed;
            MoveMessage step;
            step.position = position;
            step.message = MsgType.BEGIN_CHASE;
            m_moveMessage.Enqueue(step);
        }

        public void EndChase(Vector3 position)
        {
            MoveMessage step;
            step.position = position;
            step.message = MsgType.END_CHASE;
            m_moveMessage.Enqueue(step);
        }

        public void BeginBack(Vector3 position)
        {
            m_moveMessage.Clear();
            m_desiredSpeed = maxSpeed;
            MoveMessage step;
            step.position = position;
            step.message = MsgType.BEGIN_BACK;
            m_moveMessage.Enqueue(step);
        }

        public void EndBack(Vector3 position)
        {
            //DebugUtil.DrawLine(position, position + Vector3.up, Color.red);
            MoveMessage step;
            step.position = position;
            step.message = MsgType.END_BACK;
            m_moveMessage.Enqueue(step);
        }

        public void MoveStep(Vector3 position)
        {
            MoveMessage step;
            step.position = position;
            step.message = MsgType.MOVE;
            m_moveMessage.Enqueue(step);
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public void BeHit(int decHP, ICreatureBehavior source)
        {
            var msg = new Damageable.DamageMessage()
            {
                amount = decHP,
                damager = this,
                direction = Vector3.up,
                stopCamera = false
            };

            m_damageable.ApplyDamage(msg);
        }

        public void Die()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if ((ColliderLayer.value & 1 << other.gameObject.layer) == 0)
                return;

            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            if (player == null || !player.IsMine)
                return;

            CEnemyClosing msg = new CEnemyClosing();
            msg.entityId = m_entity.entityId;
            msg.enemyId = player.Entity.entityId;
            MyNetwork.Send(msg);
        }

        private void OnTriggerExit(Collider other)
        {
            if ((ColliderLayer.value & 1 << other.gameObject.layer) == 0)
                return;

            NetworkEntity enemy = other.gameObject.GetComponent<NetworkEntity>();
            if (enemy == null)
                return;
        }
    }
}
