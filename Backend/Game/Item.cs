using Common.Data;

namespace Backend.Game
{
    public class Item : Entity
    {
        override public DEntity ToDEntity()
        {
            DEntity entity = base.ToDEntity();
            entity.type = (int)EntityType.ITEM;
            return entity;
        }

        override public void FromDEntity(DEntity entity)
        {
            base.FromDEntity(entity);
        }
    }
}
