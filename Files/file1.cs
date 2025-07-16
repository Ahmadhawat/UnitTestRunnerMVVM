
// UnitTestRunnerMVVM/
// │
// ├── App.xaml
// ├── Program.cs
// │
// ├── Models/
// │   ├── TestResult.cs
// │   ├── TestRunner.cs
// │   └── Interfaces/ITestRunner.cs
// │
// ├── ViewModels/
// │   ├── BaseViewModel.cs
// │   ├── MainViewModel.cs
// │   └── SettingsViewModel.cs
// │
// ├── Views/
// │   ├── MainWindow.xaml
// │   └── SettingsView.xaml
// │
// ├── Helpers/
// │   ├── RelayCommand.cs
// │   ├── AsyncRelayCommand.cs
// │   └── Logger.cs
// │
// ├── Services/
// │   ├── TestRunnerService.cs
// │   ├── LoggerService.cs
// │   └── NavigationService.cs


// =====================
// App.xaml
// =====================
< Application x:Class="UnitTestRunnerMVVM.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             StartupUri="Views/MainWindow.xaml">
    <Application.Resources>
    </Application.Resources>
</Application>


// =====================
// Program.cs
// =====================
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using UnitTestRunnerMVVM.Models.Interfaces;
using UnitTestRunnerMVVM.Services;
using UnitTestRunnerMVVM.ViewModels;
using UnitTestRunnerMVVM.Views;

namespace UnitTestRunnerMVVM
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            var services = ConfigureServices();
            var app = new App();
            var mainWindow = services.GetRequiredService<MainWindow>();
            app.Run(mainWindow);
        }

        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<ITestRunner, TestRunnerService>();
            services.AddSingleton<ILoggerService, LoggerService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<MainWindow>();

            return services.BuildServiceProvider();
        }
    }
}


// =====================
// Models/TestResult.cs
// =====================
namespace UnitTestRunnerMVVM.Models
{
    public enum TestResultStatus
    {
        Info,
        Passed,
        Failed
    }

    public class TestResult
    {
        public string Message { get; set; }
        public TestResultStatus Status { get; set; }
    }
}


// =====================
// Models/Interfaces/ITestRunner.cs
// =====================
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitTestRunnerMVVM.Models.Interfaces
{
    public interface ITestRunner
    {
        Task<List<TestResult>> RunTestsAsync(string dllPath);
    }
}


// =====================
// ViewModels/BaseViewModel.cs
// =====================
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UnitTestRunnerMVVM.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}


// =====================
// ViewModels/MainViewModel.cs
// =====================
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using UnitTestRunnerMVVM.Helpers;
using UnitTestRunnerMVVM.Models;
using UnitTestRunnerMVVM.Models.Interfaces;

namespace UnitTestRunnerMVVM.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly ITestRunner _testRunner;
        private string _dllPath;

        public string DllPath
        {
            get => _dllPath;
            set { _dllPath = value; OnPropertyChanged(); _runTestsCommand?.RaiseCanExecuteChanged(); }
        }

        public ObservableCollection<string> ConsoleOutput { get; } = new();
        public ObservableCollection<TestResult> SummaryResults { get; } = new();

        public ICommand BrowseDllCommand { get; }
        public ICommand RunTestsCommand => _runTestsCommand;

        private readonly AsyncRelayCommand _runTestsCommand;

        public MainViewModel(ITestRunner testRunner)
        {
            _testRunner = testRunner;
            BrowseDllCommand = new RelayCommand(BrowseDll);
            _runTestsCommand = new AsyncRelayCommand(RunTestsAsync, () => !string.IsNullOrWhiteSpace(DllPath));
        }

        private void BrowseDll()
        {
            var dialog = new OpenFileDialog { Filter = "DLL Files (*.dll)|*.dll" };
            if (dialog.ShowDialog() == true)
                DllPath = dialog.FileName;
        }

        private async Task RunTestsAsync()
        {
            try
            {
                ConsoleOutput.Clear();
                SummaryResults.Clear();

                var results = await _testRunner.RunTestsAsync(DllPath);
                foreach (var result in results)
                {
                    ConsoleOutput.Add(result.Message);
                    if (result.Status != TestResultStatus.Info)
                        SummaryResults.Add(result);
                }
            }
            catch (Exception ex)
            {
                var error = new TestResult { Message = "[ERROR] " + ex.Message, Status = TestResultStatus.Failed };
                ConsoleOutput.Add(error.Message);
                SummaryResults.Add(error);
            }
        }
    }
}


// =====================
// ViewModels/SettingsViewModel.cs
// =====================
namespace UnitTestRunnerMVVM.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private string _setting;
        public string Setting
        {
            get => _setting;
            set { _setting = value; OnPropertyChanged(); }
        }
    }
}


// =====================
// Views/MainWindow.xaml
// =====================
<Window x:Class="UnitTestRunnerMVVM.Views.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Unit Test Runner" Height="600" Width="900">
    <Grid Margin="10">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
            <RowDefinition Height="2*"/>
        </Grid.RowDefinitions>

        <StackPanel Orientation="Horizontal" Margin="0,0,0,10">
            <Button Content="Browse DLL" Command="{Binding BrowseDllCommand}" Width="100" />
            <TextBox Text="{Binding DllPath}" Width="600" Margin="10,0" IsReadOnly="True" />
            <Button Content="Run Tests" Command="{Binding RunTestsCommand}" Width="100" Margin="10,0,0,0" />
        </StackPanel>

        <ListBox Grid.Row="1" ItemsSource="{Binding ConsoleOutput}" FontFamily="Consolas" />
        <ListBox Grid.Row="2" ItemsSource="{Binding SummaryResults}">
            <ListBox.ItemTemplate>
                <DataTemplate>
                    <TextBlock Text="{Binding Message}" />
                </DataTemplate>
            </ListBox.ItemTemplate>
        </ListBox>
    </Grid>
</Window>


// =====================
// Views/SettingsView.xaml
// =====================
<UserControl x:Class="UnitTestRunnerMVVM.Views.SettingsView"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <StackPanel>
        <TextBlock Text="Settings" FontWeight="Bold" FontSize="20" Margin="10"/>
        <TextBox Text="{Binding Setting}" Margin="10" Width="200" />
    </StackPanel>
</UserControl>


// =====================
// Helpers/RelayCommand.cs
// =====================
using System;
using System.Windows.Input;

namespace UnitTestRunnerMVVM.Helpers
{
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;
        public void Execute(object parameter) => _execute();
        public event EventHandler CanExecuteChanged { add => CommandManager.RequerySuggested += value; remove => CommandManager.RequerySuggested -= value; }
        public void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();
    }
}


// =====================
// Helpers/AsyncRelayCommand.cs
// =====================
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UnitTestRunnerMVVM.Helpers
{
    public class AsyncRelayCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;

        public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;
        public async void Execute(object parameter) => await _execute();
        public event EventHandler CanExecuteChanged { add => CommandManager.RequerySuggested += value; remove => CommandManager.RequerySuggested -= value; }
        public void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();
    }
}


// =====================
// Helpers/Logger.cs
// =====================
namespace UnitTestRunnerMVVM.Helpers
{
    public interface ILoggerService
    {
        void Log(string message);
    }

    public class LoggerService : ILoggerService
    {
        public void Log(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[LOG]: {message}");
        }
    }
}


// =====================
// Services/TestRunnerService.cs
// =====================
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnitTestRunnerMVVM.Models;
using UnitTestRunnerMVVM.Models.Interfaces;

namespace UnitTestRunnerMVVM.Services
{
    public class TestRunnerService : ITestRunner
    {
        public async Task<List<TestResult>> RunTestsAsync(string dllPath)
        {
            var results = new List<TestResult>();

            var psi = new ProcessStartInfo
            {
                FileName = "vstest.console.exe",
                Arguments = $"\"{dllPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = psi };
            process.Start();

            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            process.WaitForExit();

            foreach (var line in output.Split(Environment.NewLine))
            {
                if (line.Contains("Passed"))
                    results.Add(new TestResult { Message = line, Status = TestResultStatus.Passed });
                else if (line.Contains("Failed"))
                    results.Add(new TestResult { Message = line, Status = TestResultStatus.Failed });
                else
                    results.Add(new TestResult { Message = line, Status = TestResultStatus.Info });
            }

            if (!string.IsNullOrEmpty(error))
                results.Add(new TestResult { Message = "[ERROR] " + error, Status = TestResultStatus.Failed });

            return results;
        }
    }
}


// =====================
// Services/NavigationService.cs
// =====================
namespace UnitTestRunnerMVVM.Services
{
    public interface INavigationService
    {
        void NavigateTo(string viewName);
    }

    public class NavigationService : INavigationService
    {
        public void NavigateTo(string viewName)
        {
            // Basic navigation logic placeholder
        }
    }
}
