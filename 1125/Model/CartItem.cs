using _1125.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public class CartItem : INotifyPropertyChanged
{
    private int quantity;

    public Product Product { get; set; }
    public int Availability => Product?.Availability ?? 0;

    public int Quantity
    {
        get => quantity;
        set
        {
            if (quantity != value)
            {
                quantity = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
}
