using System;
using UnityEngine;
using UnityEngine.UI;

public class CartItemHandler : MonoBehaviour
{
    Button button;
    Text textCost;
    InputField textCount;
    int count = 0;
    string itemName;

    void Awake()
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

        tf = transform.Find("InputCount");
        if (tf == null)
        {
            return;
        }
        textCount = tf.GetComponent<InputField>();
        if (textCount == null)
        {
            return;
        }


    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init(string name)
    {
        Sprite sprite;
        if (button == null || textCost == null || textCost == null)
        {
            return;
        }
        if (!GetAllIcons.icons.TryGetValue(name, out sprite))
        {
            return;
        }
        itemName = name;
        count++;
        button.image.sprite = sprite;
        textCount.text = System.Convert.ToString(count);
        textCost.text = "$5";
    }

    public void Increase()
    {
        count++;
        textCount.text = System.Convert.ToString(count);
        textCost.text = "$5";
    }

    public void Decrease()
    {
        count--;
        if (count == 0)
        {
            if (transform.parent == null)
            {
                return;
            }
            CartGridHandler gridHandler = transform.parent.GetComponent<CartGridHandler>();
            if (gridHandler != null)
            {
                gridHandler.RemoveFromCart(itemName);
            }
        }
        else
        {
            textCount.text = System.Convert.ToString(count);
            textCost.text = "$5";
        }
    }

}
