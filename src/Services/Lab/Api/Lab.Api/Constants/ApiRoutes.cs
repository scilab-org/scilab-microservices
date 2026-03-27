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

    public static class PaperBank
    {
        #region Constants

        public const string Tags = "Paper Bank";
        private const string Base = "/paper-bank";
        private const string BaseAdmin = "/admin/paper-bank";

        #endregion

        #region Enpoints

        public const string Create = $"{BaseAdmin}";
        public const string Update = $"{BaseAdmin}/{{id}}";
        public const string Delete = $"{BaseAdmin}/{{id}}";
        public const string GetPaperBanks = $"{Base}";
        public const string GetPaperBankById = $"{Base}/{{id}}";

        #endregion
    }

    public static class Paper
    {
        #region Constants

        public const string Tags = "Papers";
        private const string Base = "/papers";
        private const string BaseManager = "/manager/papers";

        #endregion

        #region Enpoints

        public const string Create = $"{Base}";
        public const string Delete = $"{BaseManager}/{{id}}";
        public const string GetPaperSamples = $"{Base}/sample";
        public const string Initialize = $"{BaseManager}/initialize";
        public const string GetAssignedPaperSections = $"{Base}/{{id}}/assigned-sections";
        public const string GetAssignedPaperSectionsHistory = $"{Base}/{{id}}/assigned-sections/history";
        public const string GetSectionsByPaperId = $"{Base}/{{id}}/sections";
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

    public static class PaperContributor
    {
        #region Constants

        public const string Tags = "Paper Contributors";
        private const string Base = "/paper-contributors";
        private const string BaseAuthor = "/author/paper-contributors";

        #endregion

        #region Enpoints

        public const string Create = $"{BaseAuthor}";
        public const string Update = $"{BaseAuthor}/{{id}}";
        public const string Delete = $"{BaseAuthor}/{{id}}";
        public const string GetMemberSection = $"{Base}/{{sectionId}}/members";
        public const string GetAvailableMemberSection = $"{Base}/{{sectionId}}/members/available";
        public const string GetPaperContributors = $"{Base}/papers/{{paperId}}/contributors";

        #endregion
    }

    public static class Section
    {
        #region Constants

        public const string Tags = "Sections";
        private const string Base = "/sections";

        #endregion

        #region Endpoints

        public const string Create = $"{Base}";
        public const string Update = $"{Base}/{{id}}";
        public const string Delete = $"{Base}/{{id}}";
        public const string GetSectionById = $"{Base}/{{id}}";
        public const string GetSectionByMarkSectionId = $"{Base}/mark-section/{{id}}";
        public const string Upload = $"{Base}/{{id}}/upload-file";
        public const string GetSectionFileById = $"{Base}/{{id}}/files";

        #endregion

    }

    public static class Journal
    {
        #region Constants

        public const string Tags = "Journals";
        private const string Base = "/journals";
        private const string BaseAdmin = "/admin/journals";

        #endregion

        #region Endpoints

        public const string Create = $"{BaseAdmin}";
        public const string Update = $"{BaseAdmin}/{{id}}";
        public const string Delete = $"{BaseAdmin}/{{id}}";
        public const string GetJournals = $"{Base}";
        public const string GetJournalById = $"{Base}/{{id}}";

        #endregion
    }
    
    public static class Comment
    {
        #region Constants

        public const string Tags = "Comments";
        private const string Base = "/comments";

        #endregion

        #region Endpoints

        public const string Create = $"{Base}";
        public const string Update = $"{Base}/{{id}}";
        public const string Delete = $"{Base}/{{id}}";
        public const string GetCommentsBySectionId = $"{Base}/section/{{sectionId}}";

        #endregion
    }

    public static class Task
    {
        #region Constants

        public const string Tags = "Tasks";
        private const string Base = "/tasks";

        #endregion

        #region Endpoints

        public const string Create = $"{Base}";
        public const string Update = $"{Base}/{{id}}";
        public const string Delete = $"{Base}/{{id}}";
        public const string GetMyTasks = $"{Base}/my-tasks";
        public const string GetTaskById = $"{Base}/{{id}}";
        public const string GetTasksByPaperId = $"{Base}/paper/{{paperId}}";
        #endregion
    }
}
