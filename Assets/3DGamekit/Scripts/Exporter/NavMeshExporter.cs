using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine;
//using UnityEngine.AI;
using System.Xml.Serialization;
using Common.Data;
using Gamekit3D.Network;

//navmesh导出数据
public class NavMeshExporter : MonoBehaviour
{
    private GameObject _testMap;
    [MenuItem("Tools/Export Assets")]
    private static void Export()
    {
        DSceneAsset asset = new DSceneAsset();
        NavMeshToAsset(asset);
        EntitiesToAsset(asset);
        using (StreamWriter sw = new StreamWriter(Application.dataPath + "/navmesh/" + asset.scene + ".xml"))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(DSceneAsset));
            serializer.Serialize(sw, asset);
        }
        Debug.Log("导出完成：" + asset.scene);
        AssetDatabase.Refresh();
    }

    private static void NavMeshToAsset(DSceneAsset asset)
    {
        GameObject obj = CreateNavMeshObject();
        Transform transform = obj.transform.Find("_NavMesh");
        Vector3[] localVectors = transform.GetComponent<MeshFilter>().sharedMesh.vertices;
        int[] triangles = transform.GetComponent<MeshFilter>().sharedMesh.triangles;
        Vector3[] worldVectors = new Vector3[localVectors.Length];
        for (int i = 0; i < localVectors.Length; i++)
        {
            Vector3 pos = transform.TransformPoint(localVectors[i]);
            worldVectors[i] = pos;

        }
        for (int i = 0; i < triangles.Length; i += 3)
        {

            DTngl element = new DTngl();
            for (int j = 0; j < 3; j++)
            {
                element.p[j] = new V3();
                element.p[j].x = worldVectors[triangles[i + j]].x;
                element.p[j].y = worldVectors[triangles[i + j]].y;
                element.p[j].z = worldVectors[triangles[i + j]].z;
            }
            asset.mesh.list.Add(element);
        }

        DestroyImmediate(obj);
    }

    private static void EntitiesToAsset(DSceneAsset asset)
    {
        // exprot all network entity
        asset.scene = SceneManager.GetActiveScene().name;
        NetworkEntity[] entities = GameObject.FindObjectsOfType<NetworkEntity>();
        foreach (NetworkEntity entity in entities)
        {
            entity.Init();
            if (entity.parent == null)
            {
                asset.entities.list.Add(entity.ToDEntity());
            }
        }
    }

    static private GameObject CreateNavMeshObject()
    {
        UnityEngine.AI.NavMeshTriangulation triangulatedNavMesh = UnityEngine.AI.NavMesh.CalculateTriangulation();

        Mesh mesh = new Mesh();
        mesh.name = "_NavMesh";
        mesh.vertices = triangulatedNavMesh.vertices;
        mesh.triangles = triangulatedNavMesh.indices;

        string baseName = "navmesh_" + SceneManager.GetActiveScene().name;
        string fileName = Application.dataPath + "/navmesh/" + baseName + ".obj";
        ExportNavmesh(mesh, fileName);

        AssetDatabase.Refresh();
        string assetName = fileName.Replace(Application.dataPath, "Assets");
        GameObject navMesh = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(assetName));
        navMesh.name = baseName;

        ExportNavMeshLua(navMesh);
        return navMesh;
    }

    [MenuItem("Tools/NavMesh Data Test")]
    private static void Test()
    {
        GameObject obj = GameObject.Find("_NavMesh");
        Vector3[] localVectors = obj.GetComponent<MeshFilter>().sharedMesh.vertices;
        int[] triangles = obj.GetComponent<MeshFilter>().sharedMesh.triangles;

        //把mesh的本地坐标转成世界坐标
        Vector3[] worldVectors = new Vector3[localVectors.Length];
        for (int i = 0; i < localVectors.Length; ++i)
        {
            Vector3 pos = obj.transform.TransformPoint(localVectors[i]);
            worldVectors[i] = pos;
        }

        //检测点
        Vector3 checkPoint = GameObject.Find("TestPoint").transform.position;
        bool _isInside = false;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            //Debug.Log(string.Format("{0},{1},{2}", triangles[i], triangles[i + 1], triangles[i + 2]));

            if (IsInside(worldVectors[triangles[i]], worldVectors[triangles[i + 1]], worldVectors[triangles[i + 2]], checkPoint))
            {
                _isInside = true;
                break;
            }
        }

        if (_isInside)
            Debug.Log("该点合法");
        else
            Debug.Log("该点非法");
    }

    private static string ParseMesh(Mesh mesh)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("g ").Append(mesh.name).Append("\n");
        foreach (Vector3 v in mesh.vertices)
        {
            // -v.x ????
            sb.Append(string.Format("v {0} {1} {2}\n", -v.x, v.y, v.z));
        }
        sb.Append("\n");
        foreach (Vector3 v in mesh.normals)
        {
            sb.Append(string.Format("vn {0} {1} {2}\n", v.x, v.y, v.z));
        }
        sb.Append("\n");
        foreach (Vector3 v in mesh.uv)
        {
            sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
        }
        for (int m = 0; m < mesh.subMeshCount; m++)
        {
            sb.Append("\n");

            int[] triangles = mesh.GetTriangles(m);
            for (int i = 0; i < triangles.Length; i += 3)
            {
                sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n", triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1));
            }
        }
        return sb.ToString();
    }

    private static void ExportNavmesh(Mesh mesh, string filename)
    {
        using (StreamWriter sw = new StreamWriter(filename))
        {
            sw.Write(ParseMesh(mesh));
        }
    }

    private static void ExportNavMeshLua(GameObject obj)
    {
        Transform transform = obj.transform.Find("_NavMesh");
        Vector3[] localVectors = transform.GetComponent<MeshFilter>().sharedMesh.vertices;
        int[] triangles = transform.GetComponent<MeshFilter>().sharedMesh.triangles;
        //把mesh的本地坐标转成世界坐标
        Vector3[] worldVectors = new Vector3[localVectors.Length];
        for (int i = 0; i < localVectors.Length; i++)
        {
            Vector3 pos = transform.TransformPoint(localVectors[i]);
            worldVectors[i] = pos;
        }
        StringBuilder sb = new StringBuilder();
        sb.Append("local nav = {\n");
        for (int i = 0; i < triangles.Length; i += 3)
        {
            sb.AppendFormat("\t{{{0},{1},{2}}},\n", _VectorToLua(worldVectors[triangles[i]]), _VectorToLua(worldVectors[triangles[i + 1]]), _VectorToLua(worldVectors[triangles[i + 2]]));
        }
        sb.Append("}\n");
        sb.Append("return nav");
        using (StreamWriter sw = new StreamWriter(Application.dataPath + "/navmesh/" + obj.name + ".lua"))
        {
            sw.Write(sb.ToString());
        }
    }

    private static string _VectorToLua(Vector3 vec)
    {
        return string.Format("{{{0},{1},{2}}}", vec.x, vec.y, vec.z);
    }

    //判断点是否在三角形内
    public static bool IsInside(Vector3 A, Vector3 B, Vector3 C, Vector3 P)
    {
        Vector3 v0 = C - A;
        Vector3 v1 = B - A;
        Vector3 v2 = P - A;

        float dot00 = Vector3.Dot(v0, v0);
        float dot01 = Vector3.Dot(v0, v1);
        float dot02 = Vector3.Dot(v0, v2);
        float dot11 = Vector3.Dot(v1, v1);
        float dot12 = Vector3.Dot(v1, v2);

        float inverDeno = 1 / (dot00 * dot11 - dot01 * dot01);

        float u = (dot11 * dot02 - dot01 * dot12) * inverDeno;
        if (u < 0 || u > 1) // if u out of range, return directly
        {
            return false;
        }

        float v = (dot00 * dot12 - dot01 * dot02) * inverDeno;
        if (v < 0 || v > 1) // if v out of range, return directly
        {
            return false;
        }

        return u + v <= 1;

    }
}
