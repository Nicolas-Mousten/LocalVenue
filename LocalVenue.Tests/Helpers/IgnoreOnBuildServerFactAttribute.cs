namespace LocalVenue.Tests.Helpers
{
    // https://stackoverflow.com/a/69196697
    public sealed class IgnoreOnBuildServerFactAttribute : FactAttribute
    {
        public IgnoreOnBuildServerFactAttribute()
        {
            if (IsRunningOnBuildServer())
            {
                Skip =
                    "This integration test is is being skipped due to it calling out to a live third-party API.";
            }
        }

        private static bool IsRunningOnBuildServer()
        {
            return bool.TryParse(
                    Environment.GetEnvironmentVariable("GITHUB_ACTIONS"),
                    out var buildServerFlag
                ) && buildServerFlag;
        }
    }
}
