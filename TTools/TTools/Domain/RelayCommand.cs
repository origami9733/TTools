using System;
using System.Windows.Input;
using TTools.Models;

namespace TTools.Views
{
    public class RelayCommand<T> : ICommand
    {
        private Action<T> execute;
        private Func<T, bool> canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            if (canExecute == null)
            {
                throw new ArgumentNullException("canExecute");
            }

            this.execute = execute;
            this.canExecute = canExecute;
        }

        public RelayCommand(Action<T> execute)
            : this(execute, p => true)
        {
        }

        public void Execute(T parameter)
        {
            this.execute(parameter);
        }

        public bool CanExecute(T parameter)
        {
            return this.canExecute(parameter);
        }

        bool ICommand.CanExecute(object parameter)
        {
            if (parameter == null)
            {
                return this.CanExecute(default(T));
            }

            return this.CanExecute((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        void ICommand.Execute(object parameter)
        {
            if (parameter == null)
            {
                this.Execute(default(T));
                return;
            }

            this.execute((T)parameter);
        }
    }
}