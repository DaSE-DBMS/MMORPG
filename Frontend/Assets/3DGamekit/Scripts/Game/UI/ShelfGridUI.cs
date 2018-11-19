using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShelfGridUI : MonoBehaviour
{
    public GameObject ShelfItem;

    private void Awake()
    {

    }
    // Use this for initialization
    void Start()
    {
        foreach (KeyValuePair<string, Sprite> kv in GetAllIcons.icons)
        {
            string key = kv.Key;
            GameObject cloned = GameObject.Instantiate(ShelfItem);
            if (cloned == null)
            {
                continue;
            }
            cloned.SetActive(true);
            cloned.transform.SetParent(this.transform, false);
            ShelfItemUI handler = cloned.GetComponent<ShelfItemUI>();
            if (handler == null)
            {
                continue;
            }
            handler.Init(key);
        }
        ShelfItem.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
