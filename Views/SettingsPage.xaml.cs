using Samostoyayka.ViewModels;

namespace Samostoyayka.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();

        // Указываем экрану, откуда брать данные
        BindingContext = new SettingsViewModel();
    }
}