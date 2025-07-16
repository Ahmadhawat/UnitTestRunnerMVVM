using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using UnitTestRunnerMVVM.Models.Interfaces;
using UnitTestRunnerMVVM.Models;
using UnitTestRunnerMVVM.Services;
using UnitTestRunnerMVVM.ViewModels;

namespace UnitTestRunnerMVVM
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            var provider = services.BuildServiceProvider();

            var app = new App();
            var mainWindow = new Views.MainWindow
            {
                DataContext = provider.GetRequiredService<MainViewModel>()
            };
            app.Run(mainWindow);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ITestRunner, TestRunner>();
            services.AddSingleton<ILoggerService, LoggerService>();
            services.AddSingleton<INavigationService, NavigationService>();

            services.AddSingleton<MainViewModel>();
            services.AddSingleton<SettingsViewModel>();
        }
    }
}
