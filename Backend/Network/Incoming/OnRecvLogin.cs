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
            string scene = "Level1";
            response.user = request.user;
            response.token = request.user;
            response.scene = scene;
            channel.Send(response);

            Player player = new Player(channel);
            player.scene = scene;
            // TODO read from database
            DEntity dentity = World.Instance().EntityData["Ellen"];
            player.FromDEntity(dentity);
            player.forClone = false;
            ClientTipInfo(channel, "TODO: get player's attribute from database");
            // player will be added to scene when receive client's CEnterSceneDone message
        }
    }
}
