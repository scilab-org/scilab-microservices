namespace Lab.Api.Constants;

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

    public static class Paper
    {
        #region Constants

        public const string Tags = "Papers";
        private const string Base = "/papers";
        private const string BaseAdmin = "/admin/papers";

        #endregion

        #region Enpoints

        public const string Create = $"{BaseAdmin}";
        public const string Update = $"{BaseAdmin}/{{id}}";
        public const string Delete = $"{BaseAdmin}/{{id}}";
        public const string GetPapers = $"{Base}";
        public const string GetPaperById = $"{Base}/{{id}}";

        #endregion
    }

    public static class Tag
    {
        #region Constants

        public const string Tags = "Tags";
        private const string Base = "/tags";
        private const string BaseAdmin = "/admin/tags";

        #endregion

        #region Enpoints

        public const string Create = $"{BaseAdmin}";
        public const string Update = $"{BaseAdmin}/{{id}}";
        public const string Delete = $"{BaseAdmin}/{{id}}";
        public const string GetTags = $"{Base}";
        public const string GetTagById = $"{Base}/{{id}}";

        #endregion
    }
}