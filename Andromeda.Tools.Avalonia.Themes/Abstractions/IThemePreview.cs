using Andromeda.Tools.Avalonia.Themes.Models;
using System.Collections.Generic;

namespace Andromeda.Tools.Avalonia.Themes.Abstractions
{
    internal interface IThemePreview
    {
        IEnumerable<ColorInfo> Colors { get; }

        IEnumerable<BrushInfo> Brushes { get; }
    }
}
