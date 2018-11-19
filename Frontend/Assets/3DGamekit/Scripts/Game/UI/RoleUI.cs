using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoleUI : MonoBehaviour
{
    public TextMeshProUGUI HPValue;
    public TextMeshProUGUI InteligenceValue;
    public TextMeshProUGUI SpeedValue;
    public TextMeshProUGUI LevelValue;
    public TextMeshProUGUI AttackValue;
    public TextMeshProUGUI DefenseValue;

    private void Awake()
    {

    }
    // Use this for initialization
    void Start()
    {
        // TODO ...
        Test();
    }

    private void OnEnable()
    {

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
}
