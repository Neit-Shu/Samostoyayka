using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
            // Убрали LoadData отсюда, теперь экран сам будет просить данные
        }

        // Этот метод будет вызываться каждый раз при показе экрана
        public void RefreshData()
        {
            // Очищаем старые списки перед загрузкой новых данных
            Chores.Clear();
            CompletedChores.Clear();

            var allChores = _dbService.GetChores();

            // Если база пустая, добавляем тестовые
            if (allChores.Count == 0)
            {
                _dbService.SaveChore(new ChoreItem { Title = "Почистить зубы", IsCompleted = false });
                _dbService.SaveChore(new ChoreItem { Title = "Заправить постель", IsCompleted = false });
                _dbService.SaveChore(new ChoreItem { Title = "Собрать рюкзак", IsCompleted = false });
                allChores = _dbService.GetChores();
            }

            // Распределяем задачи
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