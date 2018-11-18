using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CartGridHandler : MonoBehaviour
{
    public GameObject prefab;

    private Dictionary<string, GameObject> items = new Dictionary<string, GameObject>();

    private void Awake()
    {
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddToCart(string name)
    {
        Sprite sprite;
        GameObject item;
        if (prefab == null)
        {
            return;
        }
        if (!GetAllIcons.icons.TryGetValue(name, out sprite))
        {
            return;
        }
        bool exists = items.TryGetValue(name, out item);
        if (!exists)
        {
            item = GameObject.Instantiate(prefab);
            if (item == null)
            {
                return;
            }
            item.transform.SetParent(transform, false);
            item.SetActive(true);
            items.Add(name, item);
        }
        CartItemHandler handler = item.GetComponent<CartItemHandler>();
        if (handler == null)
        {
            return;
        }

        if (exists)
        {
            handler.Increase();
        }
        else
        {
            handler.Init(name);
        }
    }

    public void RemoveFromCart(string name)
    {
        GameObject item;
        if (items.TryGetValue(name, out item))
        {
            items.Remove(name);
            Destroy(item);
        }

    }
}
