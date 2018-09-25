using System;
using System.Collections.Generic;

namespace Backend.Game
{
    public class Player : Creature
    {
        public IChannel connection;
        public string user;
        public string token;

        public Scene GetScene()
        {
            return (Scene)World.Instance.GetEntity(Parent());
        }

        public Player(IChannel channel)
        {
            connection = channel;
            channel.SetContent(this);
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

        virtual public void OnDie()
        {

        }

        virtual public void OnBirth()
        {

        }

        public void SendSpawn(Creature creature)
        {
            SCreatureSpawn msg = new SCreatureSpawn();
            msg.id = creature.id;
            msg.positionX = creature.positionX;
            msg.positionY = creature.positionY;
            msg.positionZ = creature.positionZ;
            msg.rotationX = creature.rotationX;
            msg.rotationY = creature.rotationY;
            msg.rotationZ = creature.rotationZ;
            msg.rotationW = creature.rotationW;
            msg.hitPoints = creature.hitPoints;
            msg.maxHitPoints = creature.maxHitPoints;
            msg.level = creature.level;
            msg.objectName = creature.name;
            msg.isMine = creature.id == this.id;
            connection.Send(msg);
        }

        override public void Spawn()
        {
        }

        override public void Vanish()
        {

        }
    }
}
