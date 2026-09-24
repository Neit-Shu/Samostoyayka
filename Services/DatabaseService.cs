using SQLite;
using Samostoyayka.Models;

namespace Samostoyayka.Services
{
    public class DatabaseService
    {
        private SQLiteConnection _db;

        public DatabaseService()
        {
            // Указываем путь, где будет лежать файл базы на устройстве
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "Samostoyayka.db3");

            // Подключаемся к базе (если файла нет — он создастся сам)
            _db = new SQLiteConnection(dbPath);

            // Создаем таблицу для наших задач (если она еще не создана)
            _db.CreateTable<ChoreItem>();
        }

        // Метод для получения всех задач из базы
        public List<ChoreItem> GetChores()
        {
            return _db.Table<ChoreItem>().ToList();
        }

        // Метод для добавления или обновления задачи
        public void SaveChore(ChoreItem chore)
        {
            // Ищем задачу в базе по Id. Если есть — обновляем, если нет — добавляем новую
            if (_db.Table<ChoreItem>().FirstOrDefault(c => c.Id == chore.Id) != null)
            {
                _db.Update(chore);
            }
            else
            {
                _db.Insert(chore);
            }
        }
        // Метод для удаления задачи из базы данных
        public void DeleteChore(ChoreItem chore)
        {
            _db.Delete(chore);
        }
    }
}