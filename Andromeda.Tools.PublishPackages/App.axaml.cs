using Andromeda.Tools.PublishPackages.ViewModels;
using Andromeda.Tools.PublishPackages.Views;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace Andromeda.Tools.PublishPackages
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime cdl)
            {
                cdl.MainWindow = new MainWindow()
                {
                    DataContext = new MainViewModel(),
                };
            }
        }
    }
}
