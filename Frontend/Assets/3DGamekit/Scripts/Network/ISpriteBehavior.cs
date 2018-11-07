using Common;
using UnityEngine;

namespace Gamekit3D.Network
{
    public interface ISpriteBehavior
    {
        void BeginChase(Vector3 position, ICreatureBehavior target);

        void EndChase(Vector3 position);

        void BeginBack(Vector3 position);

        void EndBack(Vector3 position);

        void MoveStep(Vector3 position);
    }
}
