using _1125.DB;
using _1125.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace _1125.ViewModel
{
    public class OrderItemViewModel : OrderItem, INotifyPropertyChanged
    {
        public ImageSource? ProductImage
        {
            get
            {
                if (ImageData != null && ImageData.Length > 0)
                {
                    using var stream = new MemoryStream(ImageData);
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream;
                    image.EndInit();
                    image.Freeze();
                    return image;
                }
                return null;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class CustomerOrderViewModel : INotifyPropertyChanged
    {
        private readonly CustomerOrder _order;

        public CustomerOrderViewModel(CustomerOrder order)
        {
            _order = order;
            Items = new ObservableCollection<OrderItemViewModel>(
                order.Items.Select(i => new OrderItemViewModel
                {
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    ImageData = i.ImageData
                }));
        }

        public int Id => _order.Id;
        public string Status => _order.Status;
        public string Phone => _order.Phone;
        public string Address => _order.Address;
        public string DeliveryDateStr => _order.DeliveryDate?.ToString("dd.MM.yyyy") ?? "Дата не указана";
        public ObservableCollection<OrderItemViewModel> Items { get; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class MyOrderViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<CustomerOrderViewModel> Orders { get; set; } = new();

        private CustomerOrderViewModel? _selectedOrder;
        public CustomerOrderViewModel? SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                if (_selectedOrder != value)
                {
                    _selectedOrder = value;
                    OnPropertyChanged();
                }
            }
        }

        public MyOrderViewModel()
        {
            LoadOrdersFromDatabase();
        }

        private void LoadOrdersFromDatabase()
        {
            int userId = User.Current.Id;
            var customerOrders = MyOrderDB.GetDb().GetOrdersByUserId(userId);

            Orders.Clear();
            foreach (var order in customerOrders)
            {
                Orders.Add(new CustomerOrderViewModel(order));
            }

            SelectedOrder = Orders.FirstOrDefault();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
