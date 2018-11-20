using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using Gamekit3D.Network;

namespace Gamekit3D
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class DamageZone : MonoBehaviour
    {
        public int damageAmount = 1;
        public bool stopCamera = false;

        private void Reset()
        {
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerStay(Collider other)
        {
            var pc = other.GetComponent<PlayerController>();
            if (pc != null && pc.isMine)
            {
                CDamage msg = new CDamage();
                msg.entityId = pc.Entity.entityId;
                msg.decHP = damageAmount;
                MyNetwork.Send(msg);
            }
        }
    }
}
