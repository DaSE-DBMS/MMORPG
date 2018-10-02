using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    namespace Data
    {
        [Serializable]
        public struct V2
        {
            public V2(float x, float y)
            {
                this.x = x;
                this.y = y;
            }
            public float x;
            public float y;
        }

        [Serializable]
        public struct V3
        {
            public V3(float x, float y, float z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }
            public float x;
            public float y;
            public float z;
        }

        [Serializable]
        public struct V4
        {
            public V4(float x, float y, float z, float w)
            {
                this.x = x;
                this.y = y;
                this.z = z;
                this.w = w;
            }
            public float x;
            public float y;
            public float z;
            public float w;
        }

        [Serializable]
        public class DTngl
        {
            public V3[] p = new V3[3];
        }

        [Serializable]
        public class DNavM
        {
            public List<DTngl> list = new List<DTngl>();
        }

        public enum EntityType
        {
            PLAYER = 0,
            SPRITE = 1,
            ITEM = 2,
            WEAPON = 3,
            UNKOWN = 4,
        }

        [Serializable]
        public class DEntity
        {

            public int type;
            public int id;
            public string name;
            public int currentHP;
            public int maxHP;
            public float invTime; // invulnerabilty time, second
            public float hitAngle;
            public int level;
            public int speed;
            public bool aggressive;
            public bool forClone;
            public DEntity parent;
            public List<DEntity> children = new List<DEntity>();
            public V3 pos = new V3();
            public V4 rot = new V4();
        }

        [Serializable]
        public class DEntityList
        {
            public List<DEntity> list = new List<DEntity>();
        }

        [Serializable]
        public class DSceneAsset
        {
            public string scene;

            public DEntityList entities = new DEntityList();

            public DNavM mesh = new DNavM();
        }
    }
}




