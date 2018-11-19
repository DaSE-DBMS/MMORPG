using System;
using UnityEngine;
using UnityEngine.UI;

public class CartItemUI : MonoBehaviour
{
    public Button button;
    public Text textCost;
    public InputField inputCount;
    int count = 0;
    string itemName;

    void Awake()
    {
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
        inputCount.text = System.Convert.ToString(count);
        textCost.text = "$5";
    }

    public void Increase()
    {
        count++;
        inputCount.text = System.Convert.ToString(count);
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
            CartGridUI gridHandler = transform.parent.GetComponent<CartGridUI>();
            if (gridHandler != null)
            {
                gridHandler.RemoveFromCart(itemName);
            }
        }
        else
        {
            inputCount.text = System.Convert.ToString(count);
            textCost.text = "$5";
        }
    }

}
