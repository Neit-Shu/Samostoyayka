using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Storage;
using Samostoyayka.Models;
using Samostoyayka.Services;

namespace Samostoyayka.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public ObservableCollection<ChoreItem> Chores { get; set; } = new();
        public ObservableCollection<ChoreItem> CompletedChores { get; set; } = new();

        // Свойство счетчика дней
        [ObservableProperty]
        private int streakCount;

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

            DateTime lastResetDate = Preferences.Default.Get("LastResetDate", DateTime.MinValue);
            DateTime today = DateTime.Today;

            if (today > lastResetDate)
            {
                // ПРОЦЕСС СБРОСА СТРИКА: Если последний раз все задачи выполнялись раньше, чем вчера
                DateTime lastStreakDate = Preferences.Default.Get("LastStreakDate", DateTime.MinValue);
                if (lastStreakDate < today.AddDays(-1))
                {
                    Preferences.Default.Set("StreakCount", 0);
                }

                foreach (var chore in allChores)
                {
                    chore.IsCompleted = false;
                    _dbService.SaveChore(chore);
                }
                Preferences.Default.Set("LastResetDate", today);
            }

            // Загружаем текущий счетчик
            StreakCount = Preferences.Default.Get("StreakCount", 0);

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
                if (chore.IsCompleted) CompletedChores.Add(chore);
                else Chores.Add(chore);
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

                        // Проверяем, выполнил ли ребенок все задачи
                        CheckStreak();
                    }
                    else
                    {
                        CompletedChores.Remove(chore);
                        Chores.Add(chore);
                    }
                }
            }
        }

        // Метод: если список дел пуст, засчитываем день
        private void CheckStreak()
        {
            if (Chores.Count == 0 && CompletedChores.Count > 0)
            {
                DateTime today = DateTime.Today;
                DateTime lastStreakDate = Preferences.Default.Get("LastStreakDate", DateTime.MinValue);

                // Засчитываем только если сегодня еще не прибавляли
                if (lastStreakDate < today)
                {
                    StreakCount++;
                    Preferences.Default.Set("StreakCount", StreakCount);
                    Preferences.Default.Set("LastStreakDate", today);
                }
            }
        }

        [RelayCommand]
        private async Task GoToSettings()
        {
            string pin = await Shell.Current.DisplayPromptAsync(
                "Родительский контроль", "Введите ПИН-код (1234):", keyboard: Keyboard.Numeric);

            if (pin == "1234") await Shell.Current.GoToAsync("SettingsPage");
            else if (!string.IsNullOrEmpty(pin)) await Shell.Current.DisplayAlert("Ошибка", "Неверный ПИН-код", "ОК");
        }
    }
}