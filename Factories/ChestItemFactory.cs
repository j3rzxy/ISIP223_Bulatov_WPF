namespace ISIP223_Bulatov.Factories
{
    internal class ChestItemFactory : IItemFactory
    {
        private readonly Random _random = new Random();
        private readonly Item[] _items = new Item[]
        {
            new() { Name = "Зелье здоровья", Attack = 0, Defense = 0 },
            new() { Name = "Острый меч", Attack = 15, Defense = 0 },
            new() { Name = "Кольчуга", Attack = 0, Defense = 25 },
            new() { Name = "Клинок дракона", Attack = 30, Defense = 0 },
            new() { Name = "Доспехи Легиона", Attack = 0, Defense = 40 }
        };
        public Item CreateRandomItem() => _items[_random.Next(_items.Length)];
    }
}