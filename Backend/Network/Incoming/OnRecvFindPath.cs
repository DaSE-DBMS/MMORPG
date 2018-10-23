using System.Collections.Generic;
using Common;
using Backend.Game;
using GeometRi;

namespace Backend.Network
{
    public partial class Incoming
    {
        private void RecvFindPath(IChannel channel, Message message)
        {
            CFindPath request = (CFindPath)message;
            Player player = (Player)channel.GetContent();
            V3 start = player.GetPosition();
            Point3d end = new Point3d((float)request.pos.x,
                (float)request.pos.y,
                (float)request.pos.z);
            LinkedList<Point3d> path = new LinkedList<Point3d>();
            if (player.GetScene().FindPath(player.Position, end, path))
            {
                SFindPath response = new SFindPath();
                foreach (Point3d point in path)
                {
                    V3 v3 = new V3((float)point.X, (float)point.Y, (float)point.Z);
                    response.path.Add(v3);
                }
                channel.Send(response);
            }
        }
    }
}
