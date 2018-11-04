using System;
using UnityEngine;

namespace Gamekit3D
{
    class DebugUtil
    {
        public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 10, float width = 0.1f)
        {
            if (!Application.isEditor)
            {
                return;
            }
            GameObject line = new GameObject();
            line.transform.position = start;
            line.AddComponent<LineRenderer>();
            LineRenderer render = line.GetComponent<LineRenderer>();
            // Unity Builtin Shader
            render.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
            render.startColor = color;
            render.endColor = color;
            render.startWidth = width;
            render.endWidth = width;
            render.SetPosition(0, start);
            render.SetPosition(1, end);
            GameObject.Destroy(line, duration);
        }

    }
}
