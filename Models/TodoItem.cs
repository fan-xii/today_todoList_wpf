using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TodoApp.Models;

public class TodoItem : INotifyPropertyChanged
{
    private string _text = string.Empty;
    private bool _isCompleted;

    public Guid Id { get; set; } = Guid.NewGuid();

    public string Text
    {
        get => _text;
        set => SetField(ref _text, value);
    }

    public bool IsCompleted
    {
        get => _isCompleted;
        set => SetField(ref _isCompleted, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
