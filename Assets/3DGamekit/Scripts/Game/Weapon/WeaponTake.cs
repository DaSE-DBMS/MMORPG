using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gamekit3D
{
    [RequireComponent(typeof(Collider))]
    public class WeaponTake : MonoBehaviour
    {
        public UnityEvent OnEnter, OnExit;
        public GameObject weapon;
        void Reset()
        {

        }

        void OnTriggerEnter(Collider other)
        {
            ExecuteOnEnter(other);
        }

        protected virtual void ExecuteOnEnter(Collider other)
        {

            PlayerController controller = other.GetComponent<PlayerController>();
            if (controller == null)
            {
                return;
            }
            controller.PlayerWantTakeWeapon(weapon);
            OnEnter.Invoke();
        }

        void OnTriggerExit(Collider other)
        {
            ExecuteOnExit(other);
        }

        protected virtual void ExecuteOnExit(Collider other)
        {
            OnExit.Invoke();
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "InteractionTrigger", false);
        }

        void OnDrawGizmosSelected()
        {
            //need to inspect events and draw arrows to relevant gameObjects.
        }

    }
}
