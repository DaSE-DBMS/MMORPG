using Common;
using Backend.Game;

namespace Backend.Network
{
    public partial class Incoming
    {
        private void OnRecvLogin(IChannel channel, Message message)
        {
            CLogin request = (CLogin)message;
            SPlayerEnter response = new SPlayerEnter();
            response.user = request.user;
            response.token = request.user;
            channel.Send(response);
            Player player = new Player(channel);
            DEntity dentity = World.Instance().EntityData["Ellen"];
            player.FromDEntity(dentity);
            player.forClone = false;

            // player will be added to scene when receive client's CEnterSceneDone message
        }
    }
}
