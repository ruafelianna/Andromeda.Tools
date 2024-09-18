using Andromeda.Tools.PublishPackages.ViewModels;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using System;

namespace Andromeda.Tools.PublishPackages
{
    internal class ViewLocator : IDataTemplate
    {
        public Control? Build(object? param)
        {
            var name = param?.GetType()?.FullName?.Replace("ViewModel", "View");

            if (name is not null)
            {
                var type = Type.GetType(name);

                if (type is not null)
                {
                    return (Control?)Activator.CreateInstance(type);
                }
            }

            return new TextBlock { Text = "Not Found: " + name };
        }

        public bool Match(object? data)
            => data is ViewModelBase;
    }
}
