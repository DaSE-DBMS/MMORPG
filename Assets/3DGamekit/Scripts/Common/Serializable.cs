using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    namespace Data
    {
        [Serializable]
        public class Pos
        {
            public Pos()
            {
                x = y = z = 0;
            }

            public Pos(float x, float y, float z)
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
        public class Tngl
        {
            public Pos[] p = new Pos[3];
        }

        [Serializable]
        public class NavM
        {
            public List<Tngl> mesh = new List<Tngl>();
        }
    }
}




