using Common;
using Gamekit3D;
using Gamekit3D.Network;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Text.RegularExpressions;

public class GameStart : MonoBehaviour
{
    MyNetwork m_network;
    AsyncOperation m_asyncOp;


    const string USERNAME = "username";
    const string PASSWORD = "password";

    public GameObject m_menuBackground;
    public InputField m_usernameInput;
    public InputField m_passwordInput;
    public InputField m_registerUsernameInput;
    public InputField m_registerPasswordInput;
    public InputField m_registerPasswordRepeatInput;
    public InputField m_hostInput;
    public InputField m_portInput;

    void Awake()
    {
        UpdateHostPortInputField();
        UpdateUsernamePasswordInputField();
    }
    void Update()
    {
        if (m_asyncOp == null)
        {
            return;
        }
        if (m_asyncOp.isDone)
        {
            ;
        }
        else
        {

        }
    }

    public void UpdateHostPortInputField()
    {
        if (PlayerPrefs.HasKey(MyNetwork.HOST) && PlayerPrefs.HasKey(MyNetwork.PORT))
        {
            string host = PlayerPrefs.GetString(MyNetwork.HOST);
            int ip = PlayerPrefs.GetInt(MyNetwork.PORT);
            m_hostInput.text = host;
            m_portInput.text = ip.ToString();
        }
    }

    public void UpdateUsernamePasswordInputField()
    {
        if (PlayerPrefs.HasKey(USERNAME) && PlayerPrefs.HasKey(PASSWORD))
        {
            string username = PlayerPrefs.GetString(USERNAME);
            string password = PlayerPrefs.GetString(PASSWORD);
            m_usernameInput.text = username;
            m_passwordInput.text = password;
        }
    }
    public void OnConnectClicked()
    {
        string host = m_hostInput.text;
        short port = short.Parse(m_portInput.text);
        bool ok = MyNetwork.Connect(host, port);
        if (!ok)
        {
            MessageBox.Show(string.Format("connect to {0}:{1} fail", host, port));
        }
        else
        {
            PlayerPrefs.SetString(MyNetwork.HOST, host);
            PlayerPrefs.SetInt(MyNetwork.PORT, port);
            MessageBox.Show(string.Format("connect to {0}:{1} success", host, port));
        }
    }

    public void OnLoginClicked()
    {
        string username = m_usernameInput.text;
        string password = m_passwordInput.text;
        if (!CheckNameAndPassword(username, password, password))
        {
            return;
        }

        bool save = true;
        CLogin login = new CLogin();
        login.user = username;
        login.password = password;
        if (save)
        {
            PlayerPrefs.SetString(USERNAME, username);
            PlayerPrefs.SetString(PASSWORD, password);
        }
        Client.Instance().Send(login);
    }

    public void OnRegisterClicked()
    {
        string username = m_registerUsernameInput.text;
        string password = m_registerPasswordInput.text;
        string pwRepeat = m_registerPasswordRepeatInput.text;
        if (!CheckNameAndPassword(username, password, pwRepeat))
        {
            return;
        }
        CRegister register = new CRegister();
        register.user = username;
        register.password = password;
        Client.Instance().Send(register);
    }

    public void PlayerEnter(string scene)
    {
        Application.backgroundLoadingPriority = ThreadPriority.Low;
        m_menuBackground.SetActive(false);
        StartCoroutine(AsyncLoading(scene));
    }

    IEnumerator AsyncLoading(string scene)
    {
        yield return new WaitForEndOfFrame();
        m_asyncOp = SceneManager.LoadSceneAsync(scene);
        yield return m_asyncOp;
    }

    static bool CheckNameAndPassword(string username, string password, string repeatPw)
    {
        if (username.Length == 0 || password.Length == 0)
        {
            MessageBox.Show("User name and password cannot empty.");
            return false;
        }
        if (!Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$"))
        {
            MessageBox.Show("User name can only have letters, numbers and underscore.");
            return false;
        }
        if (!Regex.IsMatch(password, @"^[a-zA-Z0-9_]+$"))
        {
            MessageBox.Show("Password can only have letters, numbers and underscore.");
            return false;
        }
        if (password != repeatPw)
        {
            MessageBox.Show("The passwords entered did not match.");
        }
        return true;
    }
}
