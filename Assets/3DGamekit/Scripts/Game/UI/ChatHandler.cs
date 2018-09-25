using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatHandler : MonoBehaviour
{

    public GameObject messageView;
    public GameObject myMessage;
    public GameObject friendMessage;


    // my message info content layout | ----------------- message text | image |
    // friend's info content layout   | image | message text ----------------- |

    // Use this for initialization
    void Start()
    {
        Test();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ReceiveFriendMessage(string text)
    {
        if (friendMessage == null)
            return;

        GameObject cloned = GameObject.Instantiate(friendMessage);
        if (cloned == null)
            return;

        AddElement(cloned, text);
    }

    public void SendMyMessage(string text)
    {
        if (myMessage == null)
            return;

        GameObject cloned = GameObject.Instantiate(myMessage);
        if (cloned == null)
            return;

        AddElement(cloned, text);
    }

    public void OnSendButtonClick(GameObject inputField)
    {
        InputField input = inputField.GetComponent<InputField>();
        if (input == null)
            return;

        if (input.text.Trim().Length == 0)
            return;


        SendMyMessage(input.text);

        input.text = "";
    }

    void AddElement(GameObject element, string text)
    {
        TextMeshProUGUI textMesh = element.GetComponentInChildren<TextMeshProUGUI>();
        if (textMesh == null)
            return;
        //float width = textMesh.GetPreferredValues().x; // get preferred width before assign text
        textMesh.text = text;
        RectTransform rectTransform = element.GetComponent<RectTransform>();
        if (rectTransform == null)
            return;

        RectTransform parentRect = this.GetComponent<RectTransform>();
        if (parentRect == null)
            return;

        if (textMesh.preferredWidth < parentRect.rect.width)
        {
            ContentSizeFitter filter = textMesh.GetComponent<ContentSizeFitter>();
            if (filter != null)
            {
                filter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                textMesh.rectTransform.sizeDelta = new Vector2(textMesh.preferredWidth, textMesh.preferredHeight);
            }
        }

        element.transform.SetParent(this.transform, false);

        ScrollRect scrollRect = messageView.GetComponent<ScrollRect>();
        if (scrollRect == null)
            return;

        scrollRect.normalizedPosition = new Vector2(0, 0);
    }

    void Test()
    {
        //AddNewMessage(true, "my message send");
        //AddNewMessage(false, "friend message receive");

        SendMyMessage("hello");
        ReceiveFriendMessage("hello");
    }

    /*
    void AddNewMessage(bool mine, string message)
    {
        GameObject newContent = GameObject.Instantiate(content);
        if (newContent == null)
            return;
        GameObject newImage = GameObject.Instantiate(image);
        if (newImage == null)
            return;
        GameObject newText = GameObject.Instantiate(text);
        if (newText == null)
            return;

        HorizontalLayoutGroup layout = newContent.GetComponent<HorizontalLayoutGroup>();
        if (mine)
            layout.childAlignment = TextAnchor.UpperRight;
        else
            layout.childAlignment = TextAnchor.UpperLeft;

        TextMeshProUGUI textMesh = text.GetComponentInChildren<TextMeshProUGUI>();
        if (textMesh == null)
            return;

        //float width = textMesh.GetPreferredValues().x; // get preferred width before assign text
        textMesh.text = message;
        RectTransform rectTransform = text.GetComponent<RectTransform>();
        if (rectTransform == null)
            return;

        RectTransform viewRect = messageContent.GetComponent<RectTransform>();
        if (viewRect == null)
            return;

        if (textMesh.preferredWidth < viewRect.rect.width)
        {
            ContentSizeFitter filter = textMesh.GetComponent<ContentSizeFitter>();
            if (filter != null)
            {
                filter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                rectTransform.sizeDelta = new Vector2(textMesh.preferredWidth, textMesh.preferredHeight);
            }
        }

        newImage.transform.SetParent(newContent.transform, false);
        newText.transform.SetParent(newContent.transform, false);
        newContent.transform.SetParent(messageContent.transform, false);
    }
    */
}
