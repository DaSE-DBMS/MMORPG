using Gamekit3D;
using Gamekit3D.Network;
using Common.Data;
using UnityEngine;


namespace Gamekit3D
{
    [DefaultExecutionOrder(100)]
    public class ChomperBehavior : MonoBehaviour, IMessageReceiver, ICreatureBehavior
    {
        public static readonly int hashInPursuit = Animator.StringToHash("InPursuit");
        public static readonly int hashAttack = Animator.StringToHash("Attack");
        public static readonly int hashHit = Animator.StringToHash("Hit");
        public static readonly int hashVerticalDot = Animator.StringToHash("VerticalHitDot");
        public static readonly int hashHorizontalDot = Animator.StringToHash("HorizontalHitDot");
        public static readonly int hashThrown = Animator.StringToHash("Thrown");
        public static readonly int hashGrounded = Animator.StringToHash("Grounded");
        public static readonly int hashVerticalVelocity = Animator.StringToHash("VerticalVelocity");
        public static readonly int hashSpotted = Animator.StringToHash("Spotted");
        public static readonly int hashNearBase = Animator.StringToHash("NearBase");

        public static readonly int hashIdleState = Animator.StringToHash("ChomperIdle");

        public EnemyController controller { get { return m_Controller; } }

        public PlayerController target { get { return m_Target; } }
        public TargetDistributor.TargetFollower followerData { get { return m_FollowerInstance; } }

        public Vector3 originalPosition { get; protected set; }
        [System.NonSerialized]
        public float attackDistance = 3;

        public MeleeWeapon meleeWeapon;
        public TargetScanner playerScanner;
        [Tooltip("Time in seconde before the Chomper stop pursuing the player when the player is out of sight")]
        public float timeToStopPursuit;

        [Header("Audio")]
        public RandomAudioPlayer attackAudio;
        public RandomAudioPlayer frontStepAudio;
        public RandomAudioPlayer backStepAudio;
        public RandomAudioPlayer hitAudio;
        public RandomAudioPlayer gruntAudio;
        public RandomAudioPlayer deathAudio;
        public RandomAudioPlayer spottedAudio;

        protected float m_TimerSinceLostTarget = 0.0f;

        protected PlayerController m_Target = null;
        protected EnemyController m_Controller;
        protected TargetDistributor.TargetFollower m_FollowerInstance = null;

        private Animator m_Animator;
        private bool m_ground = true;
        protected void OnEnable()
        {
            //m_Controller = GetComponentInChildren<EnemyController>();
            m_Animator = GetComponent<Animator>();
            originalPosition = transform.position;

            meleeWeapon.SetOwner(gameObject);

            m_Animator.Play(hashIdleState, 0, Random.value);

            SceneLinkedSMB<ChomperBehavior>.Initialise(m_Animator, this);
        }

        /// <summary>
        /// Called by animation events.
        /// </summary>
        /// <param name="frontFoot">Has a value of 1 when it's a front foot stepping and 0 when it's a back foot.</param>
        void PlayStep(int frontFoot)
        {
            if (frontStepAudio != null && frontFoot == 1)
                frontStepAudio.PlayRandomClip();
            else if (backStepAudio != null && frontFoot == 0)
                backStepAudio.PlayRandomClip();
        }

        /// <summary>
        /// Called by animation events.
        /// </summary>
        public void Grunt()
        {
            if (gruntAudio != null)
                gruntAudio.PlayRandomClip();
        }

        public void Spotted()
        {
            if (spottedAudio != null)
                spottedAudio.PlayRandomClip();
        }

        protected void OnDisable()
        {
            if (m_FollowerInstance != null)
                m_FollowerInstance.distributor.UnregisterFollower(m_FollowerInstance);
        }

        private void FixedUpdate()
        {
            m_Animator.SetBool(hashGrounded, m_ground);

            Vector3 toBase = originalPosition - transform.position;
            toBase.y = 0;

            m_Animator.SetBool(hashNearBase, toBase.sqrMagnitude < 0.1 * 0.1f);
        }


        public void FindTarget()
        {/*

            //we ignore height difference if the target was already seen
            PlayerController target = playerScanner.Detect(transform, m_Target == null);

            if (m_Target == null)
            {
                //we just saw the player for the first time, pick an empty spot to target around them
                if (target != null)
                {
                    m_Controller.animator.SetTrigger(hashSpotted);
                    m_Target = target;
                    TargetDistributor distributor = target.GetComponentInChildren<TargetDistributor>();
                    if (distributor != null)
                        m_FollowerInstance = distributor.RegisterNewFollower();
                }
            }
            else
            {
                //we lost the target. But chomper have a special behaviour : they only loose the player scent if they move past their detection range
                //and they didn't see the player for a given time. Not if they move out of their detectionAngle. So we check that this is the case before removing the target
                if (target == null)
                {
                    m_TimerSinceLostTarget += Time.deltaTime;

                    if (m_TimerSinceLostTarget >= timeToStopPursuit)
                    {
                        Vector3 toTarget = m_Target.transform.position - transform.position;

                        if (toTarget.sqrMagnitude > playerScanner.detectionRadius * playerScanner.detectionRadius)
                        {
                            if (m_FollowerInstance != null)
                                m_FollowerInstance.distributor.UnregisterFollower(m_FollowerInstance);

                            //the target move out of range, reset the target
                            m_Target = null;
                        }
                    }
                }
                else
                {
                    if (target != m_Target)
                    {
                        if (m_FollowerInstance != null)
                            m_FollowerInstance.distributor.UnregisterFollower(m_FollowerInstance);

                        m_Target = target;

                        TargetDistributor distributor = target.GetComponentInChildren<TargetDistributor>();
                        if (distributor != null)
                            m_FollowerInstance = distributor.RegisterNewFollower();
                    }

                    m_TimerSinceLostTarget = 0.0f;
                }
            }
        */
        }

        public void StartPursuit()
        {
            if (m_FollowerInstance != null)
            {
                m_FollowerInstance.requireSlot = true;
                RequestTargetPosition();
            }

            m_Animator.SetBool(hashInPursuit, true);
        }

        public void StopPursuit()
        {
            if (m_FollowerInstance != null)
            {
                m_FollowerInstance.requireSlot = false;
            }

            m_Animator.SetBool(hashInPursuit, false);
        }

        public void RequestTargetPosition()
        {
            /*
            Vector3 fromTarget = transform.position - m_Target.transform.position;
            fromTarget.y = 0;

            m_FollowerInstance.requiredPoint = m_Target.transform.position + fromTarget.normalized * attackDistance * 0.9f;
            */
        }

        public void WalkBackToBase()
        {
            if (m_FollowerInstance != null)
                m_FollowerInstance.distributor.UnregisterFollower(m_FollowerInstance);
            //m_Target = null;
            //StopPursuit();
            //m_Controller.SetTarget(originalPosition);
            //m_Controller.SetFollowNavmeshAgent(true);
        }

        public void TriggerAttack()
        {
            m_Animator.SetTrigger(hashAttack);
        }

        public void AttackBegin()
        {
            meleeWeapon.BeginAttack(false);
        }

        public void AttackEnd()
        {
            meleeWeapon.EndAttack();
        }

        public void OnReceiveMessage(DamageMsgType type, object sender, object msg)
        {
            switch (type)
            {
                case DamageMsgType.DEAD:
                    Death((Damageable.DamageMessage)msg);
                    break;
                case DamageMsgType.DAMAGED:
                    ApplyDamage((Damageable.DamageMessage)msg);
                    break;
                default:
                    break;
            }
        }

        public void Death(Damageable.DamageMessage msg)
        {
            Vector3 pushForce = transform.position - msg.damageSource;

            pushForce.y = 0;

            transform.forward = -pushForce.normalized;
            //controller.AddForce(pushForce.normalized * 7.0f - Physics.gravity * 0.6f);

            m_Animator.SetTrigger(hashHit);
            m_Animator.SetTrigger(hashThrown);

            //We unparent the hit source, as it would destroy it with the gameobject when it get replaced by the ragdol otherwise
            deathAudio.transform.SetParent(null, true);
            deathAudio.PlayRandomClip();
            GameObject.Destroy(deathAudio, deathAudio.clip == null ? 0.0f : deathAudio.clip.length + 0.5f);
        }

        public void ApplyDamage(Damageable.DamageMessage msg)
        {
            //TODO : make that more generic, (e.g. move it to the MeleeWeapon code with a boolean to enable shaking of camera on hit?)
            if (msg.damager.name == "Staff")
                CameraShake.Shake(0.06f, 0.1f);

            float verticalDot = Vector3.Dot(Vector3.up, msg.direction);
            float horizontalDot = Vector3.Dot(transform.right, msg.direction);

            Vector3 pushForce = transform.position - msg.damageSource;

            pushForce.y = 0;

            transform.forward = -pushForce.normalized;
            //controller.AddForce(pushForce.normalized * 5.5f, false);

            m_Animator.SetFloat(hashVerticalDot, verticalDot);
            m_Animator.SetFloat(hashHorizontalDot, horizontalDot);

            m_Animator.SetTrigger(hashHit);

            hitAudio.PlayRandomClip();
        }

        public void ReHited(int HP, ICreatureBehavior source)
        {

            Vector3 direction = source.GetPosition() - transform.position;
            float verticalDot = Vector3.Dot(Vector3.up, direction);
            float horizontalDot = Vector3.Dot(transform.right, direction);

            Vector3 pushForce = transform.position - source.GetPosition();

            pushForce.y = 0;

            transform.forward = -pushForce.normalized;
            //controller.AddForce(pushForce.normalized * 5.5f, false);

            m_Animator.SetFloat(hashVerticalDot, verticalDot);
            m_Animator.SetFloat(hashHorizontalDot, horizontalDot);

            m_Animator.SetTrigger(hashHit);

            hitAudio.PlayRandomClip();
        }

        public void ActionJump()
        {
            // Chomper can't jump
        }

        public void ActionAttack(ICreatureBehavior target)
        {
        }

        public void ActionMoveBegin(V2 move,
            V3 pos,
            V4 rot)
        {
            //m_moving = true;
            //m_movement.Set(0f, 0f);
        }

        public void ActionMoveStep(
            V2 move,
            V3 pos,
            V4 rot)
        {
            //m_moving = true;
            //m_movement.Set(move.x, move.y);
            Vector3 position = new Vector3(pos.x, pos.y, pos.z);
            Quaternion rotation = new Quaternion(rot.x, rot.y, rot.z, rot.w);
            transform.position = position;
            transform.rotation = rotation;
        }

        public void ActionMoveEnd(V2 move,
            V3 pos,
            V4 rot)
        {
            Vector3 position = new Vector3(pos.x, pos.y, pos.z);
            transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * 10.0f);
            //m_movement.Set(0f, 0f);
            //m_moving = false;
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            playerScanner.EditorGizmo(transform);
        }
#endif
    }
}
