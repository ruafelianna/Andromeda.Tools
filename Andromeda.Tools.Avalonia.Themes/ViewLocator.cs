using Andromeda.Tools.Avalonia.Themes.Abstractions;
using Andromeda.Tools.Avalonia.Themes.ViewModels;
using Andromeda.Tools.Avalonia.Themes.Views;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using System;

namespace Andromeda.Tools.Avalonia.Themes
{
    internal class ViewLocator : IDataTemplate
    {
        public Control? Build(object? param)
        {
            var vmType = param?.GetType();

            if (vmType?.IsAssignableTo(typeof(IThemePreview)) ?? false)
            {
                return new ThemePreviewView();
            }

            var name = vmType?.FullName?.Replace("ViewModel", "View");

            if (name is not null)
            {
                var vType = Type.GetType(name);

                if (vType is not null)
                {
                    return (Control?)Activator.CreateInstance(vType);
                }
            }

            return new TextBlock { Text = "Not Found: " + name };
        }

        public bool Match(object? data)
            => data is ViewModelBase;
    }
}
