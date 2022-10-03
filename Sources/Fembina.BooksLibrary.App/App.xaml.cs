﻿using System.Windows;
using System.Windows.Threading;
using DependencyManagement.Composition.Containers;
using DependencyManagement.Composition.Extensions;
using DependencyManagement.Injection.Extensions;
using DependencyManagement.Injection.Strategies;
using DependencyManagement.Modularity.Extensions;
using Microsoft.Extensions.Logging;
using Prism.Ioc;

namespace Fembina.BooksLibrary.App;

using Utils;

public partial class App : Application
{
    private readonly IContainer _container;

    public App()
    {
        _container = CreateConfiguredContainer();
        Dispatcher.UnhandledException += OnUnhandledException;
    }

    public IReadOnlyContainer Container => _container;

    private static IContainer CreateConfiguredContainer()
    {
        var container = new Container();
        container.WithStrategies();

        container.AddModule<AppModule>()
            .With(() => new())
            .ToModule();

        container.SetTarget<AppWindow>()
            .With(static c => new(c.FirstInstance<INavigator>()))
            .ToSingleton();

        return container;
    }

    private void OnStartup(object sender, StartupEventArgs e)
    {
        var window = Container.LastInstance<AppWindow>();

        window.Show();
    }

    private void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        var logger = _container.TryFirstInstance<ILogger>();

        logger?.LogError($"App shutdown with exception: {e.Exception.Message}");
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _container.Dispose();
    }
}