using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using TeamCapacityBalancing.Navigation;
using System.Collections.Generic;
using TeamCapacityBalancing.Models;
using System;
using System.Collections.Generic;
using TeamCapacityBalancing.Models;
using TeamCapacityBalancing.Services.LocalDataSerialization;
using TeamCapacityBalancing.Services.Postgres_connection;
using TeamCapacityBalancing.Services.ServicesAbstractions;
using TeamCapacityBalancing.ViewModels;
using TeamCapacityBalancing.Views;

namespace TeamCapacityBalancing
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            ServiceCollection serviceCollection = new();
            serviceCollection.AddSingleton(serviceCollection);
            serviceCollection.AddSingleton<PageService>();
            serviceCollection.AddSingleton<NavigationService>();

            serviceCollection.AddSingleton<BalancingViewModel>();
            serviceCollection.AddSingleton<BalancingPage>();

            serviceCollection.AddSingleton<TeamViewModel>();
            serviceCollection.AddSingleton<TeamPage>();

            serviceCollection.AddSingleton<ResourceViewModel>();
            serviceCollection.AddSingleton<ResourcePage>();

            PageService pageService = serviceCollection.GetService<PageService>();
            pageService.RegisterPage<BalancingPage, BalancingViewModel>("Balancing Page");
            pageService.RegisterPage<TeamPage, TeamViewModel>("Team Page");
            pageService.RegisterPage<ResourcePage, ResourceViewModel>("Resource Page");

            NavigationService navigationService = serviceCollection.GetService<NavigationService>();
            navigationService.CurrentPageType=typeof(BalancingPage);

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Line below is needed to remove Avalonia data validation.
                // Without this line you will get duplicate validations from both Avalonia and CT
                ExpressionObserver.DataValidators.RemoveAll(x => x is DataAnnotationsValidationPlugin);
                desktop.MainWindow = new MainWindow
                {
                    DataContext = serviceCollection.CreateService<MainWindowViewModel>(),
                };
                serviceCollection.AddSingleton(desktop.MainWindow);
                IDataProvider p = new QueriesForDataBase();
                List<IssueData> list = new();
                list = p.GetAllEpicsByTeamLeader("JIRAUSER10101");
                list = p.GetAllStoriesByTeamLeader("JIRAUSER10101");
                list = p.GetStoriesByEpic(10030);
                

                

            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}