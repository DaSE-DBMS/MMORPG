using System;
namespace Common
{
    public enum ItemType
    {
        WEAPON,
    }
    [Serializable]
    public class CPlayerTake : Message
    {
        public CPlayerTake() : base(Command.C_PLAYER_TAKE)
        {
        }
        public ItemType itemType;
        public bool byName;
        public int playerId;
        public int targetId;
        public string targetName;
    }
}
