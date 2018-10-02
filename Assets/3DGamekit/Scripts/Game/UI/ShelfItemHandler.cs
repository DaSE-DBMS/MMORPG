using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShelfItemHandler : MonoBehaviour
{
    public string itemName;
    public GameObject cartContent;

    Button button;
    Text textName;
    Text textCost;
    CartGridHandler handler;

    private void Awake()
    {
        Transform tf = transform.Find("Button");
        if (tf == null)
        {
            return;
        }
        button = tf.GetComponent<Button>();
        if (button == null)
        {
            return;
        }

        tf = transform.Find("TextName");
        if (tf == null)
        {
            return;
        }
        textName = tf.GetComponent<Text>();
        if (textName == null)
        {
            return;
        }

        tf = transform.Find("TextCost");
        if (tf == null)
        {
            return;
        }
        textCost = tf.GetComponent<Text>();
        if (textCost == null)
        {
            return;
        }
        if (cartContent != null)
        {
            handler = cartContent.GetComponent<CartGridHandler>();
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

    public void Init(string name)
    {
        itemName = name;
        Sprite sprite;
        if (button == null || textName == null || textCost == null)
        {
            return;
        }
        if (!GetAllIcons.icons.TryGetValue(name, out sprite))
        {
            return;
        }
        button.image.sprite = sprite;
        textName.text = name;
        textCost.text = "$5";
    }

    public void AddToCart()
    {
        if (handler != null)
            handler.AddToCart(itemName);
    }
}
