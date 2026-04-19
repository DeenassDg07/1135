using _1125.DB;
using _1125.Model;
using _1125.View;
using _1125.VMTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace _1125.ViewModel
{
    public class RegistrationVM : BaseVM
    {
        static User UsingUser;
        private List<User> userlist;
        private string login = "";
        private string password = "";
        private string password2 = "";
        public ICommand Logingood { get; set; }
        public string Login
        {
            get => login;
            set
            {
                login = value;
                Signal();
            }
        }
        public List<User> Userlist
        {
            get => userlist;
            set
            {
                userlist = value;
                Signal();
            }
        }
        public string Password
        {
            get => password;
            set
            {
                password = value;
                Signal();
            }
        }
        public string Password2
        {
            get => password2;
            set
            {
                password2 = value;
                Signal();
            }
        }
        public RegistrationVM()
        {

            Logingood = new CommandVM(() =>
            { 
                if (Login == Password)
                {
                    MessageBox.Show("Пароль слишком лёгкий");
                    return; 
                }
                if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password) != string.IsNullOrWhiteSpace(Password2))
                {
                    MessageBox.Show("Поля заполнены неверно");
                }
                else
                {
                    User user = new User
                    {
                        Login = Login,
                        Password = Password,
                        Role = "user",
                    };
                    UserDB.GetDb().Insert(user);

                    EntranceWindow entrancewindow = new EntranceWindow(true);
                    entrancewindow.Show();
                    close?.Invoke();
                    
                }
            }, () => true);
        }
        Action close;
        internal void SetClose(Action close)
        {
            this.close = close;
        }
    }
}
