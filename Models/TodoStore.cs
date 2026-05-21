using System.IO;
using System.Text.Json;

namespace TodoApp.Models;

public class TodoStore
{
    private static readonly string DataFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "todos.json");
    private Dictionary<string, List<TodoItem>> _allTodos = new();

    public TodoStore()
    {
        Load();
    }

    public List<TodoItem> GetTodos(DateTime date)
    {
        var key = date.ToString("yyyy-MM-dd");
        return _allTodos.TryGetValue(key, out var todos) ? todos : new List<TodoItem>();
    }

    public void SaveTodos(DateTime date, List<TodoItem> todos)
    {
        var key = date.ToString("yyyy-MM-dd");
        _allTodos[key] = todos;
        Save();
    }

    private void Load()
    {
        if (!File.Exists(DataFilePath)) return;
        try
        {
            var json = File.ReadAllText(DataFilePath);
            _allTodos = JsonSerializer.Deserialize<Dictionary<string, List<TodoItem>>>(json) ?? new();
        }
        catch
        {
            _allTodos = new();
        }
    }

    private void Save()
    {
        var json = JsonSerializer.Serialize(_allTodos, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(DataFilePath, json);
    }
}
