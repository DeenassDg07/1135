using _1125.DB;
using _1125.Model;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace _1125.ViewModel
{
    public class EditableOrderViewModel : INotifyPropertyChanged
    {
        private readonly EditOrder _db = EditOrder.GetDb();
        public event Action RequestClose;

        public ObservableCollection<int> OrderIds { get; set; } = new ObservableCollection<int>();

        public ObservableCollection<string> Statuses { get; set; } = new ObservableCollection<string>
        {
            "Ожидает подтверждения",
            "Ожидает",
            "В пути",
            "Доставлен",
            "Отменён"
        };

        private int _selectedOrderId;
        public int SelectedOrderId
        {
            get => _selectedOrderId;
            set
            {
                if (_selectedOrderId != value)
                {
                    _selectedOrderId = value;
                    LoadOrderDetails(value);
                    OnPropertyChanged();
                }
            }
        }

        private EditableOrder _selectedEditableOrder;
        public EditableOrder SelectedEditableOrder
        {
            get => _selectedEditableOrder;
            set
            {
                if (_selectedEditableOrder != value)
                {
                    if (_selectedEditableOrder != null)
                        _selectedEditableOrder.PropertyChanged -= SelectedEditableOrder_PropertyChanged;

                    _selectedEditableOrder = value;

                    if (_selectedEditableOrder != null)
                        _selectedEditableOrder.PropertyChanged += SelectedEditableOrder_PropertyChanged;

                    OnPropertyChanged();
                }
            }
        }

        private void SelectedEditableOrder_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Можно обработать изменения в заказе, если нужно
            if (e.PropertyName == nameof(EditableOrder.Status))
            {
                // Например, логика при изменении статуса
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public EditableOrderViewModel()
        {
            LoadOrderIds();

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void LoadOrderIds()
        {
            OrderIds.Clear();
            foreach (var id in _db.GetAllId())
                OrderIds.Add(id);

            if (OrderIds.Count > 0)
                SelectedOrderId = OrderIds[0];
        }

        private void LoadOrderDetails(int id)
        {
            var order = _db.GetOrder(id);
            if (order != null)
            {
                if (!Statuses.Contains(order.Status))
                    order.Status = Statuses[0]; // если статус из базы не в списке, заменить на первый

                SelectedEditableOrder = order;
            }
        }

        private void Save()
        {
            if (SelectedEditableOrder != null)
                _db.UpdateOrderDitails(SelectedEditableOrder);
        }

        private void Cancel()
        {
            RequestClose?.Invoke();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
