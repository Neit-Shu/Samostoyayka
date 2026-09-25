using Samostoyayka.ViewModels;

namespace Samostoyayka;

public partial class MainPage : ContentPage
{
    private MainViewModel _viewModel;

    public MainPage()
    {
        InitializeComponent();

        // Сохраняем ViewModel в переменную, чтобы обращаться к ней позже
        _viewModel = new MainViewModel();
        BindingContext = _viewModel;
    }

    // Этот метод вызывается автоматически каждый раз, когда экран появляется перед пользователем
    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Даем команду обновить списки из базы данных
        _viewModel.RefreshData();
    }
}