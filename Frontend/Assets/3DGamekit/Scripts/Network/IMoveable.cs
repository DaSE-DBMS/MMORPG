using Common.Data;

namespace Gamekit3D.Network
{
    public interface IMoveable
    {
        void RecvActionMoveBegin(
            V2 move,
            V3 pos,
            V4 rot);

        void RecvActionMoveStep(
            V2 move,
            V3 pos,
            V4 rot);

        void RecvActionMoveEnd(V2 move,
            V3 pos,
            V4 rot);
    }
}
