using Common.Data;
using UnityEngine;

namespace Gamekit3D.Network
{
    public interface IHitable
    {
        void RecvHit(int HP, GameObject source);
    }
}

