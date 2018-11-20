using System;

namespace Common
{
    [Serializable]
    public class CEnemyClosing : Message
    {
        public CEnemyClosing() : base(Command.C_ENEMY_CLOSING) { }
        public int entityId;
        public int enemyId;
    }
}
