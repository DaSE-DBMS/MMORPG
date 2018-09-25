using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShelfGridHandler : MonoBehaviour
{
    public GameObject prefab;

    private void Awake()
    {

    }
    // Use this for initialization
    void Start()
    {
        if (prefab == null)
        {
            return;
        }
        foreach (KeyValuePair<string, Sprite> kv in GetAllIcons.icons)
        {
            string key = kv.Key;
            GameObject cloned = GameObject.Instantiate(prefab);
            if (cloned == null)
            {
                continue;
            }
            cloned.transform.SetParent(this.transform, false);
            ShelfItemHandler handler = cloned.GetComponent<ShelfItemHandler>();
            if (handler == null)
            {
                continue;
            }
            handler.Init(key);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
