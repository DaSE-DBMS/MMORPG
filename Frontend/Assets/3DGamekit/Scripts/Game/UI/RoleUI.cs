using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Gamekit3D;

public class RoleUI : MonoBehaviour
{

    public TextMeshProUGUI HPValue;
    public TextMeshProUGUI InteligenceValue;
    public TextMeshProUGUI SpeedValue;
    public TextMeshProUGUI LevelValue;
    public TextMeshProUGUI AttackValue;
    public TextMeshProUGUI DefenseValue;

    private Damageable m_damageable;
    private PlayerController m_controller;

    private void Awake()
    {

    }
    // Use this for initialization
    void Start()
    {

    }

    private void OnEnable()
    {
        PlayerMyController.Instance.EnabledWindowCount++;
        if (m_controller == null || m_damageable == null)
        {
            m_controller = PlayerController.Mine;
            m_damageable = PlayerController.Mine.GetComponent<Damageable>();
        }
        string hp = string.Format("{0}/{1}", m_damageable.currentHitPoints, m_damageable.maxHitPoints);
        HPValue.SetText(hp, true);
    }

    private void OnDisable()
    {
        PlayerMyController.Instance.EnabledWindowCount--;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Test()
    {
        HPValue.text = "100";
        InteligenceValue.text = "100";
    }

    public void InitUI(PlayerController controller)
    {
        m_damageable = controller.GetComponent<Damageable>();
        m_controller = controller;
    }
}
