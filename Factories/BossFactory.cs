using System;

namespace ISIP223_Bulatov.Factories
{
    internal class BossFactory : IEnemyFactory
    {
        private readonly Random _random = new Random();
        private readonly Enemy[] _bosses = new Enemy[]
        {
            new Boss { Name = "ВВГ", Type = "Goblin", HP = 60, MaxHP = 60, Attack = 18, Defense = 3, CritChanceBonus = 10 },
            new Boss { Name = "Ковальский", Type = "Skeleton", HP = 62, MaxHP = 62, Attack = 13, Defense = 7 },
            new Boss { Name = "Архимаг C++", Type = "Wizard", HP = 36, MaxHP = 36, Attack = 14, Defense = 2, FreezeChanceBonus = 10 },
            new Boss { Name = "Пестов С--", Type = "Skeleton", HP = 32, MaxHP = 32, Attack = 18, Defense = 3, FreezeChanceBonus = 15 }
        };
        public Enemy CreateRandomEnemy() => _bosses[_random.Next(_bosses.Length)];
    }
}