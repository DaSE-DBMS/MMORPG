using Common;
using Backend.Game;

namespace Backend.Network
{
    public partial class Incoming
    {
        private void OnRecvEnemyClosing(IChannel channel, Message message)
        {
            CEnemyClosing request = message as CEnemyClosing;
            Sprite attacker = World.Instance.GetEntity(request.entityId) as Sprite;
            Creature enemy = World.Instance.GetEntity(request.enemyId) as Creature;
            if (attacker == null || enemy == null)
                return;
            attacker.EnemyClosing(enemy);
        }
    }
}
