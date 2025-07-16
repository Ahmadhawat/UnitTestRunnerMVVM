namespace UnitTestRunnerMVVM.Models.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ITestRunner
    {
        Task<List<TestResult>> RunTestsAsync(string dllPath);
    }
}
