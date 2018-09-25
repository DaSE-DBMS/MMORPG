using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendHandler : MonoBehaviour
{
    public GameObject friendInfo;

    bool m_init = false;
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
        m_init = true;
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
