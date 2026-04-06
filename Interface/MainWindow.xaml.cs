using ISIP223_Bulatov.Factories;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace ISIP223_Bulatov_WPF
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // --- Game State Fields ---
        private Player _player;
        private Enemy _currentEnemy;
        private Random _rand = new Random();
        private int _turn = 1;
        private bool _isDefending;
        private bool _isGameActive;
        private bool _encounterInProgress;

        // --- Factories ---
        private readonly IItemFactory _itemFactory = new ChestItemFactory();
        private readonly IEnemyFactory _enemyFactory = new EnemyFactory();
        private readonly IEnemyFactory _bossFactory = new BossFactory();

        // --- UI Bindable Properties ---
        public string PlayerName { get; set; } = "Герой";

        private double _playerHP;
        public double PlayerHP
        {
            get => _playerHP;
            set { _playerHP = value; OnPropertyChanged(); UpdatePlayerHPText(); }
        }

        private double _playerMaxHP = 100;
        public double PlayerMaxHP
        {
            get => _playerMaxHP;
            set { _playerMaxHP = value; OnPropertyChanged(); UpdatePlayerHPText(); }
        }

        public string PlayerHPText { get; private set; } = "HP: 100/100";

        private bool _isFrozen;
        public bool IsFrozen
        {
            get => _isFrozen;
            set { _isFrozen = value; OnPropertyChanged(); }
        }

        public string PlayerStatsText => $"ATK: {_player?.GetTotalAttack() ?? 0} | DEF: {_player?.GetTotalDefense() ?? 0}";
        public string PlayerWeaponName => _player?.Weapon?.Name ?? "Кулаки";
        public string PlayerArmorName => _player?.Armor?.Name ?? "Тряпьё";

        private string _currentEnemyName;
        public string CurrentEnemyName
        {
            get => _currentEnemyName;
            set { _currentEnemyName = value; OnPropertyChanged(); }
        }

        private double _currentEnemyHP;
        public double CurrentEnemyHP
        {
            get => _currentEnemyHP;
            set { _currentEnemyHP = value; OnPropertyChanged(); UpdateEnemyHPText(); }
        }

        private double _currentEnemyMaxHP;
        public double CurrentEnemyMaxHP
        {
            get => _currentEnemyMaxHP;
            set { _currentEnemyMaxHP = value; OnPropertyChanged(); UpdateEnemyHPText(); }
        }

        public string CurrentEnemyHPText { get; private set; } = "HP: 0/0";

        private string _currentEnemyType;
        public string CurrentEnemyType
        {
            get => _currentEnemyType;
            set { _currentEnemyType = value; OnPropertyChanged(); }
        }

        private bool _isGameOver;
        public bool IsGameOver
        {
            get => _isGameOver;
            set { _isGameOver = value; OnPropertyChanged(); }
        }

        private string _gameOverMessage;
        public string GameOverMessage
        {
            get => _gameOverMessage;
            set { _gameOverMessage = value; OnPropertyChanged(); }
        }

        public bool IsGameActive
        {
            get => _isGameActive;
            set { _isGameActive = value; OnPropertyChanged(); }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            StartNewGame();
        }

        private void StartNewGame()
        {
            _player = new Player();
            _rand = new Random();
            _turn = 1;
            _isDefending = false;
            _encounterInProgress = false;

            PlayerHP = _player.HP;
            PlayerMaxHP = _player.MaxHP;
            IsFrozen = false;
            IsGameOver = false;
            IsGameActive = true;
            LogList.Items.Clear();

            AddLog("=== НОВАЯ ИГРА ===");
            AddLog("Добро пожаловать в КИПФИН Рогалик!");
            NextTurnSetup();
        }

        private void NextTurnSetup()
        {
            if (_player.HP <= 0)
            {
                EndGame(false);
                return;
            }

            if (_encounterInProgress) return;

            _encounterInProgress = true;

            if (_turn % 10 == 0)
            {
                SpawnEnemy(_bossFactory.CreateRandomEnemy(), true);
            }
            else
            {
                if (_rand.Next(2) == 0)
                {
                    SpawnChest();
                }
                else
                {
                    SpawnEnemy(_enemyFactory.CreateRandomEnemy(), false);
                }
            }
            _turn++;
        }

        private void SpawnEnemy(Enemy enemy, bool isBoss)
        {
            _currentEnemy = enemy;
            if (_currentEnemy.HP <= 0) _currentEnemy.HP = _currentEnemy.MaxHP;

            CurrentEnemyName = _currentEnemy.Name + (isBoss ? " (БОСС)" : "");
            CurrentEnemyMaxHP = _currentEnemy.MaxHP;
            CurrentEnemyHP = _currentEnemy.HP;
            CurrentEnemyType = _currentEnemy.Type;

            AddLog($"\n⚠️ Встречен враг: {_currentEnemy.Name}!");
            _isDefending = false;

        }

        private void SpawnChest()
        {
            _currentEnemy = null;
            CurrentEnemyName = "🎁 Сундук";
            CurrentEnemyHP = 0;
            CurrentEnemyMaxHP = 1;
            CurrentEnemyType = "Chest";

            AddLog("\n🎁 Вы нашли сундук!");
            var item = _itemFactory.CreateRandomItem();

            if (item.Name == "Зелье здоровья")
            {
                AddLog("✨ Внутри зелье! Вы полностью исцелены.");
                _player.Heal();
                PlayerHP = _player.HP;
                AddLog($"💚 Ваше здоровье: {PlayerHP:F0}/{PlayerMaxHP:F0}");
            }
            else
            {
                AddLog($"Предмет: {item.Name} (ATK:{item.Attack} DEF:{item.Defense})");

                string currentGear = item.Attack > 0
                    ? $"Текущее: {_player.Weapon.Name}"
                    : $"Текущее: {_player.Armor.Name}";
                AddLog(currentGear);

                bool isWeapon = item.Attack > 0;
                bool isBetter = isWeapon
                    ? item.Attack > _player.Weapon.Attack
                    : item.Defense > _player.Armor.Defense;

                if (isBetter)
                {
                    if (isWeapon) { _player.Weapon = item; AddLog($"⚔️ Экипировано: {item.Name}"); }
                    else { _player.Armor = item; AddLog($"🛡️ Экипировано: {item.Name}"); }
                    OnPropertyChanged(nameof(PlayerWeaponName));
                    OnPropertyChanged(nameof(PlayerArmorName));
                    OnPropertyChanged(nameof(PlayerStatsText));
                }
                else
                {
                    AddLog("Предмет хуже текущего, проигнорирован.");
                }
            }
            EndEncounter();
        }

        private void EndEncounter()
        {
            _encounterInProgress = false;
            _currentEnemy = null;

            NextTurnSetup();
        }

        // --- BUTTON HANDLERS ---

        private void BtnAttack_Click(object sender, RoutedEventArgs e)
        {

            if (!IsGameActive || !_encounterInProgress || _currentEnemy == null) return;


            if (IsFrozen)
            {
                AddLog("❄️ Вы заморожены и пропускаете ход!");
                IsFrozen = false;

                EnemyTurn();
                return;
            }

            // Логика атаки
            int damage = _player.GetTotalAttack();
            if (_currentEnemy.Type == "Slug") damage = Math.Max(1, damage - 2);

            _currentEnemy.HP -= damage;
            CurrentEnemyHP = _currentEnemy.HP;
            AddLog($"⚔️ Вы атаковали! Нанесено {damage} урона.");

            if (_currentEnemy.HP <= 0)
            {
                AddLog($"🏆 Вы победили {_currentEnemy.Name}!");
                EndEncounter();
                return;
            }

            EnemyTurn();
        }

        private void BtnDefend_Click(object sender, RoutedEventArgs e)
        {
            if (!IsGameActive || !_encounterInProgress || _currentEnemy == null) return;

            _isDefending = true;
            AddLog("🛡️ Вы перешли в оборону (снижение урона).");
            EnemyTurn();
        }


        private void BtnRestart_Click(object sender, RoutedEventArgs e)
        {
            StartNewGame();
        }

        // --- COMBAT LOGIC ---

        private void EnemyTurn()
        {
            if (_currentEnemy == null || _currentEnemy.HP <= 0) return;

            if (_rand.Next(100) < 40)
            {
                AddLog("✨ Вы уклонились от атаки!");
                return;
            }

            int damage = _currentEnemy.Attack;

            if (_currentEnemy.Type != "Skeleton")
            {
                if (_isDefending)
                {
                    damage = Math.Max(1, damage / 2);
                    AddLog("🛡️ Оборона снизила входящий урон!");
                }
                else
                {
                    int block = _rand.Next(70, 101) * (int)_player.GetTotalDefense() / 100;
                    damage = Math.Max(1, damage - block);
                }
            }

            _player.HP -= damage;
            PlayerHP = _player.HP;
            AddLog($"👹 {_currentEnemy.Name} атакует! Вы получили {damage} урона.");

            if (_currentEnemy.Type == "Goblin" && _rand.Next(100) < 20)
            {
                int crit = _currentEnemy.Attack;
                _player.HP -= crit;
                PlayerHP = _player.HP;
                AddLog($"💥 Критический удар гоблина! Еще -{crit} HP!");
            }
            else if (_currentEnemy.Type == "Wizard" && _rand.Next(100) < 20)
            {
                IsFrozen = true;
                AddLog("🧙‍♂️ Маг заморозил вас! Следующий ход пропущен.");
            }

            _isDefending = false;  
            if (_player.HP <= 0)
            {
                _player.HP = 0;
                PlayerHP = 0;
                EndGame(false);
                return;
            }
        }


        private void EndGame(bool win)
        {
            IsGameActive = false;
            IsGameOver = true;
            _encounterInProgress = false; 
            GameOverMessage = win ? "ПОБЕДА!" : "ВЫ ПОГИБЛИ";
            AddLog(win ? "\n🎉 Игра пройдена!" : "\n💀 Игра окончена...");
        }

        // --- UI HELPERS ---

        private void AddLog(string message)
        {
            Application.Current.Dispatcher.Invoke(() => {
                var tb = new TextBlock
                {
                    Text = message,
                    Margin = new Thickness(0, 2, 0, 0),
                    TextWrapping = TextWrapping.Wrap
                };
                LogList.Items.Add(tb);
                // Прокрутка вниз
                LogScroll.ScrollToEnd();
            });
        }

        private void UpdatePlayerHPText()
        {
            PlayerHPText = $"HP: {PlayerHP:F0}/{PlayerMaxHP:F0}";
            OnPropertyChanged(nameof(PlayerHPText));
        }

        private void UpdateEnemyHPText()
        {
            CurrentEnemyHPText = $"HP: {CurrentEnemyHP:F0}/{CurrentEnemyMaxHP:F0}";
            OnPropertyChanged(nameof(CurrentEnemyHPText));
        }

        // --- INotifyPropertyChanged ---
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}