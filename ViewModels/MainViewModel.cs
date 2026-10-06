using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Storage; // Добавили для работы с Preferences
using Samostoyayka.Models;
using Samostoyayka.Services;

namespace Samostoyayka.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public ObservableCollection<ChoreItem> Chores { get; set; } = new();
        public ObservableCollection<ChoreItem> CompletedChores { get; set; } = new();

        private DatabaseService _dbService;

        public MainViewModel()
        {
            _dbService = new DatabaseService();
        }

        public void RefreshData()
        {
            Chores.Clear();
            CompletedChores.Clear();

            var allChores = _dbService.GetChores();

            // === ПРОЦЕСС СБРОСА В ПОЛНОЧЬ ===
            // Получаем дату последнего сброса (если ее нет, берется минимально возможная дата)
            DateTime lastResetDate = Preferences.Default.Get("LastResetDate", DateTime.MinValue);
            DateTime today = DateTime.Today;

            // Если наступил новый день
            if (today > lastResetDate)
            {
                foreach (var chore in allChores)
                {
                    chore.IsCompleted = false;   // Снимаем галочку
                    _dbService.SaveChore(chore); // Обновляем в базе
                }
                // Запоминаем текущую дату, чтобы сегодня больше не сбрасывать
                Preferences.Default.Set("LastResetDate", today);
            }
            // ================================

            if (allChores.Count == 0)
            {
                _dbService.SaveChore(new ChoreItem { Title = "Почистить зубы", IsCompleted = false });
                _dbService.SaveChore(new ChoreItem { Title = "Заправить постель", IsCompleted = false });
                _dbService.SaveChore(new ChoreItem { Title = "Собрать рюкзак", IsCompleted = false });
                allChores = _dbService.GetChores();
            }

            foreach (var chore in allChores)
            {
                chore.PropertyChanged += OnChorePropertyChanged;
                if (chore.IsCompleted)
                    CompletedChores.Add(chore);
                else
                    Chores.Add(chore);
            }
        }

        private void OnChorePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ChoreItem.IsCompleted))
            {
                var chore = sender as ChoreItem;
                if (chore != null)
                {
                    _dbService.SaveChore(chore);

                    if (chore.IsCompleted)
                    {
                        Chores.Remove(chore);
                        CompletedChores.Add(chore);
                    }
                    else
                    {
                        CompletedChores.Remove(chore);
                        Chores.Add(chore);
                    }
                }
            }
        }

        [RelayCommand]
        private async Task GoToSettings()
        {
            await Shell.Current.GoToAsync("SettingsPage");
        }
    }
}