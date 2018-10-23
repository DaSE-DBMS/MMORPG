using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetAllIcons : MonoBehaviour
{

    public static Dictionary<string, Sprite> icons = new Dictionary<string, Sprite>();

    // Use this for initialization
    void Awake()
    {
        Component[] components = GetComponentsInChildren(typeof(SpriteRenderer), true);
        foreach (Component component in components)
        {
            SpriteRenderer sr = (SpriteRenderer)component;
            icons.Add(sr.gameObject.name, sr.sprite);
        }
    }

    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {

    }
}
