using Andromeda.Tools.Avalonia.Themes.Abstractions;
using Andromeda.Tools.Avalonia.Themes.Models;
using Avalonia.Media;
using Avalonia.Styling;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Andromeda.Tools.Avalonia.Themes.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            Themes = [
                new(AvailableThemes.Fluent, "Fluent"),
                new(AvailableThemes.Simple, "Simple"),
            ];

            ThemeVariants = [
                new(AvailableThemeVariants.Light, "Light"),
                new(AvailableThemeVariants.Dark, "Dark"),
            ];

            AllColors = typeof(Colors)
                .GetProperties()
                .Select(p => {
                    var color = (Color?)p.GetValue(null);
                    if (color is null)
                    {
                        return null;
                    }
                    return new DefaultColorInfo(new SolidColorBrush(color.Value), p.Name);
                })
                .Where(x => x is not null)!;

            this
                .WhenAnyValue(
                    vm => vm.SelectedTheme,
                    vm => vm.SelectedThemeVariant
                )
                .Select(x => {
                    if (x.Item1 is null || x.Item2 is null)
                    {
                        return null;
                    }

                    var variant = x.Item2.Value switch
                    {
                        AvailableThemeVariants.Light => ThemeVariant.Light,
                        AvailableThemeVariants.Dark => ThemeVariant.Dark,
                        AvailableThemeVariants.Default => ThemeVariant.Default,
                        _ => ThemeVariant.Default,
                    };

                    return (IThemePreview?)(x.Item1?.Value switch
                    {
                        AvailableThemes.Fluent
                            => new FluentViewModel(variant),
                        AvailableThemes.Simple
                            => new SimpleViewModel(variant),
                        _ => null
                    });
                })
                .ToPropertyEx(this, vm => vm.Content);
        }

        [ObservableAsProperty]
        public IThemePreview? Content { get; }

        [Reactive]
        public ThemeInfo? SelectedTheme { get; set; }

        [Reactive]
        public ThemeVariantInfo? SelectedThemeVariant { get; set; }

        public IEnumerable<ThemeInfo> Themes { get; }

        public IEnumerable<ThemeVariantInfo> ThemeVariants { get; }

        public IEnumerable<DefaultColorInfo> AllColors { get; }
    }
}
