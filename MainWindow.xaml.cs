using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using TodoApp.ViewModels;

namespace TodoApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        LoadIcon();
        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
            Calendar.SetDate(vm.SelectedDate);
    }

    private void CalendarToggle_Click(object sender, RoutedEventArgs e)
    {
        CalendarPopupHost.IsOpen = !CalendarPopupHost.IsOpen;
        if (CalendarPopupHost.IsOpen && DataContext is MainViewModel vm)
            Calendar.SetDate(vm.SelectedDate);
    }

    private void Calendar_DateSelected(object? sender, DateTime date)
    {
        if (DataContext is MainViewModel vm)
            vm.SelectedDate = date;
        CalendarPopupHost.IsOpen = false;
    }

    private void LoadIcon()
    {
        try
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var icoPath = Path.Combine(baseDir, "app.ico");
            if (!File.Exists(icoPath))
                icoPath = Path.Combine(baseDir, "..", "..", "..", "app.ico");
            if (File.Exists(icoPath))
            {
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(icoPath, UriKind.Absolute);
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.EndInit();
                Icon = bmp;
            }
        }
        catch
        {
            // Fallback to default icon
        }
    }
}
