using Avalonia.Media;

namespace Andromeda.Tools.Avalonia.Themes.Models
{
    internal record BrushInfo(
        IBrush? Brush,
        bool Found,
        string Name,
        string? Type
    );
}
