using System.Windows;
using _1125.ViewModel;

namespace _1125.View
{
    public partial class CategoryWindow : Window
    {
        public CategoryWindow()
        {
            InitializeComponent();
            DataContext = new CategoryVM();
        }
    }
}
