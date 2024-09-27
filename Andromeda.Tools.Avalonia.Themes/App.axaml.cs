using Andromeda.Tools.Avalonia.Themes.ViewModels;
using Andromeda.Tools.Avalonia.Themes.Views;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace Andromeda.Tools.Avalonia.Themes
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime cdl)
            {
                cdl.MainWindow = new MainWindow()
                {
                    DataContext = new MainViewModel(),
                };
            }
        }
    }
}
