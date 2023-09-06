using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ViewSonic
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public static List<string> BrushColorList { get; set; }
        public ICommand DrawCommand { get; }

        public static SolidColorBrush BrushColor = Brushes.Black;
        public static int Thickness = 2;

        public ViewModel() {
            BrushColorList = new List<string> { "Black", "Red", "Green" };
            DrawCommand = new RelayCommand(ExecuteDrawCommand, CanExecuteDrawCommand);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Color
        {
            set
            {
                if ("Black" == value)
                {
                    BrushColor = Brushes.Black;
                }
                else if ("Red" == value)
                {
                    BrushColor = Brushes.Red;

                }
                else if ("Green" == value)
                {
                    BrushColor = Brushes.Green;
                }

                OnPropertyChanged(nameof(Color));
            }
        }

        public int thickness
        {
            get { return Thickness; }
            set
            {
                if (Thickness != value)
                {
                    Thickness = value;
                    OnPropertyChanged(nameof(thickness));
                }
            }
        }

        private void ExecuteDrawCommand(object parameter)
        {
            MainWindow.ActionOption = 0;
        }

        private bool CanExecuteDrawCommand(object parameter)
        {
            //Always return true for this app
            return true;
        }
        public class RelayCommand : ICommand
        {
            private readonly Action<object> _execute;
            private readonly Func<object, bool> _canExecute;

            public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute)); //Check execute is not null 
                _canExecute = canExecute;
            }

            public event EventHandler CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }

            public bool CanExecute(object parameter)
            {
                return _canExecute == null || _canExecute(parameter);   //Check function is available
            }

            public void Execute(object parameter)
            {
                _execute(parameter);
            }
        }
    }
}
