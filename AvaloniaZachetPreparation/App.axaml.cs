using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using AvaloniaZachetPreparation.ViewModels;
using AvaloniaZachetPreparation.Views;
using Microsoft.EntityFrameworkCore;
using AvaloniaZachetPreparation.Models;

namespace AvaloniaZachetPreparation;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var options = new DbContextOptionsBuilder<_41pProductsContext>()
                .UseNpgsql("Host=edu.ngknn.ru;Port=5442;Database=41P_products;Username=21P;Password=123")
                    .Options;
            
            var db = new _41pProductsContext(options);
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(db),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}