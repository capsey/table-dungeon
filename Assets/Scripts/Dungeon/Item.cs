using System;

namespace TableDungeon.Dungeon
{
    public enum Item
    {
        Bomb, Spell
    }
    
    public class Chest
    {
        public readonly Item item;
        public bool looted = false;

        public Chest(Random random)
        {
            var items = Utilities.GetEnumValues<Item>();
            item = items[random.Next(0, items.Length)];
        }
    }

    public class Trap
    {
        public readonly Item item;
        public readonly bool player1;

        public Trap(Item item, bool player1)
        {
            this.item = item;
            this.player1 = player1;
        }
    }
}