namespace ISIP223_Bulatov.Factories
{
    internal class Player
    {
        public double HP { get; set; } = 100;
        public double MaxHP { get; set; } = 100;
        public Item Weapon { get; set; } = new Item { Name = "Железный меч", Attack = 10 };
        public Item Armor { get; set; } = new Item { Name = "Железные доспехи", Defense = 15 };
        public bool IsFrozen { get; set; } = false;

        public void Heal() => HP = MaxHP;
        public int GetTotalAttack() => Weapon?.Attack ?? 0;
        public int GetTotalDefense() => Armor?.Defense ?? 0;
    }
}
