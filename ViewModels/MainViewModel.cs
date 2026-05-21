using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using TodoApp.Models;

namespace TodoApp.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly TodoStore _store = new();
    private DateTime _selectedDate = DateTime.Today;
    private string _newTodoText = string.Empty;

    private static readonly string[] WeekdayNames = ["星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六"];

    public MainViewModel()
    {
        Todos = new ObservableCollection<TodoItem>();
        Todos.CollectionChanged += OnTodosChanged;
        AddTodoCommand = new RelayCommand(AddTodo, () => !string.IsNullOrWhiteSpace(NewTodoText));
        DeleteTodoCommand = new RelayCommand<TodoItem>(DeleteTodo);
        LoadTodosForDate();
    }

    public ObservableCollection<TodoItem> Todos { get; }

    public DateTime SelectedDate
    {
        get => _selectedDate;
        set
        {
            if (SetProperty(ref _selectedDate, value))
            {
                LoadTodosForDate();
                OnPropertyChanged(nameof(DateDisplay));
                OnPropertyChanged(nameof(DateBigDisplay));
                OnPropertyChanged(nameof(WeekdayDisplay));
            }
        }
    }

    public string NewTodoText
    {
        get => _newTodoText;
        set => SetProperty(ref _newTodoText, value);
    }

    public string DateDisplay => SelectedDate.ToString("yyyy年MM月dd日 dddd");

    public string DateBigDisplay => SelectedDate.ToString("MM月dd日");

    public string WeekdayDisplay => WeekdayNames[(int)SelectedDate.DayOfWeek];

    public int TotalCount => Todos.Count;

    public int CompletedCount => Todos.Count(t => t.IsCompleted);

    public double Progress => TotalCount == 0 ? 0 : (double)CompletedCount / TotalCount;

    public string ProgressPercent => TotalCount == 0 ? "0%" : $"{(int)(Progress * 100)}%";

    public Visibility EmptyVisibility => TotalCount == 0 ? Visibility.Visible : Visibility.Collapsed;

    public ICommand AddTodoCommand { get; }
    public ICommand DeleteTodoCommand { get; }

    private void AddTodo()
    {
        var item = new TodoItem { Text = NewTodoText.Trim() };
        Todos.Add(item);
        NewTodoText = string.Empty;
        SaveCurrentTodos();
    }

    private void DeleteTodo(TodoItem? item)
    {
        if (item == null) return;
        Todos.Remove(item);
        SaveCurrentTodos();
    }

    private void LoadTodosForDate()
    {
        Todos.CollectionChanged -= OnTodosChanged;
        UnsubscribeAll();
        Todos.Clear();
        foreach (var todo in _store.GetTodos(SelectedDate))
        {
            todo.PropertyChanged += OnItemPropertyChanged;
            Todos.Add(todo);
        }
        Todos.CollectionChanged += OnTodosChanged;
        RefreshStats();
    }

    private void SaveCurrentTodos()
    {
        _store.SaveTodos(SelectedDate, Todos.ToList());
    }

    private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TodoItem.IsCompleted))
        {
            SaveCurrentTodos();
            OnPropertyChanged(nameof(CompletedCount));
            OnPropertyChanged(nameof(Progress));
            OnPropertyChanged(nameof(ProgressPercent));
        }
    }

    private void OnTodosChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
            foreach (TodoItem item in e.OldItems)
                item.PropertyChanged -= OnItemPropertyChanged;

        if (e.NewItems != null)
            foreach (TodoItem item in e.NewItems)
                item.PropertyChanged += OnItemPropertyChanged;

        RefreshStats();
    }

    private void UnsubscribeAll()
    {
        foreach (var item in Todos)
            item.PropertyChanged -= OnItemPropertyChanged;
    }

    private void RefreshStats()
    {
        OnPropertyChanged(nameof(TotalCount));
        OnPropertyChanged(nameof(CompletedCount));
        OnPropertyChanged(nameof(Progress));
        OnPropertyChanged(nameof(ProgressPercent));
        OnPropertyChanged(nameof(EmptyVisibility));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(name);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;

    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;
    public void Execute(object? parameter) => _execute();
}

public class RelayCommand<T> : ICommand
{
    private readonly Action<T?> _execute;

    public RelayCommand(Action<T?> execute) => _execute = execute;

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object? parameter) => true;
    public void Execute(object? parameter) => _execute((T?)parameter);
}
