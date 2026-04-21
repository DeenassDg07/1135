using _1125.ViewModel;
using System.Windows;

namespace _1125.View
{
    public partial class BasketWindow : Window
    {
        public BasketWindow()
        {
            InitializeComponent();

            var vm = new BasketVM();
            vm.SetClose(() => this.Close());  // чтобы команда Back2 могла закрыть окно
            this.DataContext = vm;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
