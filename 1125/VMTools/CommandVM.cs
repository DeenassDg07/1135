// VMTools/CommandVM.cs
using System;
using System.Windows.Input;

namespace _1125.VMTools
{
    public class CommandVM : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public CommandVM(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object parameter) => _execute();

        public event EventHandler CanExecuteChanged;
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }


    public class CommandParamVM<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;

        public CommandParamVM(Action<T> execute, Predicate<T> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return parameter is T t && (_canExecute == null || _canExecute(t));
        }

        public void Execute(object parameter)
        {
            if (parameter is T t)
                _execute(t);
        }

        public event EventHandler CanExecuteChanged;
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

}
