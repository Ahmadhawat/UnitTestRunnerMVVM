using System.Windows.Media;

namespace UnitTestRunnerMVVM.Models
{
    public enum TestResultStatus { Info, Passed, Failed }

    public class TestResult
    {
        public string Message { get; set; }
        public TestResultStatus Status { get; set; }

        public Brush StatusBrush => Status switch
        {
            TestResultStatus.Passed => Brushes.Green,
            TestResultStatus.Failed => Brushes.Red,
            _ => Brushes.Gray
        };
    }
}
