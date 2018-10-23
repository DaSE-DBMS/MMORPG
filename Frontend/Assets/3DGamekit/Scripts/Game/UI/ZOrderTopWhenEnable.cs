using System;
using System.Collections.Generic;
using UnityEngine;

public class ZOrderTopWhenEnable : MonoBehaviour
{
    private void OnEnable()
    {
        int count = transform.parent.childCount;
        transform.SetSiblingIndex(count - 1);
    }
}
