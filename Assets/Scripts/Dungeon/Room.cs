using System;

namespace TableDungeon.Dungeon
{
    public class Room
    {
        public readonly DirectionMap<Room> doors = new DirectionMap<Room>();
        public Chest[] chests = new Chest[4];
        public Trap trap = null;
        public bool blocked = false;
        public State state1 = State.Unreachable;
        public State state2 = State.Unreachable;
        
        private readonly int _seed;

        public Random Random => new Random(_seed);
        
        public Room(int seed)
        {
            _seed = seed;
        }

        public enum State
        {
            Unreachable, Unvisited, Visited
        }
    }
}
