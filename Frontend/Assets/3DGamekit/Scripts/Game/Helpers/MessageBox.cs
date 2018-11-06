using UnityEngine;

namespace Gamekit3D
{
    class MessageBox : Singleton<MessageBox>
    {
        static private GameObject m_messageBox;
        static private DialogueCanvasController m_controller;

        static public void Init()
        {
            if (m_controller == null || m_messageBox == null)
            {
                m_messageBox = GameObject.Instantiate(Resources.Load("MessageBox")) as GameObject;
                m_controller = m_messageBox.GetComponent<DialogueCanvasController>();
            }
        }

        static public void Show(string text)
        {
            m_controller.ActivateCanvasWithText(text);
            m_controller.DeactivateCanvasWithDelay(5);
        }
    }
}
