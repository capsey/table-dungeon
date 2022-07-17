using System;

namespace TableDungeon.Dungeon
{
    public class Item
    {
        public readonly Type type;
        public bool looted = false;

        public Item(Random random)
        {
            var types = Utilities.GetEnumValues<Type>();
            type = types[random.Next(0, types.Length)];
        }
        
        public enum Type
        {
            Bomb
        }
    }
}