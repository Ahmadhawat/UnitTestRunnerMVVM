namespace UnitTestRunnerMVVM.Services
{
    public interface ILoggerService
    {
        void LogInfo(string message);
        void LogError(string message);
    }

    public class LoggerService : ILoggerService
    {
        public void LogInfo(string message) =>
            System.Diagnostics.Debug.WriteLine("INFO: " + message);

        public void LogError(string message) =>
            System.Diagnostics.Debug.WriteLine("ERROR: " + message);
    }
}
