using EnvManager.Models;
using EnvManager.Services;
using EnvManager.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace EnvManager.Views;

public partial class MainWindow : Window
{
    private readonly EnvironmentService _envService;
    private readonly CommentStorageService _commentStorageService;
    private readonly MainViewModel _mainViewModel;
    private ObservableCollection<EnvironmentVariable> Variables { get; set; }

    public MainWindow(EnvironmentService envService,
        CommentStorageService commentStorageServicel,
        MainViewModel mainViewModel)
    {
        InitializeComponent();
        _envService = envService;
        _commentStorageService = commentStorageServicel;
        _mainViewModel = mainViewModel;
        Variables = _mainViewModel.Variables;
        VariablesGrid.ItemsSource = Variables;
        Loaded += (s, e) => _mainViewModel.LoadData();

        DataContext = mainViewModel;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        try 
        {
            foreach (var variable in Variables)
            {
                _envService.SetVariable(variable);
                _commentStorageService.SetComment(variable);
            }
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
            _mainViewModel.LoadData();
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