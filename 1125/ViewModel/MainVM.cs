using _1125.Model;
using _1125.View;
using _1125.VMTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace _1125.ViewModel
{
    internal class MainVM : BaseVM
    {
 
        public ICommand OpenLogin { get; set; }
        public ICommand OpenLoginDirector { get; set; }
        public ICommand Category { get; set; }
        public MainVM()
        {
            Category = new CommandVM(() =>
            {
                User.Current = new User(); 
                CategoryWindow categoryWindow = new CategoryWindow();
                close?.Invoke();
                categoryWindow.ShowDialog();
            }, () => true);


            OpenLogin = new CommandVM(() =>
            {
                EntranceWindow entranceWindow = new EntranceWindow(true);
                close?.Invoke();
                entranceWindow.ShowDialog();
            }, () => true);

            OpenLoginDirector = new CommandVM(() =>
            {
                EntranceWindow entranceWindow = new EntranceWindow(false);
                close?.Invoke();
                entranceWindow.ShowDialog();
            }, () => true);

        }
        Action close;
        internal void SetClose(Action close)
        {
            this.close = close;
        }
    }
}
