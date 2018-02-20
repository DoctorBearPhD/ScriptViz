using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ScriptViz.Command
{
    class RelayCommand : ICommand
    {
        private Action commandTask;

        // If the circumstances for CanExecute have changed, this event is raised (for listeners to react to it)
        public event EventHandler CanExecuteChanged;


        public RelayCommand(Action task)
        {
            commandTask = task;
        }

        public bool CanExecute(object parameter)
        {
            // If there's a reason we can't execute the command, define that reason here.
            return true;
        }

        // execute the command
        public void Execute(object parameter)
        {
            // Show the Script Window
            commandTask();
        }
    }
}
