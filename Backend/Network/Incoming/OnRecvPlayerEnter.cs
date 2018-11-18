using Common;
using Backend.Game;

namespace Backend.Network
{
    public partial class Incoming
    {
        private void OnRecvPlayerEnter(IChannel channel, Message message)
        {
            CPlayerEnter request = message as CPlayerEnter;
            Player player = (Player)channel.GetContent();
            Scene scene = World.Instance().GetScene(player.scene);
            // add the player to the scene
            player.Spawn();
            scene.AddEntity(player);
        }
    }
}
