using CommunityToolkit.Mvvm.ComponentModel;
using SQLite; // Добавили пространство имен для работы с БД

namespace Samostoyayka.Models
{
    public partial class ChoreItem : ObservableObject
    {
        // Пометили Id как первичный ключ (уникальный идентификатор в базе)
        [PrimaryKey]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private bool isCompleted;
    }
}