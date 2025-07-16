using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using UnitTestRunnerMVVM.Helpers;
using UnitTestRunnerMVVM.Models;
using UnitTestRunnerMVVM.Models.Interfaces;
using UnitTestRunnerMVVM.Services;

namespace UnitTestRunnerMVVM.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly ITestRunner _testRunner;
        private readonly ILoggerService _logger;

        public ObservableCollection<string> ConsoleOutput { get; } = new();
        public ObservableCollection<TestResult> SummaryResults { get; } = new();

        private string _dllPath;
        public string DllPath
        {
            get => _dllPath;
            set { _dllPath = value; OnPropertyChanged(); _runTestsCommand?.RaiseCanExecuteChanged(); }
        }

        public ICommand BrowseDllCommand { get; }
        public ICommand RunTestsCommand => _runTestsCommand;

        private readonly AsyncRelayCommand _runTestsCommand;

        public MainViewModel(ITestRunner testRunner, ILoggerService logger)
        {
            _testRunner = testRunner;
            _logger = logger;

            BrowseDllCommand = new RelayCommand(BrowseDll);
            _runTestsCommand = new AsyncRelayCommand(RunTestsAsync, () => !string.IsNullOrWhiteSpace(DllPath));
        }

        private void BrowseDll()
        {
            OpenFileDialog dlg = new() { Filter = "DLL Files (*.dll)|*.dll", Title = "Select Unit Test DLL" };
            if (dlg.ShowDialog() == true) DllPath = dlg.FileName;
        }

        private async Task RunTestsAsync()
        {
            try
            {
                var results = await _testRunner.RunTestsAsync(DllPath);

                ConsoleOutput.Clear();
                SummaryResults.Clear();

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
                _logger.LogError(ex.Message);
            }
        }
    }
}
