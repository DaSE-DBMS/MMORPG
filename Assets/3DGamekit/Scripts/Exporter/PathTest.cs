using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine;
//using UnityEngine.AI;
using System.Xml.Serialization;
using Common.Data;
using Frontend.Network;

//navmesh导出数据
public class PathTest : MonoBehaviour
{
    public Vector3 pos = Vector3.zero;

    public void Test()
    {
        CPathFinding msg = new CPathFinding();
        msg.pos.x = pos.x;
        msg.pos.y = pos.y;
        msg.pos.z = pos.z;
        MyNetwork.instance.Send(msg);
    }
}

[CustomEditor(typeof(PathTest))]
public class NavExportHelper : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("PathTest"))
        {
            var test = target as PathTest;
            test.Test();
        }
    }
}
