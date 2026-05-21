using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace TodoApp.Controls;

public partial class CalendarPopup : UserControl
{
    private DateTime _currentMonth;
    private DateTime _selectedDate;

    public event EventHandler<DateTime>? DateSelected;

    private static readonly SolidColorBrush InkDeep = new(Color.FromRgb(0x11, 0x18, 0x27));
    private static readonly SolidColorBrush Accent = new(Color.FromRgb(0x2D, 0x3F, 0x5F));
    private static readonly SolidColorBrush TodayGreen = new(Color.FromRgb(0x4A, 0x7E, 0x50));
    private static readonly SolidColorBrush LightGray = new(Color.FromRgb(0xC8, 0xC3, 0xBC));
    private static readonly SolidColorBrush HoverBg = new(Color.FromRgb(0xF0, 0xED, 0xE8));
    private static readonly SolidColorBrush SelectedBg = new(Color.FromRgb(0x2D, 0x3F, 0x5F));
    private static readonly SolidColorBrush TodayBg = new(Color.FromRgb(0xE8, 0xF0, 0xE9));

    static CalendarPopup()
    {
        InkDeep.Freeze(); Accent.Freeze(); TodayGreen.Freeze();
        LightGray.Freeze(); HoverBg.Freeze(); SelectedBg.Freeze(); TodayBg.Freeze();
    }

    public CalendarPopup()
    {
        InitializeComponent();
        _currentMonth = DateTime.Today;
        _selectedDate = DateTime.Today;
    }

    public void SetDate(DateTime date)
    {
        _selectedDate = date;
        _currentMonth = new DateTime(date.Year, date.Month, 1);
        RebuildCalendar();
    }

    private void RebuildCalendar()
    {
        MonthYearLabel.Text = _currentMonth.ToString("yyyy年 M月");

        DayGrid.Children.Clear();

        var firstDay = _currentMonth;
        int startCol = (int)firstDay.DayOfWeek;
        int daysInMonth = DateTime.DaysInMonth(_currentMonth.Year, _currentMonth.Month);
        var today = DateTime.Today;

        for (int day = 1; day <= daysInMonth; day++)
        {
            var date = new DateTime(_currentMonth.Year, _currentMonth.Month, day);
            int idx = startCol + day - 1;
            int row = idx / 7;
            int col = idx % 7;

            var btn = CreateDayButton(day, date, today);
            Grid.SetRow(btn, row);
            Grid.SetColumn(btn, col);
            DayGrid.Children.Add(btn);
        }
    }

    private Button CreateDayButton(int day, DateTime date, DateTime today)
    {
        bool isToday = date == today;
        bool isSelected = date.Date == _selectedDate.Date;
        bool isCurrentMonth = date.Month == _currentMonth.Month;

        var btn = new Button
        {
            Content = day.ToString(),
            FontSize = 14,
            Width = 36,
            Height = 36,
            Cursor = Cursors.Hand,
            BorderThickness = new Thickness(0),
            Tag = date
        };

        if (isSelected)
        {
            btn.Background = SelectedBg;
            btn.Foreground = Brushes.White;
            btn.FontWeight = FontWeights.SemiBold;
        }
        else if (isToday)
        {
            btn.Background = TodayBg;
            btn.Foreground = TodayGreen;
            btn.FontWeight = FontWeights.SemiBold;
        }
        else if (!isCurrentMonth)
        {
            btn.Background = Brushes.Transparent;
            btn.Foreground = LightGray;
        }
        else
        {
            btn.Background = Brushes.Transparent;
            btn.Foreground = InkDeep;
        }

        btn.Click += Day_Click;
        btn.MouseEnter += Day_MouseEnter;
        btn.MouseLeave += Day_MouseLeave;

        return btn;
    }

    private void Day_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is DateTime date)
        {
            _selectedDate = date;
            DateSelected?.Invoke(this, date);
            RebuildCalendar();
        }
    }

    private void Day_MouseEnter(object sender, MouseEventArgs e)
    {
        if (sender is Button btn && btn.Tag is DateTime date)
        {
            if (date.Date != _selectedDate.Date && date != DateTime.Today)
                btn.Background = HoverBg;
        }
    }

    private void Day_MouseLeave(object sender, MouseEventArgs e)
    {
        if (sender is Button btn && btn.Tag is DateTime date)
        {
            if (date.Date != _selectedDate.Date && date != DateTime.Today)
                btn.Background = Brushes.Transparent;
        }
    }

    private void PrevMonth_Click(object sender, RoutedEventArgs e)
    {
        _currentMonth = _currentMonth.AddMonths(-1);
        RebuildCalendar();
    }

    private void NextMonth_Click(object sender, RoutedEventArgs e)
    {
        _currentMonth = _currentMonth.AddMonths(1);
        RebuildCalendar();
    }

    private void Today_Click(object sender, RoutedEventArgs e)
    {
        _selectedDate = DateTime.Today;
        _currentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        DateSelected?.Invoke(this, _selectedDate);
        RebuildCalendar();
    }
}
