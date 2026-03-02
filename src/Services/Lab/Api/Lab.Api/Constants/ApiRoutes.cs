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
        public const string GetPaperSamples = $"{Base}/sample";

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
    
    public static class Template
    {
        #region Constants

        public const string Tags = "Paper Templates";
        private const string Base = "/paper-templates";
        private const string BaseAdmin = "/admin/paper-templates";

        #endregion

        #region Endpoints

        public const string Create = $"{BaseAdmin}";
        public const string Update = $"{BaseAdmin}/{{id}}";
        public const string Delete = $"{BaseAdmin}/{{id}}";

        public const string GetTemplates = $"{Base}";
        public const string GetTemplateById = $"{Base}/{{id}}";
        public const string GetByCode = $"{Base}/code/{{code}}";

        #endregion
    }
}