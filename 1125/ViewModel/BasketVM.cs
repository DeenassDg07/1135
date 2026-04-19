using _1125.DB;
using _1125.Model;
using _1125.View;
using _1125.VMTools;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace _1125.ViewModel
{
    public class BasketVM : BaseVM
    {
        public ObservableCollection<CartItem> CartItems { get; set; }

        public ICommand Back2 { get; set; }
        public ICommand Order { get; }
        public ICommand IncreaseCommand { get; }
        public ICommand DecreaseCommand { get; }
        public ICommand RemoveCommand { get; }

        private string tel;
        public string Tel
        {
            get => tel;
            set
            {
                tel = value;
                Signal();
            }
        }

        private string address;
        public string Address
        {
            get => address;
            set
            {
                address = value;
                Signal();
            }
        }


        private bool CanCreateOrder()
        {
            foreach (var item in CartItems)
            {
                if (item.Quantity > item.Product.Availability)
                {
                    MessageBox.Show($"Недостаточно товара '{item.Product.Name}'. Доступно: {item.Product.Availability}, выбрано: {item.Quantity}");
                    return false;
                }
            }
            return true;
        }


        public BasketVM()
        {
            CartItems = CartDB.GetDb().GetCartItems(User.Current.Id);

            foreach (var item in CartItems)
                item.PropertyChanged += CartItem_PropertyChanged;

            Back2 = new CommandVM(() =>
            {
                //CategoryWindow categoryWindow = new CategoryWindow();
                close?.Invoke();
                //categoryWindow.ShowDialog();
            });

            IncreaseCommand = new CommandParamVM<CartItem>((item) =>
            {
                if (item.Quantity < item.Product.Availability)
                {
                    item.Quantity++;
                    CartDB.GetDb().UpdateCartQuantity(User.Current.Id, item.Product.Id, item.Quantity);
                }
                else
                {
                    MessageBox.Show($"Доступно только {item.Product.Availability} шт для товара {item.Product.Name}");
                }
            });

            DecreaseCommand = new CommandParamVM<CartItem>((item) =>
            {
                if (item.Quantity > 1)
                {
                    item.Quantity--;
                    CartDB.GetDb().UpdateCartQuantity(User.Current.Id, item.Product.Id, item.Quantity);
                }
            });

            RemoveCommand = new CommandParamVM<CartItem>((item) =>
            {
                CartDB.GetDb().RemoveFromCart(User.Current.Id, item.Product.Id);
                item.PropertyChanged -= CartItem_PropertyChanged;
                CartItems.Remove(item);
                Signal(nameof(TotalPrice));
            });

            Order = new CommandVM(() =>
            {
                if (!CanCreateOrder())
                    return;
                string tel = Tel;
                string address = Address;

                if (string.IsNullOrWhiteSpace(Tel))
                {
                    MessageBox.Show("Необходимо указать контактный телефон");
                    return;
                }
                if (string.IsNullOrWhiteSpace(Address))
                {
                    MessageBox.Show("Необходимо указать адрес доставки");
                    return;
                }





                var orderWindow = new OrderWindow();
                var orderVM = new OrderVM();
                orderWindow.DataContext = orderVM;

                orderVM.SetClose((result) =>
                {
                    orderWindow.DialogResult = result;
                    orderWindow.Close();
                });

                bool? dialogResult = orderWindow.ShowDialog();
       

                if (dialogResult == true)
                {
                    int orderId = CartDB.GetDb().CreateOrder(User.Current.Id, orderVM.DeliveryDate,tel,address);

                    if (orderId > 0)
                    {
                        CartDB.GetDb().ClearCart(User.Current.Id);

                        CartItems.Clear();

                        Signal(nameof(TotalPrice));

                        Tel = string.Empty;
                        Address = string.Empty;
                    }
      
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window is ProductsWindow productsWindow &&
                            productsWindow.DataContext is ProductsVM productsVM)
                        {
                            productsVM.LoadProducts();
                            break;
                        }
                    }
                }
            });


        }

        private void CartItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CartItem.Quantity))
                Signal(nameof(TotalPrice));
        }

        public decimal TotalPrice => CartItems.Sum(x => x.Product.Price * x.Quantity);

        Action close;
        internal void SetClose(Action close) => this.close = close;
    }
}
