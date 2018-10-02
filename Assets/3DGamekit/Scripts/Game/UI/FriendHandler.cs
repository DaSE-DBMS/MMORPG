using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendHandler : MonoBehaviour
{
    public GameObject friendInfo;

    private void Awake()
    {
        if (friendInfo == null)
        {
            return;
        }
        for (int i = 0; i < 100; i++)
        {
            GameObject cloned = GameObject.Instantiate(friendInfo);
            cloned.transform.SetParent(transform, false);
        }
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
