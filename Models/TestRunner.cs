using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnitTestRunnerMVVM.Models.Interfaces;

namespace UnitTestRunnerMVVM.Models
{
    public class TestRunner : ITestRunner
    {
        public async Task<List<TestResult>> RunTestsAsync(string dllPath)
        {
            var results = new List<TestResult>();

            var processStartInfo = new ProcessStartInfo
            {
                FileName = "vstest.console.exe",
                Arguments = $"\"{dllPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = processStartInfo };
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
