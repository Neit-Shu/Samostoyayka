using Samostoyayka.Views; // Добавляем ссылку на папку Views

namespace Samostoyayka
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Регистрируем маршрут к экрану настроек
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
        }
    }
}