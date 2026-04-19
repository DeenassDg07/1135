using _1125.VMTools;
using System;
using System.Windows.Input;

namespace _1125.ViewModel
{
    internal class OrderVM : BaseVM
    {
        private DateTime deliveryDate = DateTime.Now.Date.AddDays(7);

        public DateTime DeliveryDate => deliveryDate;

        public ICommand Cancel { get; set; }
        public ICommand Ok { get; set; }

        public OrderVM()
        {
            Cancel = new CommandVM(() =>
            {
                close?.Invoke(false);
            });

            Ok = new CommandVM(() =>
            {
                close?.Invoke(true);
            });
        }

        private Action<bool> close;
        internal void SetClose(Action<bool> close)
        {
            this.close = close;
        }
    }
}
