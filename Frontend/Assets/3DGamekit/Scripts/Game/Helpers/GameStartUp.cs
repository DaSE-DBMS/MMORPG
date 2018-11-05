using UnityEngine;
using UnityEngine.SceneManagement;
using Gamekit3D.Network;
using System.Collections;

public class GameStartUp : MonoBehaviour
{
    MyNetwork m_network;
    AsyncOperation m_asyncOp;

    public void Awake()
    {
        m_network = GameObject.FindObjectOfType<MyNetwork>();
    }
    public void Update()
    {

    }

    public void OnStart()
    {
        // ... TODO get ip && port from configure file
        m_network.Connect();
    }

    public void LoadScene(string scene)
    {
        StartCoroutine(scene);
    }

    IEnumerator AsyncLoading(string scene)
    {
        m_asyncOp = SceneManager.LoadSceneAsync(scene);
        m_asyncOp.allowSceneActivation = false;
        yield return m_asyncOp;
    }
}
