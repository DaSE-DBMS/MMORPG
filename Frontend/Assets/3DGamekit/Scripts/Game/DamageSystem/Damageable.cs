﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Gamekit3D;
using UnityEngine.Serialization;

namespace Gamekit3D
{
    public partial class Damageable : MonoBehaviour
    {

        public int maxHitPoints;
        [Tooltip("Time that this gameObject is invulnerable for, after receiving damage.")]
        public int invulnerabiltyTime;

        [Tooltip("The max speed of this object, for backend")]
        public int maxSpeed;

        [Tooltip("The angle from the which that damageable is hitable. Always in the world XZ plane, with the forward being rotate by hitForwardRoation")]
        [Range(0.0f, 360.0f)]
        public float hitAngle = 360.0f;
        [Tooltip("Allow to rotate the world forward vector of the damageable used to define the hitAngle zone")]
        [Range(0.0f, 360.0f)]
        [FormerlySerializedAs("hitForwardRoation")] //SHAME!
        public float hitForwardRotation = 360.0f;

        public bool isInvulnerable { get; set; }
        public int currentHitPoints { get; set; }

        public UnityEvent OnDeath, OnReceiveDamage, OnHitWhileInvulnerable, OnBecomeVulnerable, OnResetDamage;

        [Tooltip("When this gameObject is damaged, these other gameObjects are notified.")]
        [EnforceType(typeof(IMessageReceiver))]
        public List<MonoBehaviour> onDamageMessageReceivers;

        protected float m_timeSinceLastHit = 0.0f;
        protected Collider m_Collider;

        System.Action schedule;

        void Start()
        {
            ResetDamage();
            m_Collider = GetComponent<Collider>();
        }

        void Update()
        {

        }

        public void ResetDamage()
        {
            currentHitPoints = maxHitPoints;
            m_timeSinceLastHit = 0.0f;
            OnResetDamage.Invoke();
        }

        public void SetColliderState(bool enabled)
        {
            m_Collider.enabled = enabled;
        }

        public void ApplyDamage(DamageMessage data)
        {
            if (currentHitPoints <= 0)
            {//ignore damage if already dead. TODO : may have to change that if we want to detect hit on death...
                return;
            }

            Vector3 forward = transform.forward;
            forward = Quaternion.AngleAxis(hitForwardRotation, transform.up) * forward;

            //we project the direction to damager to the plane formed by the direction of damage
            Vector3 positionToDamager = data.damageSource - transform.position;
            positionToDamager -= transform.up * Vector3.Dot(transform.up, positionToDamager);

            if (Vector3.Angle(forward, positionToDamager) > hitAngle * 0.5f)
                return;

            currentHitPoints -= data.amount;

            if (currentHitPoints <= 0)
                schedule += OnDeath.Invoke; //This avoid race condition when objects kill each other.
            else
                OnReceiveDamage.Invoke();

            var messageType = currentHitPoints <= 0 ? MsgType.DEAD : MsgType.DAMAGED;

            for (var i = 0; i < onDamageMessageReceivers.Count; ++i)
            {
                var receiver = onDamageMessageReceivers[i] as IMessageReceiver;
                receiver.OnReceiveMessage(messageType, this, data);
            }
        }

        void LateUpdate()
        {
            if (schedule != null)
            {
                schedule();
                schedule = null;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Vector3 forward = transform.forward;
            forward = Quaternion.AngleAxis(hitForwardRotation, transform.up) * forward;

            if (Event.current.type == EventType.Repaint)
            {
                UnityEditor.Handles.color = Color.blue;
                UnityEditor.Handles.ArrowHandleCap(0, transform.position, Quaternion.LookRotation(forward), 1.0f,
                    EventType.Repaint);
            }


            UnityEditor.Handles.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
            forward = Quaternion.AngleAxis(-hitAngle * 0.5f, transform.up) * forward;
            UnityEditor.Handles.DrawSolidArc(transform.position, transform.up, forward, hitAngle, 1.0f);
        }
#endif
    }

}
