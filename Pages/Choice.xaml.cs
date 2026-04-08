using ISIP223_Bulatov_WPF.DBResources;
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

namespace ISIP223_Bulatov_WPF.Pages
{
    public partial class ConfiguratorPage : Page
    {
        public ConfiguratorPage()
        {
            InitializeComponent();
            LoadSlots();
            InitializeAssembly();
            UpdateTotal();
        }

        private void InitializeAssembly()
        {
            if (Core.CurrentAssembly != null)
            {
                TbAssemblyName.Text = Core.CurrentAssembly.name;
                TbAuthorName.Text = Core.CurrentAssembly.author;
                foreach (var slot in Core.PartSlots)
                {
                    slot.SelectedPartId = 0;
                    slot.SelectedPartName = "Не выбрано";
                    slot.Price = 0;
                    slot.ImagePath = String.Empty;
                }
                foreach (var partAssembly in Core.Context.partassembly_.Where(pa => pa.assemblyid == Core.CurrentAssembly.id))
                {
                    var slot = Core.PartSlots.FirstOrDefault(s => s.CategoryId == Core.Context.basepart_.FirstOrDefault(p => p.id == partAssembly.partid).parttypeid);
                    if (slot != null)
                    {
                        var part = Core.Context.basepart_.FirstOrDefault(p => p.id == partAssembly.partid);
                        slot.SelectedPartId = part.id;
                        slot.SelectedPartName = part.name;
                        slot.Price = part.price;
                        slot.ImagePath = part.image;
                    }
                }
                PartsItemsControl.ItemsSource = null;
                PartsItemsControl.ItemsSource = Core.PartSlots;
            }
        }

        private void LoadSlots()
        {
            if (Core.PartSlots == null)
            {
                Core.PartSlots = new List<PartSlot>();
                foreach (var partType in Core.Context.parttype_)
                {
                    Core.PartSlots.Add(new PartSlot { CategoryName = partType.name, CategoryId = partType.id });
                }
            }

            PartsItemsControl.ItemsSource = Core.PartSlots;
        }

        private void SelectPart_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            int categoryId = (int)btn.Tag;

            NavigationService.Navigate(new PartSelectionPage(categoryId));
        }

        private void SaveAssembly_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbAssemblyName.Text) || string.IsNullOrWhiteSpace(TbAuthorName.Text))
            {
                MessageBox.Show("Заполните название и автора сборки!");
                return;
            }

            var assembly = new assembly_
            {
                name = TbAssemblyName.Text,
                author = TbAuthorName.Text
            };
            Core.Context.assembly_.Add(assembly);

            foreach (var slot in Core.PartSlots)
            {
                if (slot.SelectedPartId != 0)
                {
                    var partAssembly = new partassembly_
                    {
                        assemblyid = assembly.id,
                        partid = slot.SelectedPartId
                    };
                    Core.Context.partassembly_.Add(partAssembly);
                }
            }
            Core.Context.SaveChanges();
            MessageBox.Show("Сборка успешно сохранена!");
        }

        public void UpdateTotal()
        {
            decimal total = Core.PartSlots.Sum(s => s.Price);
            TxtTotalPrice.Text = $"{total:N2} ₽";

            CheckCompatibility();
        }

        private void CheckCompatibility()
        {
            var result = CompatibilityChecker.CheckCompatibility(Core.PartSlots);
            if (result == "Совместимо")
                TxtCompatibility.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27AE60"));
            else
                TxtCompatibility.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF4545"));

            TxtCompatibility.Text = result;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите очистить конфигуратор? Все выбранные детали будут удалены.", "Подтверждение", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                for (int i = 0; i < Core.PartSlots.Count; i++)
                {
                    Core.PartSlots[i].SelectedPartId = 0;
                    Core.PartSlots[i].SelectedPartName = "Не выбрано";
                    Core.PartSlots[i].Price = 0;
                    Core.PartSlots[i].ImagePath = String.Empty;
                }
                UpdateTotal();
                PartsItemsControl.ItemsSource = null;
                PartsItemsControl.ItemsSource = Core.PartSlots;
                TbAssemblyName.Text = String.Empty;
                TbAuthorName.Text = String.Empty;
            }
        }

        private void ShowPartDetails(basepart_ part)
        {
            if (part == null) return;

            ModalTitle.Text = part.name;
            ModalPrice.Text = $"{part.price:N2} ₽";
            ModalImg.Source = new BitmapImage(new Uri(part.image, UriKind.RelativeOrAbsolute));

            ModalSpecs.ItemsSource = Core.LoadSpecs(part);

            ModalOverlay.Visibility = Visibility.Visible;
        }
        private void PartName_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var textBlock = sender as TextBlock;
            if (textBlock == null || textBlock.Tag == null) return;

            int partId = (int)textBlock.Tag;

            if (partId == 0) return;

            var part = Core.Context.basepart_.FirstOrDefault(p => p.id == partId);

            if (part != null)
            {
                ShowPartDetails(part);
            }

        }

        private void CloseModal_Click(object sender, RoutedEventArgs e)
        {
            ModalOverlay.Visibility = Visibility.Collapsed;
        }
    }

}