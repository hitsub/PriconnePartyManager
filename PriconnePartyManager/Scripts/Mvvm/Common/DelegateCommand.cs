using System;
using System.Windows.Input;

namespace PriconnePartyManager.Scripts.Mvvm.Common
{
    public class DelegateCommand : ICommand
    {
        Action execute;
        Func<bool> canExecute;

        public bool CanExecute(object parameter)
        {
            return canExecute != null ? canExecute() : true;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void Execute(object parameter)
        {
            execute();
        }

        public DelegateCommand(Action execute)
        {
            this.execute = execute;
        }

        public DelegateCommand(Action execute, Func<bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }
    }
}