using _1125.VMTools;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using _1125.Model;
using _1125.View;
using _1125.DB;
using System.Windows;

namespace _1125.ViewModel
{
    internal class ProductsVM : BaseVM
    {
        private Action _close;
        private ObservableCollection<Product> _products;
        private Product _selectedProduct;
        private string _searchText;
        private readonly string _productType;
        private readonly DispatcherTimer _typingTimer;

        public ICommand BackCommand { get; private set; }
        public ICommand EditProductCommand { get; private set; }
        public ICommand Baket { get; private set; }
        public ICommand AddProductCommand { get; private set; }
        public ICommand AddToCartCommand { get; private set; }
        public ICommand EditOrders { get; private set; }

        public ICommand MyOrders { get; private set; }

        public bool IsUserRole => User.Current?.Role == "user";
        public Visibility UserOnlyVisibility => User.Current?.Role == "user" ? Visibility.Visible : Visibility.Collapsed;



        public ObservableCollection<Product> Products
        {
            get => _products;
            private set
            {
                _products = value;
                Signal();
            }
        }

        private string _selectedSort;
        public string SelectedSort
        {
            get => _selectedSort;
            set
            {
                if (_selectedSort != value)
                {
                    _selectedSort = value;
                    Signal(); 

                    ApplySorting();
                }
            }
        }

        public Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                Signal();  
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    Signal(); 
                    _typingTimer.Stop();
                    _typingTimer.Start();
                }
            }
        }

        public bool CanEditProduct => User.Current?.Role == "director";

        private bool _canBasket;
        public bool CanBasket
        {
            get => _canBasket;
            set
            {
                if (_canBasket != value)
                {
                    _canBasket = value;
                    Signal(); 
                    (Baket as CommandVM)?.RaiseCanExecuteChanged();
                }
            }
        }

        

        public ProductsVM(string productType, bool isGuest = false)
        {
            _productType = !string.IsNullOrEmpty(productType) ? productType : "All";

            _typingTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            _typingTimer.Tick += TypingTimer_Tick;

            InitializeCommands();

            CanBasket = !isGuest && User.Current?.Role != "guest";

            User.CurrentChanged += (s, e) =>
            {
                CanBasket = User.Current?.Role != "guest";
            };
            LoadProducts();
        }

        private void TypingTimer_Tick(object sender, EventArgs e)
        {
            _typingTimer.Stop();
            LoadProducts();
        }

        private void InitializeCommands()
        {
            BackCommand = new CommandVM(() =>
            {
               
                _close?.Invoke();
            });

            Baket = new CommandVM(() =>
            {
                var basketWindow = new BasketWindow();
                basketWindow.ShowDialog();
            }, () => CanBasket);

            EditProductCommand = new CommandVM(() =>
            {
                if (SelectedProduct != null)
                {
                    var win = new EditingAddingWindow(SelectedProduct);
                    win.ShowDialog();
                    LoadProducts();
                }
            }, () => CanEditProduct);

            AddProductCommand = new CommandVM(() =>
            {
                var win = new EditingAddingWindow(null);
                win.ShowDialog();
                LoadProducts(); 
            }, () => CanEditProduct);


            EditOrders = new CommandVM(() =>
            {
                var win = new EditingOrderWindow();
                win.ShowDialog();
            }, () => CanEditProduct);

            MyOrders = new CommandVM(() =>
            {
                var win = new MyOrdersWindow();
                win.ShowDialog();
            }, () => User.Current?.Role == "user" || User.Current?.Role == "director");

            AddToCartCommand = new CommandParamVM<Product>((product) =>
            {
                if (User.Current != null)
                {
                    CartDB.GetDb().InsertIntoCart(product, User.Current);
                    MessageBox.Show("Товар добавлен в корзину.");
                }
            }, (product) => CanBasket);



        }

        public void LoadProducts()
        {
            var list = ProductDB.GetDb().SelectAll();

            if (!_productType.Equals("All", StringComparison.OrdinalIgnoreCase))
                list = list.Where(p => p.CategoryName .Equals(_productType, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrEmpty(_searchText))
            {
                var f = _searchText;
                list = list
                    .Where(p => p.Name.Contains(f, StringComparison.OrdinalIgnoreCase)
                             || p.Description.Contains(f, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            Products = new ObservableCollection<Product>(list);

            ApplySorting();
        }

        private void ApplySorting()
        {
            if (Products == null || string.IsNullOrEmpty(SelectedSort)) return;

             ObservableCollection<Product> sorted;

            switch (SelectedSort)
            {
                case "Недорогие":
                    sorted = new ObservableCollection<Product>(Products.OrderBy(p => p.Price));
                     break;
                case "Дорогие":
                    sorted = new ObservableCollection<Product>(Products.OrderByDescending(p => p.Price));
                     break;
                case "Популярные":
                    sorted = new ObservableCollection<Product>(Products.OrderBy(p => p.Availability));
                    break;
                case "Менее популярные":
                    sorted = new ObservableCollection<Product>(Products.OrderByDescending(p => p.Availability));
                    break;
                default:
                    sorted = new ObservableCollection<Product>(Products);
                    break;
            }

            Products = sorted;
        } 

        internal void SetClose(Action close) => _close = close;
    }
}
