using System;

namespace Common
{
    [Serializable]
    public class SEquipWeapon : Message
    {
        public SEquipWeapon() : base(Command.S_EQUIP_WEAPON) { }
        public int playerID;
        public string itemName;
        public int itemID;

    }
}
