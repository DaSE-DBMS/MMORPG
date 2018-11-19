using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagUI : MonoBehaviour
{

    public GameObject BagCell;
    public GameObject BagGridContent;
    // Use this for initialization

    private void Awake()
    {
        BagCell.SetActive(false);
    }

    void Start()
    {
        // TODO
        ExtendBagCapacity(16);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ExtendBagCapacity(int n)
    {
        int cellCount = BagGridContent.transform.childCount;
        for (int i = 0; i < n - cellCount; i++)
        {
            GameObject cloned = GameObject.Instantiate(BagCell);
            cloned.SetActive(true);
            cloned.transform.SetParent(BagGridContent.transform, false);
        }
    }
}
