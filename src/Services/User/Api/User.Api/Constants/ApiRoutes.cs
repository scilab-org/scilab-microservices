namespace User.Api.Constants;

public sealed class ApiRoutes
{
    public static class System
    {
        #region Constants

        public const string Tags = "System";

        private const string BaseAdmin = "/admin/system";

        public const string InitializeData = $"{BaseAdmin}/initialize-data";

        #endregion
    }
}
