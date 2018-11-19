using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CartGridUI : MonoBehaviour
{
    public GameObject CartItem;

    private Dictionary<string, GameObject> m_items = new Dictionary<string, GameObject>();

    private void Awake()
    {
        CartItem.SetActive(false);
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
        if (!GetAllIcons.icons.TryGetValue(name, out sprite))
        {
            return;
        }
        bool exists = m_items.TryGetValue(name, out item);
        if (!exists)
        {
            item = GameObject.Instantiate(CartItem);
            if (item == null)
            {
                return;
            }
            item.transform.SetParent(transform, false);
            item.SetActive(true);
            m_items.Add(name, item);
        }
        CartItemUI handler = item.GetComponent<CartItemUI>();
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
        if (m_items.TryGetValue(name, out item))
        {
            m_items.Remove(name);
            Destroy(item);
        }
    }

    public void OnBuyButtonClicked()
    {
        foreach (var kv in m_items)
        {
            // TODO ...
        }
    }

}
