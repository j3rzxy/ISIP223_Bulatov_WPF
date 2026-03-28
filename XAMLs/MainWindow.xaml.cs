using ISIP223_Bulatov.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ISIP223_Bulatov_WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var game = new Game();
            game.Start();
        }
        internal class Game
        {
            private Player player = new Player();
            private Random rand = new Random();
            private int turn = 1;
            //Factories
            private readonly IItemFactory _itemFactory = new ChestItemFactory();
            private readonly IEnemyFactory _enemyFactory = new EnemyFactory();
            private readonly IEnemyFactory _bossFactory = new BossFactory();

            public void Start()
            {
                Console.WriteLine("Добро пожаловать в текстовую RPG!");
                while (player.HP > 0)
                {
                    Console.WriteLine();
                    if (turn % 10 == 0)
                    {
                        Fight(_bossFactory.CreateRandomEnemy()); //BossFactory
                    }
                    else
                    {
                        if (rand.Next(2) == 0)
                        {
                            Chest();
                        }
                        else
                        {
                            Fight(_enemyFactory.CreateRandomEnemy()); //EnemyFactory
                        }
                    }
                    turn++;
                    Console.WriteLine($"Ваше здоровье: {player.HP}/{player.MaxHP}");
                    Console.WriteLine("Нажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                    Console.Clear();
                }

                Console.WriteLine("\nВы проиграли! Игра окончена.");
            }

            private void Chest()
            {
                Console.WriteLine("\nВы нашли сундук!");
                var item = _itemFactory.CreateRandomItem(); // Создание через фабрику

                if (item.Name == "Зелье здоровья")
                {
                    Console.WriteLine("\nВы нашли зелье! Полностью исцелены.");
                    player.Heal();
                }
                else
                {
                    Console.WriteLine($"Вы нашли: {item.Name}");
                    Console.WriteLine($"Атака: {item.Attack}, Защита: {item.Defense}");

                    // Вывод характеристик текущей экипировки
                    if (item.Attack > 0) // Если предмет — оружие
                    {
                        Console.WriteLine($"Текущее оружие: {player.Weapon.Name}");
                        Console.WriteLine($"Атака: {player.Weapon.Attack}, Защита: {player.Weapon.Defense}");
                    }
                    else if (item.Defense > 0) // Если предмет — доспехи
                    {
                        Console.WriteLine($"Текущие доспехи: {player.Armor.Name}");
                        Console.WriteLine($"Атака: {player.Armor.Attack}, Защита: {player.Armor.Defense}");
                    }

                    Console.Write("\nВзять предмет? (Y/N): ");
                    if (Console.ReadKey().KeyChar.ToString().ToLower() == "y")
                    {
                        if (item.Attack > 0)
                        {
                            player.Weapon = item;
                            Console.WriteLine($"\nВы экипировали {item.Name}.");
                        }
                        else
                        {
                            player.Armor = item;
                            Console.WriteLine($"\nВы экипировали {item.Name}.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("\nПредмет выброшен.");
                    }
                }
            }
            private void Fight(Enemy enemy)
            {
                if (enemy.HP <= 0)
                {
                    enemy.HP = enemy.MaxHP; // Автоисправление
                }
                Console.WriteLine($"Вы встретили врага {enemy.Name}!");
                while (enemy.HP > 0 && player.HP > 0)
                {
                    if (player.IsFrozen)
                    {
                        Console.WriteLine("Вы заморожены и пропускаете ход!");
                        player.IsFrozen = false;
                    }
                    else
                    {
                        Console.Write("Выберите действие (A - атаковать, D - защищаться): ");
                        var action = Console.ReadKey().KeyChar.ToString().ToLower();
                        Console.WriteLine();

                        if (action == "a")
                        {
                            if (enemy.Type == "Slug")
                            {
                                int damage = player.GetTotalAttack();
                                damage -= 2;
                                enemy.HP -= damage;
                                Console.WriteLine($"Вы атаковали! Нанесли {damage} урона. У врага осталось {enemy.HP} HP.");
                            }
                            else
                            {
                                int damage = player.GetTotalAttack();
                                enemy.HP -= damage;
                                Console.WriteLine($"Вы атаковали! Нанесли {damage} урона. У врага осталось {enemy.HP} HP.");
                            }
                        }
                        else if (action == "d")
                        {
                            Console.WriteLine("Вы защищаетесь.");
                        }
                        if (enemy.HP <= 0)
                        {
                            Console.WriteLine($"Вы победили {enemy.Name}!");
                            return;
                        }

                        // Ход врага
                        int blockChance = rand.Next(100);
                        bool isDodged = false;
                        if (rand.Next(100) < 40) // 40% шанс уклониться
                        {
                            Console.WriteLine("Вы уклонились от атаки!");
                            isDodged = true;
                        }

                        if (!isDodged)
                        {
                            int damage = enemy.Attack;
                            if (enemy.Type == "Skeleton")
                            {
                                // Скелет игнорирует защиту
                            }
                            else
                            {
                                int block = rand.Next(70, 101) * player.GetTotalDefense() / 100;
                                damage = Math.Max(1, damage - block);
                            }

                            player.HP -= damage;
                            Console.WriteLine($"{enemy.Name} атакует! Вы получили {damage} урона.");
                        }

                        // Особенности врага
                        if (enemy.Type == "Goblin")
                        {
                            if (rand.Next(100) < 20) // 20% шанс крита
                            {
                                Console.WriteLine("Гоблин наносит критический удар!");
                                player.HP -= enemy.Attack;
                                Console.WriteLine($"Вы получили дополнительный урон! Осталось HP: {player.HP}");
                            }
                        }
                        else if (enemy.Type == "Wizard")
                        {
                            if (rand.Next(100) < 20) // 20% шанс заморозки
                            {
                                Console.WriteLine("Маг заморозил вас! Вы пропускаете следующий ход.");
                                player.IsFrozen = true;
                            }
                        }

                        if (player.HP <= 0)
                        {
                            Console.WriteLine("Вы погибли...");
                            return;
                        }
                    }
                }
            }
        }
    }
}