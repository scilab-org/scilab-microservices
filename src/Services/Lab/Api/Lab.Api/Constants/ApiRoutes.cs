namespace Lab.Api.Constants;

public sealed class ApiRoutes
{
    public static class Dashboard
    {
        public const string Tags = "Dashboard";
        public const string GetAdminKpis = "/admin/dashboard/kpis";
        public const string GetUserKpis = "/user/dashboard/kpis";
    }

    public static class System
    {
        #region Constants

        public const string Tags = "System";

        private const string BaseAdmin = "/admin/system";

        public const string InitializeData = $"{BaseAdmin}/initialize-data";
        public const string UpdateProjectRules = $"{BaseAdmin}/project-rules";

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
        public const string RetryIngestion = $"{BaseAdmin}/{{id}}/retry-ingestion";

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
        public const string Update = $"{Base}/{{id}}";
        public const string Delete = $"{BaseManager}/{{id}}";
        public const string GetPapers = $"{Base}";
        public const string GetPaperSamples = $"{Base}/sample";
        public const string Initialize = $"{BaseManager}/initialize";
        public const string GetAssignedPaperSections = $"{Base}/{{id}}/assigned-sections";
        public const string GetAssignedPaperSectionsHistory = $"{Base}/{{id}}/assigned-sections/history";
        public const string GetSectionsByPaperId = $"{Base}/{{id}}/sections";
        public const string GetVersionsByPaperId = $"{Base}/{{id}}/versions";
        public const string GetPaperById = $"{Base}/{{id}}";
        public const string Versions = $"{Base}/{{id}}/versions";
        public const string GetVersionById = $"{Base}/{{paperId}}/versions/{{versionId}}";
        public const string TransitionStatus = $"{Base}/{{id}}/status-transition";
        public const string GetStatusHistory = $"{Base}/{{id}}/status-history";
        public const string GetSubmissionStatusSummary = $"{Base}/submission-status-summary";
        public const string CreateVersionFile = $"{Base}/{{paperId}}/versions/{{versionId}}/files";
        public const string UploadPaperFile = $"{Base}/{{paperId}}/files";
        public const string GetVersionFiles = $"{Base}/{{paperId}}/versions/{{versionId}}/files";
        public const string GetVersionFileById = $"{Base}/version-files/{{id}}";

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
        public const string UpdateGuideline = $"{Base}/{{id}}/guideline";
        public const string Delete = $"{Base}/{{id}}";
        public const string GetSectionById = $"{Base}/{{id}}";
        public const string GetSectionByMarkSectionId = $"{Base}/mark-section/{{id}}";
        public const string GetSectionVersionsByMarkSectionId = $"{Base}/mark-section/{{id}}/versions";
        public const string GetSectionHistory = $"{Base}/{{id}}/history";
        public const string Upload = $"{Base}/{{id}}/upload-file";
        public const string GetSectionFileById = $"{Base}/{{id}}/files";
        public const string DeleteSectionFileById = $"{Base}/{{id}}/files/{{fileName}}";
        public const string MarkMainSection = $"{Base}/{{id}}/mark-main-section";
        public const string Reference = $"{Base}/{{id}}/reference";
        public const string InUseReference = $"{Base}/{{id}}/reference/in-use";
        public const string PreviewReference = $"{Base}/reference/preview";
        public const string MarkSectionToReview = $"{Base}/{{id}}/mark-section-to-review";
        public const string MarkSectionToCompleted = $"{Base}/{{id}}/mark-section-to-completed";
        public const string GetNumberOfCompleteSection = $"{Base}/{{id}}/number-of-complete-section";

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

        public const string Create = $"{Base}";
        public const string Update = $"{Base}/{{id}}";
        public const string Delete = $"{Base}/{{id}}";
        public const string GetJournals = $"{Base}";
        public const string GetJournalById = $"{Base}/{{id}}";
        public const string GetJournalsInProject = $"{Base}/projects/{{projectId}}";
        public const string GetJournalInProjectById = $"{Base}/{{id}}/projects/{{projectId}}";

        #endregion
    }

    public static class CheckList
    {
        #region Constants

        public const string Tags = "CheckLists";
        private const string Base = "/check-lists";
        private const string BaseAdmin = "/admin/check-lists";

        #endregion

        #region Endpoints

        public const string Create = $"{BaseAdmin}";
        public const string Update = $"{BaseAdmin}/{{id}}";
        public const string Delete = $"{BaseAdmin}/{{id}}";
        public const string GetCheckLists = $"{Base}";
        public const string GetCheckListById = $"{Base}/{{id}}";

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
    public static class AuthorRole
    {
        #region Constants

        public const string Tags = "Author Roles";
        private const string Base = "/author-roles";
        private const string BaseAdmin = "/admin/author-roles";

        #endregion

        #region Endpoints

        public const string Create = $"{BaseAdmin}";
        public const string Update = $"{BaseAdmin}/{{id}}";
        public const string Delete = $"{BaseAdmin}/{{id}}";
        public const string GetAuthorRoles = $"{Base}";
        public const string GetAuthorRoleById = $"{Base}/{{id}}";
        #endregion
    }

    public static class PaperAuthor
    {
        #region Constants

        public const string Tags = "Paper Authors";
        private const string Base = "/paper-authors";
        private const string BaseAdmin = "/admin/paper-authors";

        #endregion

        #region Endpoints

        public const string Create = $"{Base}";
        public const string Update = $"{Base}/{{id}}";
        public const string Delete = $"{Base}/{{id}}";
        public const string GetPaperAuthors = $"{Base}";
        public const string GetPaperAuthorById = $"{Base}/{{id}}";
        #endregion
    }

    public static class GapType
    {
        #region Constants

        public const string Tags = "Gap Types";
        private const string Base = "/gap-types";
        private const string BaseAdmin = "/admin/gap-types";

        #endregion

        #region Endpoints

        public const string Create = $"{BaseAdmin}";
        public const string Update = $"{BaseAdmin}/{{id}}";
        public const string Delete = $"{BaseAdmin}/{{id}}";
        public const string GetGapTypes = $"{Base}";
        public const string GetGapTypeById = $"{Base}/{{id}}";

        #endregion
    }
}
