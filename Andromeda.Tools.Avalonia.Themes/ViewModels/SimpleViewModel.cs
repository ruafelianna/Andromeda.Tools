using Avalonia.Styling;
using Avalonia.Themes.Fluent;

namespace Andromeda.Tools.Avalonia.Themes.ViewModels
{
    internal class SimpleViewModel : ThemePreview
    {
        public SimpleViewModel(ThemeVariant themeVariant) :
            base(themeVariant)
        {
        }

        protected override Styles Theme => _theme;

        protected override string[] ColorsList => _colors;

        protected override string[] BrushesList => _brushes;

        private static readonly Styles _theme = new FluentTheme();

        private static readonly string[] _colors = [
            "ThemeBackgroundColor",
            "ThemeBorderLowColor",
            "ThemeBorderMidColor",
            "ThemeBorderHighColor",
            "ThemeControlLowColor",
            "ThemeControlMidColor",
            "ThemeControlMidHighColor",
            "ThemeControlHighColor",
            "ThemeControlVeryHighColor",
            "ThemeControlHighlightLowColor",
            "ThemeControlHighlightMidColor",
            "ThemeControlHighlightHighColor",
            "ThemeForegroundColor",
            "HighlightColor",
            "HighlightColor2",
            "HyperlinkVisitedColor",
        ];

        private static readonly string[] _brushes = [
            "ThemeBackgroundBrush",
            "ThemeBorderLowBrush",
            "ThemeBorderMidBrush",
            "ThemeBorderHighBrush",
            "ThemeControlLowBrush",
            "ThemeControlMidBrush",
            "ThemeControlMidHighBrush",
            "ThemeControlHighBrush",
            "ThemeControlVeryHighBrush",
            "ThemeControlHighlightLowBrush",
            "ThemeControlHighlightMidBrush",
            "ThemeControlHighlightHighBrush",
            "ThemeForegroundBrush",
            "HighlightBrush",
            "HighlightBrush2",
            "HyperlinkVisitedBrush",
            "RefreshVisualizerForeground",
            "RefreshVisualizerBackground",
            "CaptionButtonForeground",
            "CaptionButtonBackground",
            "CaptionButtonBorderBrush",
        ];
    }
}
