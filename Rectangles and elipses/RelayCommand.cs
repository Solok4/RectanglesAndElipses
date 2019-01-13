using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Rectangles_and_elipses
{
    class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute) : this(execute, null) { }

        public RelayCommand(Action<object> execute, Predicate<object> CanExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
                this._execute = execute;
                this._canExecute = CanExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add {CommandManager.RequerySuggested += value; }
            remove {CommandManager.RequerySuggested -= value; }
        }

        public Boolean CanExecute(object parameter)
        {
            return this._execute == null ? true : this._canExecute(parameter);
        }
        public void Execute(object parameter) { this._execute(parameter); }
    }
}
