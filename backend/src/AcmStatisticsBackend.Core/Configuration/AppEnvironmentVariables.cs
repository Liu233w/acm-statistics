using System;

namespace AcmStatisticsBackend.Configuration
{
    public class AppEnvironmentVariables
    {
        public static string DefaultAdminPassword =>
            Environment.GetEnvironmentVariable("BACKEND_ADMIN_DEFAULT_PASSWORD")
            ?? "123qwe";
    }
}
