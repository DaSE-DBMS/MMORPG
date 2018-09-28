using System.Collections.Generic;
using Common.Data;

namespace Backend.Game
{
    public class Creature : Entity
    {
        public int hitPoints;
        public int maxHitPoints;
        public int level;
        public int speed;
        public bool dead = false;
        public bool aggressive = false;
        public string scene;

        public void FindPath(Entity target, Queue<V3> steps)
        {
            GetScene().FindPath(pos, target.pos, steps);
        }

        virtual public void BeHit(Creature enemy)
        {
            if (hitPoints > 0)
            {
                // TODO calculate hit point decrease by creature's attribute
                int dec = 1;
                hitPoints = hitPoints - dec < 0 ? 0 : hitPoints - dec;
                BeHit hit = new BeHit();
                hit.HP = hitPoints;
                hit.id = enemy.id;
                hit.source = this.id;
                Broundcast(hit);
            }
        }
    }
}
