namespace Backend.Game
{
    public class Creature : Entity
    {
        public int hitPoints;
        public int maxHitPoints;
        public int level;
        public bool dead = false;
        public bool aggressive = false;
        public string scene;
    }
}
