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
    /// <summary>
    /// Логика взаимодействия для AssembliesListPage.xaml
    /// </summary>
    public partial class AssembliesListPage : Page
    {
        public AssembliesListPage()
        {
            InitializeComponent();
            RefreshData();
        }
        private void RefreshData()
        {
            try
            {
                LbAssemblies.ItemsSource = null;
                var list = Core.Context.assembly_.ToList();

                LbAssemblies.ItemsSource = list;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }
        }

        private void BtnView_Click(object sender, RoutedEventArgs e)
        {
            var selectedAssembly = (sender as Button).DataContext as assembly_;

            if (selectedAssembly != null)
            {
                Core.CurrentAssembly = selectedAssembly;
                NavigationService.Navigate(new ConfiguratorPage());
            }
        }
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите удалить эту сборку?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                var assemblyId = (int)(sender as Button).Tag;
                var ssembly = Core.Context.assembly_.FirstOrDefault(a => a.id == assemblyId);
                var assemplyParts = Core.Context.partassembly_.Where(pa => pa.assemblyid == assemblyId).ToList();
                foreach (var part in assemplyParts)
                {
                    Core.Context.partassembly_.Remove(part);
                }
                Core.Context.assembly_.Remove(ssembly);
                Core.Context.SaveChanges();
            }
            RefreshData();
        }

    }
}
