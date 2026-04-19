using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using _1125.DB;
using _1125.Model;
using _1125.View;
using _1125.VMTools;

namespace _1125.ViewModel
{
    public partial class CategoryVM : BaseVM
    {
        
        public ICommand ProductsVacuumСleaner { get; set; }
        public ICommand ProductsMicrowave { get; set; }
        public ICommand ProductsFridge { get; set; }
        public ICommand ProductsTV { get; set; }
        public ICommand ProductsTelephone { get; set; }
        public ICommand ProductsAll { get; set; }


        public CategoryVM()
        {
            ProductsVacuumСleaner = new CommandVM(() =>
            {
                ProductsWindow productsWindow = new ProductsWindow("Пылесос");
                close?.Invoke();
                productsWindow.ShowDialog();
            }, () => true);

            ProductsMicrowave = new CommandVM(() =>
            {
                ProductsWindow productsWindow = new ProductsWindow("Микроволновка");
                close?.Invoke();
                productsWindow.ShowDialog();
            }, () => true);

            ProductsFridge = new CommandVM(() =>
            {
                ProductsWindow productsWindow = new ProductsWindow("Холодильник");
                 close?.Invoke();
                productsWindow.ShowDialog();
            }, () => true);

            ProductsTV = new CommandVM(() =>
            {
                ProductsWindow productsWindow = new ProductsWindow("Телевизор");
                close?.Invoke();
                productsWindow.ShowDialog();
            }, () => true);

            ProductsTelephone = new CommandVM(() =>
            {
                ProductsWindow productsWindow = new ProductsWindow("Телефон");
                close?.Invoke();
                productsWindow.ShowDialog();
            }, () => true);

            ProductsAll = new CommandVM(() =>
            {
                ProductsWindow productsWindow = new ProductsWindow("All");
                close?.Invoke();
                productsWindow.ShowDialog();
            }, () => true);

        }
        public static string CurrentUserRole = "director"; 

        private void NavigateButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUserRole == "director")
            {
               
                var editingAddingWindow = new EditingAddingWindow();
                editingAddingWindow.Show();
            }
            else if (CurrentUserRole == "user")
            {
                
                var productsWindow = new ProductsWindow("");
                productsWindow.Show();
            }

        }
        Action close;
        internal void SetClose(Action close)
        {
            this.close = close;
        }
    }
}

    


