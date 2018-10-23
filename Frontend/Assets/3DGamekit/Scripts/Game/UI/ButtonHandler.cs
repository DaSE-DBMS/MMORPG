using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    public Button button;
    public GameObject prefab;
    private void Awake()
    {
        button = GetComponent<Button>();
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
