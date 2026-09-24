using Samostoyayka.ViewModels;

namespace Samostoyayka;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        // Говорим экрану: "Твои данные лежат вот здесь"
        BindingContext = new MainViewModel();
    }
}