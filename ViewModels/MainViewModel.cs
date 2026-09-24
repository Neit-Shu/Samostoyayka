using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel; // Добавили для ObservableObject
using CommunityToolkit.Mvvm.Input;          // Добавили для [RelayCommand]
using Samostoyayka.Models;
using Samostoyayka.Services;

namespace Samostoyayka.ViewModels
{
    // Наследуем от ObservableObject
    public partial class MainViewModel : ObservableObject
    {
        public ObservableCollection<ChoreItem> Chores { get; set; }
        public ObservableCollection<ChoreItem> CompletedChores { get; set; }

        // Свойство для связи с полем ввода
        [ObservableProperty]
        private string newTaskTitle;

        private DatabaseService _dbService;

        
        public MainViewModel()
        {
            Chores = new ObservableCollection<ChoreItem>();
            CompletedChores = new ObservableCollection<ChoreItem>();

            // Инициализируем базу данных
            _dbService = new DatabaseService();

            // Загружаем данные
            LoadData();
        }

        private void LoadData()
        {
            // 1. Берем все задачи из базы
            var allChores = _dbService.GetChores();

            // 2. Если база совсем пустая (первый запуск), добавляем базовые задачи
            if (allChores.Count == 0)
            {
                var chore1 = new ChoreItem { Title = "Почистить зубы", IsCompleted = false };
                var chore2 = new ChoreItem { Title = "Заправить постель", IsCompleted = false };
                var chore3 = new ChoreItem { Title = "Собрать рюкзак", IsCompleted = false };

                _dbService.SaveChore(chore1);
                _dbService.SaveChore(chore2);
                _dbService.SaveChore(chore3);

                // Снова запрашиваем из базы, чтобы получить актуальный список
                allChores = _dbService.GetChores();
            }

            // 3. Раскидываем задачи по нашим двум спискам
            foreach (var chore in allChores)
            {
                // Подписываемся на клики по галочкам
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
                    // ВАЖНО: Сохраняем новое состояние в базу данных!
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
        // Этот метод будет срабатывать при нажатии на кнопку
        [RelayCommand]
        private void AddTask()
        {
            // Проверяем, что поле не пустое
            if (string.IsNullOrWhiteSpace(NewTaskTitle))
                return;

            // Создаем новую задачу
            var newChore = new ChoreItem
            {
                Title = NewTaskTitle.Trim(),
                IsCompleted = false
            };

            // Подписываемся на ее чекбокс
            newChore.PropertyChanged += OnChorePropertyChanged;

            // Сохраняем в базу данных
            _dbService.SaveChore(newChore);

            // Добавляем в верхний список на экране
            Chores.Add(newChore);

            // Очищаем поле ввода
            NewTaskTitle = string.Empty;
        }
        // Команда принимает конкретную задачу (ChoreItem), которую нужно удалить
        [RelayCommand]
        private void DeleteTask(ChoreItem chore)
        {
            if (chore == null) return;

            // 1. Удаляем из базы данных
            _dbService.DeleteChore(chore);

            // 2. Отписываемся от изменений, чтобы не было утечек памяти
            chore.PropertyChanged -= OnChorePropertyChanged;

            // 3. Удаляем из списков (проверяем, где она находится)
            if (chore.IsCompleted)
            {
                CompletedChores.Remove(chore);
            }
            else
            {
                Chores.Remove(chore);
            }
        }
    }

}