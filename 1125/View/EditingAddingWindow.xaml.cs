using _1125.Model;
using _1125.ViewModel;
using System.Windows;

namespace _1125.View
{
    /// <summary>
    /// Логика взаимодействия для EditingAddingWindow.xaml
    /// </summary>
    public partial class EditingAddingWindow : Window
    {
        // Конструктор для создания нового товара
        public EditingAddingWindow() : this(null) { }

        // Конструктор для редактирования переданного товара
        public EditingAddingWindow(Product product)
        {
            InitializeComponent();
            var vm = new EditingAddingWindowViewModel(product);
            vm.Close = () => this.DialogResult = true;
            DataContext = vm;
        }
    }
}
