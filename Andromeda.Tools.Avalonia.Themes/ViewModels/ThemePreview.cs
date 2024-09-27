using Andromeda.Tools.Avalonia.Themes.Abstractions;
using Andromeda.Tools.Avalonia.Themes.Models;
using Avalonia.Media;
using Avalonia.Styling;
using System.Collections.Generic;
using System.Linq;

namespace Andromeda.Tools.Avalonia.Themes.ViewModels
{
    internal abstract class ThemePreview : ViewModelBase, IThemePreview
    {
        protected ThemePreview(ThemeVariant themeVariant)
        {
            Colors = ColorsList
                .Select(x => {
                    Theme.TryGetResource(
                        x,
                        themeVariant,
                        out var value
                    );

                    if (value is Color col)
                    {
                        return new ColorInfo(col, true, x);
                    }

                    return new(null, false, x);
                })
                .OrderByDescending(x => x.Found)
                .ThenBy(x => x.Name);

            Brushes = BrushesList
                .Select(x => {
                    Theme.TryGetResource(
                        x,
                        themeVariant,
                        out var value
                    );

                    if (value is IBrush brush)
                    {
                        return new BrushInfo(
                            brush,
                            true,
                            x,
                            brush.GetType().Name
                        );
                    }

                    return new(null, false, x, null);
                })
                .OrderByDescending(x => x.Found)
                .ThenBy(x => x.Name);
        }

        public IEnumerable<ColorInfo> Colors { get; }

        public IEnumerable<BrushInfo> Brushes { get; }

        protected abstract Styles Theme { get; }

        protected abstract string[] ColorsList { get; }

        protected abstract string[] BrushesList { get; }
    }
}
