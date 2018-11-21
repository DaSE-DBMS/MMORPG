using Common;
using System;

namespace Backend.Game
{
    public class Player : Creature
    {
        public IChannel connection;
        public string user;
        public string token;
        private Weapon m_weapon;

        public Weapon Weapon { get { return m_weapon; } }
        public Player(IChannel channel)
        {
            connection = channel;
            channel.SetContent(this);
        }
        override public void OnHit(Creature enemy, int hpDec)
        {
            if (currentHP == 0)
                return;

            if (IsInvulnerable())
                return;

            m_lastHitTS = DateTime.Now;
            hpDec = currentHP - hpDec < 0 ? currentHP : hpDec;

            SHit hit = new SHit();
            hit.decHP = hpDec;
            hit.sourceId = enemy != null ? enemy.entityId : 0;
            hit.targetId = this.entityId;
            Broadcast(hit);

            currentHP = currentHP - hpDec;
            if (currentHP == 0)
            {
                OnDie();
                World.Instance.DelayInvoke(5, OnReSpawn);
            }
        }

        public override void OnReSpawn()
        {
            // TODO read from last savepoint
            // read from database
            V3 pos = DefaultData.pos;
            Position = V3ToPoint3d(pos);
            currentHP = maxHP;

            SPlayerReSpawn msg = new SPlayerReSpawn();
            msg.entityId = entityId;
            msg.HP = maxHP;
            msg.position = pos;
            Broadcast(msg);
        }

        override public DEntity ToDEntity()
        {
            DEntity entity = base.ToDEntity();
            return entity;
        }

        override public void FromDEntity(DEntity entity)
        {
            scene = "Level1";
            name = "Ellen";
            base.FromDEntity(entity);
        }

        virtual public void OnEnterScene(Scene scene)
        {

        }

        virtual public void OnLeaveScene(Scene scene)
        {

        }

        virtual public void OnEquiped(Item item)
        {

        }

        virtual public void OnUnEquiped(Item item)
        {

        }

        override public void OnDie()
        {
            SPlayerDie msg = new SPlayerDie();
            msg.entityId = entityId;
            msg.isMine = false;
            Broadcast(msg, true);

            SPlayerDie msg1 = new SPlayerDie();
            msg1.entityId = entityId;
            msg1.isMine = true;
            connection.Send(msg1);
        }

        virtual public void OnBirth()
        {

        }

        public void SendSpawn(DEntity entity)
        {
            SSpawn msg = new SSpawn();
            msg.entity = entity;
            msg.isMine = entity.entityID == entityId;
            connection.Send(msg);
        }

        override public void Spawn()
        {
        }

        override public void Vanish()
        {

        }

        public void TakeItem(Item target)
        {
            SPlayerTake msgTake = new SPlayerTake();

            if (target.forClone)
            {
                Entity clone = World.CreateEntityByName(target.name);
                clone.forClone = false;
                if (target == null)
                    return;
                msgTake.clone = true;
                msgTake.itemID = clone.entityId;
                msgTake.name = clone.name;
                target = (Item)clone;
            }
            else
            {
                msgTake.clone = false;
                msgTake.itemID = target.entityId;
                msgTake.name = target.name;
            }

            if (!(target is Item))
            {
                return;
            }
            //msgTake.itemId;
            AddEntity(target);
            connection.Send(msgTake);
            if (target is Weapon)
            {
                EquipWeapon((Weapon)target);
            }
        }

        public void EquipWeapon(Weapon weapon)
        {
            if (m_weapon != null)
                return;

            m_weapon = weapon;
            SEquipWeapon msgEquip = new SEquipWeapon();
            msgEquip.playerID = this.entityId;
            msgEquip.itemName = this.m_weapon.name;
            msgEquip.itemID = this.m_weapon.entityId;

            Broadcast(msgEquip);
        }

        public bool IsEquipedWeapon()
        {
            return m_weapon != null;
        }

        public void SendEquipWeapon(Player player)
        {
            SEquipWeapon msgEquip = new SEquipWeapon();
            msgEquip.playerID = player.entityId;
            msgEquip.itemName = player.m_weapon.name;
            msgEquip.itemID = player.m_weapon.entityId;
            connection.Send(msgEquip);
        }
    }
}
