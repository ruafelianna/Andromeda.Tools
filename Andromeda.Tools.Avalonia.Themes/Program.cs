using Avalonia;
using Avalonia.ReactiveUI;
using System;

namespace Andromeda.Tools.Avalonia.Themes
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI()
                .WithInterFont()
                .LogToTrace();
    }
}
