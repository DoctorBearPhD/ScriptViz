using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ScriptViz.ViewModel;

namespace ScriptViz.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Set ViewModel reference
            var _vm = (this.Resources["Locator"] as ViewModelLocator).Main_VM;

            // Set the Window.Close function to the ViewModel command
            if (_vm.CloseAction == null) _vm.CloseAction = new Action(Close);
        }
    }
}

