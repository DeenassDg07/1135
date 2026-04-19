using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace _1125.Model
{
    public class CustomerOrder
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public ObservableCollection<OrderItem> Items { get; set; } = new ObservableCollection<OrderItem>();
    }

    public class OrderItem
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public byte[] ImageData { get; set; }
    }

    public class ModifiableOrder : INotifyPropertyChanged
    {
        private int _id;
        private string _phone;
        private string _address;
        private DateTime? _deliveryDate;
        private string _status;

        public int Id
        {
            get => _id;
            set { if (_id != value) { _id = value; OnPropertyChanged(); } }
        }

        public string Phone
        {
            get => _phone;
            set { if (_phone != value) { _phone = value; OnPropertyChanged(); } }
        }

        public string Address
        {
            get => _address;
            set { if (_address != value) { _address = value; OnPropertyChanged(); } }
        }

        public DateTime? DeliveryDate
        {
            get => _deliveryDate;
            set { if (_deliveryDate != value) { _deliveryDate = value; OnPropertyChanged(); } }
        }

        public string Status
        {
            get => _status;
            set { if (_status != value) { _status = value; OnPropertyChanged(); } }
        }

        public ObservableCollection<ModifiableOrderItem> Items { get; set; } = new ObservableCollection<ModifiableOrderItem>();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class ModifiableOrderItem : INotifyPropertyChanged
    {
        private string _productName;
        private int _quantity;
        private byte[] _imageData;

        public string ProductName
        {
            get => _productName;
            set { if (_productName != value) { _productName = value; OnPropertyChanged(); } }
        }

        public int Quantity
        {
            get => _quantity;
            set { if (_quantity != value) { _quantity = value; OnPropertyChanged(); } }
        }

        public byte[] ImageData
        {
            get => _imageData;
            set { if (_imageData != value) { _imageData = value; OnPropertyChanged(); } }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
