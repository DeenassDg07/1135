using System.ComponentModel;
using System.Runtime.CompilerServices;

public class User : INotifyPropertyChanged
{
    private int _id;
    private string _login;
    private string _password;
    private string _role = "guest";

    public int Id
    {
        get => _id;
        set
        {
            _id = value;
            OnPropertyChanged();
        }
    }

    public string Login
    {
        get => _login;
        set
        {
            _login = value;
            OnPropertyChanged();
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged();
        }
    }

    public string Role
    {
        get => _role;
        set
        {
            _role = value;
            OnPropertyChanged();
        }
    }

    private static User _current;
    public static User Current
    {
        get => _current;
        set
        {
            _current = value;
            CurrentChanged?.Invoke(null, EventArgs.Empty);
        }
    }

    public static event EventHandler CurrentChanged;

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
