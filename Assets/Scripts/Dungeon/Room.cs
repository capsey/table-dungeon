using System;

namespace TableDungeon.Dungeon
{
    public class Room
    {
        public readonly int seed;
        public readonly DirectionMap<Room> doors = new DirectionMap<Room>();
        public Item[] chests = new Item[4];
        public State state = State.Unreachable;

        public Random Random => new Random(seed);
        
        public Room(int seed)
        {
            this.seed = seed;
        }

        public enum State
        {
            Unreachable, Unvisited, Visited
        }
    }
}
