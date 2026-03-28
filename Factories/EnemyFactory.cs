namespace ISIP223_Bulatov.Factories
{
    internal class EnemyFactory : IEnemyFactory
    {
        private readonly Random _random = new Random();
        private readonly Enemy[] _enemies = new Enemy[]
        {
            new Enemy { Name = "Гоблин", Type = "Goblin", HP = 30, MaxHP = 30, Attack = 12, Defense = 3 },
            new Enemy { Name = "Скелет", Type = "Skeleton", HP = 25, MaxHP = 25, Attack = 10, Defense = 5 },
            new Enemy { Name = "Маг", Type = "Wizard", HP = 20, MaxHP = 20, Attack = 9, Defense = 2 },
            new Enemy { Name = "Слизень", Type = "Slug", HP = 15, MaxHP = 15, Attack = 11, Defense = 6 }
        };
        public Enemy CreateRandomEnemy() => _enemies[_random.Next(_enemies.Length)];
    }
}