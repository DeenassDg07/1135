using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using _1125.DB;
using _1125.Model;
using _1125.View;
using _1125.VMTools;

namespace _1125.ViewModel
{
    internal class EntranceVM : BaseVM
    {
        private string login= "";
        private string password ="";
        private List<User> userlist;
        private Action close;

        public ICommand Logingo { get; set; }
        public ICommand Registration { get; set; }

        public List<User> Userlist
        {
            get => userlist;
            set
            {
                userlist = value;
                Signal();
            }
        }

        public string Login
        {
            get => login;
            set
            {
                login = value;
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

        public bool CanRegister { get; } = true;

        public EntranceVM(bool canRegister = true)
        {
            CanRegister = canRegister;

            Logingo = new CommandVM(() =>
            {
                var user = UserDB.GetDb().Auth(login, password);

                if (user != null && user.Id != 0)
                {
                    User.Current = user;

                    var categoryWindow = new CategoryWindow();
                    categoryWindow.ShowDialog();
                    close?.Invoke();
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            });

            Registration = new CommandVM(() =>
            {
                var registrationWindow = new RegistrationWindow();
                close?.Invoke();
                registrationWindow.ShowDialog();
            });
        }

        internal void SetClose(Action close)
        {
            this.close = close;
        }
    }
}
