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

        public Animator animator { get { return m_Animator; } }
        public bool grounded { get { return m_Grounded; } }

        public float maxSpeed = 0.3f;
        public float acceleration = 20.0f;
        protected Animator m_Animator;
        protected bool m_UnderExternalForce;
        protected bool m_ExternalForceAddGravity = true;
        protected Vector3 m_ExternalForce;
        protected bool m_Grounded;

        protected Rigidbody m_Rigidbody;

        const float k_GroundedRayDistance = .8f;

        private NetworkEntity m_entity;
        private float m_desiredSpeed;
        private float m_currentSpeed;
        struct MoveMessage
        {
            public MsgType message;
            public Vector3 position;
        }
        private Queue<MoveMessage> m_moveMessage = new Queue<MoveMessage>();
        private IMessageReceiver m_receiver;
        private int m_targetId = 0;
        private Vector3 m_destination = Vector3.zero;
        void Awake()
        {
            m_entity = GetComponent<NetworkEntity>();
            m_entity.creatureBehavior = this;
            m_desiredSpeed = 0;
        }

        void OnEnable()
        {
            m_Animator = GetComponent<Animator>();
            m_Animator.updateMode = AnimatorUpdateMode.AnimatePhysics;

            m_Rigidbody = GetComponentInChildren<Rigidbody>();
            if (m_Rigidbody == null)
                m_Rigidbody = gameObject.AddComponent<Rigidbody>();

            m_Rigidbody.isKinematic = true;
            m_Rigidbody.useGravity = false;
            m_Rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            m_Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            m_destination = transform.position;
        }

        private void FixedUpdate()
        {
            UpdateMovement();
        }
        void PositionRevise()
        {
            if (m_targetId != 0 && m_targetId == PlayerMyController.Instance.Entity.entityId)
            {
                CPositionRevise msg = new CPositionRevise();
                msg.entityId = m_entity.EntityId;
                msg.pos.x = transform.position.x;
                msg.pos.y = transform.position.y;
                msg.pos.z = transform.position.z;
                MyNetwork.Send(msg);
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
            if (distance < double.Epsilon)
            {
                m_moveMessage.Dequeue();
            }
            else
            {
                if (distance > 3.0f)
                { // if the distance between current position and next position are not close
                    transform.LookAt(position);
                }
                float t = (m_currentSpeed + 0.1f) * Time.deltaTime / distance;
                // cannot greater than 1
                t = t > 1.0f ? 1.0f : t;
                if (Mathf.Abs(t - 1.0f) < 0.1)
                { // is nearly next step
                    m_moveMessage.Dequeue();
                }
                position = Vector3.Lerp(transform.position, position, t);
            }

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

            m_receiver.OnReceiveMessage(step.message, this, position);
            if (step.message == MsgType.END_BACK ||
                step.message == MsgType.BEGIN_BACK)
            {
                // end chase enemy
                m_targetId = 0;
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
        // used to disable position being set by the navmesh agent, for case where we want the animation to move the enemy instead (e.g. Chomper attack)
        public void SetFollowNavmeshAgent(bool follow)
        {

        }


        public void SetTarget(Vector3 position)
        {
            //m_NavMeshAgent.destination = position;
        }


        public void Jump()
        {
            //can't jump
        }

        public void Attack(ICreatureBehavior target)
        {
            m_receiver.OnReceiveMessage(MsgType.ATTACK, this, target);
        }

        public void BeginChase(Vector3 position, int targetId)
        {
            m_targetId = targetId;
            m_moveMessage.Clear();
            m_desiredSpeed = maxSpeed;
            MoveMessage step;
            step.position = position;
            step.message = MsgType.BEGIN_CHASE;
            m_moveMessage.Enqueue(step);
            //DebugUtil.DrawLine(position, position + Vector3.up, Color.red);
            Debug.Log("BeginChase, position=" + position.ToString() + ", target=" + targetId.ToString());
        }

        public void EndChase(Vector3 position, int targetId)
        {
            m_targetId = targetId;
            //DebugUtil.DrawLine(position, position + Vector3.up, Color.red);
            MoveMessage step;
            step.position = position;
            step.message = MsgType.END_CHASE;
            m_moveMessage.Enqueue(step);
            Debug.Log("EndChase, position=" + position.ToString() + ", target=" + targetId.ToString());
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

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public void UnderAttack(int HP, ICreatureBehavior source)
        {

        }

        public void Die()
        {

        }
    }
}
