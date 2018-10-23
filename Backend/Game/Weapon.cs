using System;
using System.Collections.Generic;
using System.Text;
using Common;

namespace Backend.Game
{
    public class Weapon : Item
    {
        override public DEntity ToDEntity()
        {
            DEntity entity = base.ToDEntity();
            return entity;
        }
    }
}
