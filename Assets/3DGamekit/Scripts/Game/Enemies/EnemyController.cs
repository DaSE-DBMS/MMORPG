using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;
using Gamekit3D.Network;
using Common.Data;

namespace Gamekit3D
{
    //this assure it's runned before any behaviour that may use it, as the animator need to be fecthed
    [DefaultExecutionOrder(-1)]
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyController : MonoBehaviour, ICreatureBehavior
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
        private Queue<Vector3> m_moveSteps = new Queue<Vector3>();
        private Vector3 m_lastPosition = Vector3.forward;
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
        }

        private void FixedUpdate()
        {
            UpdateMovement();
        }

        void UpdateMovement()
        {
            if (m_moveSteps.Count > 0)
            {
                // Adjust the forward speed towards the desired speed.
                m_currentSpeed = Mathf.MoveTowards(m_currentSpeed, m_desiredSpeed, acceleration * Time.deltaTime);

                Vector3 position = m_moveSteps.Peek();

                float distance = Vector3.Distance(transform.position, position);
                if (distance < double.Epsilon)
                {
                    m_moveSteps.Dequeue();
                    return;
                }

                float t = (m_currentSpeed + 0.1f) * Time.deltaTime / distance;
                if (Mathf.Abs(t - 1.0f) < 0.1 ||
                    t < 0.1)
                {
                    m_moveSteps.Dequeue();
                }
                position = Vector3.Lerp(m_lastPosition, position, t);
                m_lastPosition = position;

                RaycastHit hit;
                Ray down = new Ray(position + Vector3.up * k_GroundedRayDistance * 0.5f, -Vector3.up);
                Ray up = new Ray(position + Vector3.up * k_GroundedRayDistance * 0.5f, Vector3.up);
                if (Physics.Raycast(down, out hit, k_GroundedRayDistance, Physics.AllLayers,
                    QueryTriggerInteraction.Ignore))
                {
                    position = hit.point;
                }
                else if (Physics.Raycast(up, out hit, k_GroundedRayDistance, Physics.AllLayers,
                  QueryTriggerInteraction.Ignore))
                {
                    position = hit.point;
                }

                transform.position = position;
            }
            else
            {
                m_desiredSpeed = 0;
            }
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
        }

        public void MoveBegin(V3 start,
            V3 end,
            V4 rot)
        {
            m_moveSteps.Clear();
            Vector3 s = new Vector3(start.x, start.y, start.z);
            m_moveSteps.Enqueue(s);
            m_desiredSpeed = maxSpeed;
            transform.LookAt(s);
        }

        public void MoveStep(
            V3 start,
            V3 pos,
            V4 rot)
        {
            Vector3 s = new Vector3(start.x, start.y, start.z);
            m_moveSteps.Enqueue(s);
            m_desiredSpeed = maxSpeed;
            transform.LookAt(s);
        }

        public void MoveEnd(
            V3 move,
            V3 start,
            V4 rot)
        {
            Vector3 s = new Vector3(start.x, start.y, start.z);
            transform.position = s;
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
