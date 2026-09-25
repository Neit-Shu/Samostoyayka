using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Samostoyayka.Models;
using Samostoyayka.Services;

namespace Samostoyayka.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        // Единый список всех задач для управления
        public ObservableCollection<ChoreItem> AllChores { get; set; } = new();

        [ObservableProperty]
        private string newTaskTitle;

        private DatabaseService _dbService;

        public SettingsViewModel()
        {
            _dbService = new DatabaseService();
            LoadData();
        }

        private void LoadData()
        {
            AllChores.Clear();
            var chores = _dbService.GetChores();
            foreach (var chore in chores)
            {
                AllChores.Add(chore);
            }
        }

        [RelayCommand]
        private void AddTask()
        {
            if (string.IsNullOrWhiteSpace(NewTaskTitle)) return;
            var newChore = new ChoreItem { Title = NewTaskTitle.Trim(), IsCompleted = false };

            _dbService.SaveChore(newChore);
            AllChores.Add(newChore);

            NewTaskTitle = string.Empty;
        }

        [RelayCommand]
        private void DeleteTask(ChoreItem chore)
        {
            if (chore == null) return;
            _dbService.DeleteChore(chore);
            AllChores.Remove(chore);
        }
    }
}