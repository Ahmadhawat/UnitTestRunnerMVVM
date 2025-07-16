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
            // Placeholder for real navigation logic
            System.Diagnostics.Debug.WriteLine($"Navigating to: {viewName}");
        }
    }
}
