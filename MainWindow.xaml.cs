using EnvManager.Models;
using EnvManager.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace EnvManager
{
    public partial class MainWindow : Window
    {
        private readonly EnvironmentService _envService;
        private ObservableCollection<EnvironmentVariable> Variables { get; set; }

        public MainWindow(EnvironmentService envService)
        {
            InitializeComponent();
            Variables = new ObservableCollection<EnvironmentVariable>();
            VariablesGrid.ItemsSource = Variables;
            Loaded += (s, e) => envService.LoadVariables(Variables);
            _envService = envService;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _envService.SaveVariables(Variables);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _envService.LoadVariables(Variables);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Обработчики для кастомного окна

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}