using System.Collections;
using UnityEngine;

namespace Gamekit3D
{
    public class HealthUI : MonoBehaviour
    {
        public GameObject HealthIconPrefab;
        private Damageable m_representedDamageable;
        protected Animator[] m_HealthIconAnimators;

        protected readonly int m_HashActivePara = Animator.StringToHash("Active");
        protected readonly int m_HashInactiveState = Animator.StringToHash("Inactive");
        protected const float k_HeartIconAnchorWidth = 0.041f;

        private void Awake()
        {
            HealthIconPrefab.SetActive(false);
        }

        public void InitUI(Damageable damageable)
        {
            m_representedDamageable = damageable;
            m_HealthIconAnimators = new Animator[m_representedDamageable.maxHitPoints];

            for (int i = 0; i < m_representedDamageable.maxHitPoints; i++)
            {
                GameObject healthIcon = Instantiate(HealthIconPrefab);
                healthIcon.SetActive(true);
                healthIcon.transform.SetParent(transform);
                /*
                RectTransform healthIconRect = healthIcon.transform as RectTransform;
                healthIconRect.anchoredPosition = Vector2.zero;
                healthIconRect.sizeDelta = Vector2.zero;
                healthIconRect.anchorMin += new Vector2(k_HeartIconAnchorWidth, 0f) * i;
                healthIconRect.anchorMax += new Vector2(k_HeartIconAnchorWidth, 0f) * i;
                */
                m_HealthIconAnimators[i] = healthIcon.GetComponent<Animator>();

                if (m_representedDamageable.currentHitPoints < i + 1)
                {
                    m_HealthIconAnimators[i].Play(m_HashInactiveState);
                    m_HealthIconAnimators[i].SetBool(m_HashActivePara, false);
                }
            }
        }
        public void ChangeHitPointUI(Damageable damageable)
        {
            if (m_HealthIconAnimators == null)
                return;

            for (int i = 0; i < m_HealthIconAnimators.Length; i++)
            {
                m_HealthIconAnimators[i].SetBool(m_HashActivePara, damageable.currentHitPoints >= i + 1);
            }
        }
    }
}
