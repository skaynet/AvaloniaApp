using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaApp.Services;

namespace AvaloniaApp
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override async void OnFrameworkInitializationCompleted()
        {
            await PlaywrightInitializer.EnsureInstalledAsync();
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                //desktop.MainWindow = new MainWindow();
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new ViewModels.MainWindowViewModel()
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}