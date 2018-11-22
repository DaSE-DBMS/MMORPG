using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit3D;

public class ShelfViewUI : MonoBehaviour
{
    private void Awake()
    {
    }

    private void OnEnable()
    {
        PlayerMyController.Instance.EnabledWindowCount++;
    }

    private void OnDisable()
    {
        PlayerMyController.Instance.EnabledWindowCount--;
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
