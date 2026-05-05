namespace Management.Api.Constants;

public sealed class ApiRoutes
{
    public static class Dashboard
    {
        public const string Tags = "Dashboard";
        public const string GetAdminDashboard = "/admin/dashboard";
    }

    public static class System
    {
        #region Constants

        public const string Tags = "System";

        private const string BaseAdmin = "/admin/system";

        public const string InitializeData = $"{BaseAdmin}/initialize-data";

        #endregion
    }
    public static class Project
    {
        #region Constants
        public const string Tags = "Projects";

        private const string Base = "/projects";
        private const string BaseAdmin = "/admin/projects";
        public const string Create = $"{BaseAdmin}";
        public const string Update = $"{BaseAdmin}/{{projectId}}";
        public const string Delete = $"{BaseAdmin}/{{projectId}}";
        public const string GetProjects = $"{BaseAdmin}";
        public const string GetProjectById = $"{Base}/{{projectId}}";
        public const string GetAvailableProjectUsers = $"{Base}/{{projectId}}/users/available";
        public const string GetProjectsByUserId = $"{Base}/users/{{userId}}";
        public const string GetMyProjects = $"{Base}/me";
        public const string GetAssignedPapers = $"{Base}/me/assigned-papers";
        public const string GetMyProjectRole = $"{Base}/{{projectId}}/my-role";
        #endregion
    }

    public static class SubProject
    {
        #region Constants
        public const string Tags = "Sub-Projects";

        private const string Base = "/sub-projects";
        private const string BaseProjectManager = "/manager/sub-projects";

        public const string AddSubProjectMember = $"{Base}/{{subProjectId}}/members";
        public const string DeleteSubProjectMembers = $"{BaseProjectManager}/{{subProjectId}}/members/remove";
        public const string DeleteSubProjectPaper = $"{BaseProjectManager}/{{subProjectId}}";
        public const string GetAvailableSubProjectMembers = $"{Base}/{{subProjectId}}/members/available";
        public const string GetAvailablePaperAuthorMembers = $"{Base}/{{subProjectId}}/paper-authors/available";
        public const string GetSubProjectMembers = $"{Base}/{{subProjectId}}/members";
        public const string GetMemberByPaperId = $"{Base}/papers/{{paperId}}/member";
        public const string GetSubProjectMembersByPaperId = $"{Base}/papers/{{paperId}}/members";
        #endregion
    }


    public static class ProjectPaper
    {
        public const string Tags = "Project Papers";

        private const string Base = "/projects";
        private const string BaseManager = "/manager/projects";

        public const string CreateProjectPaper = $"{BaseManager}/{{projectId}}/papers";
        public const string GetAvailablePapers = $"{BaseManager}/{{projectId}}/papers/available";
        public const string DeleteProjectPapers = $"{BaseManager}/{{projectId}}/papers/remove";
        public const string CreateSubProject = $"{Base}/{{projectId}}/sub-projects";
        public const string GetSubProjects = $"{Base}/{{projectId}}/sub-projects";
        public const string GetSubmissionStatusSummary = $"{Base}/{{projectId}}/submission-status-summary";
        public const string GetProjectPapers = $"{Base}/{{projectId}}/papers";
        public const string DeleteProjectPaperByBankId = $"{Base}/paper-bank/{{paperBankId}}";
   }

    public static class ProjectConferenceJournal
    {
        public const string Tags = "Project Conference Journals";
        private const string Base = "/projects";
        private const string BaseManager = "/manager/projects";
        public const string ProjectConferenceJournals = $"{BaseManager}/{{projectId}}/conference-journals/{{journalId}}";
        public const string DeleteProjectConferenceJournalByJournalId = $"{Base}/conference-journals/{{journalId}}";
    }

    public static class Member
    {
        #region Constants
        public const string Tags = "Members";

        private const string Base = "/projects";
        private const string BaseAdmin = "/admin/projects";
        private const string BaseProjectManager = "/manager/projects";
        public const string AddProjectManagers    = $"{BaseAdmin}/{{projectId}}/managers";
        public const string DeleteProjectManagers = $"{BaseAdmin}/{{projectId}}/managers/remove";
        public const string AddProjectMembers    = $"{BaseProjectManager}/{{projectId}}/members";
        public const string DeleteProjectMembers = $"{BaseProjectManager}/{{projectId}}/members/remove";
        public const string GetProjectMembers    = $"{Base}/{{projectId}}/members";
        public const string UpdateProjectMemberRole = $"{BaseProjectManager}/{{projectId}}/members/{{memberId}}/role";
        public const string GetMemberById = $"{Base}/members/{{memberId}}";
        public const string GetMemberAffiliations = $"{Base}/members/{{memberId}}/affiliations";
        #endregion
    }

    public static class Dataset
    {
        #region Constants
        public const string Tags = "Datasets";

        private const string Base = "/datasets";
        private const string BaseProjectManager = "/manager/datasets";
        public const string Create = $"{BaseProjectManager}";
        public const string Update = $"{BaseProjectManager}/{{datasetId}}";
        public const string Delete = $"{BaseProjectManager}/{{datasetId}}";
        public const string GetDatasets = $"{Base}";

        #endregion
    }

    public static class Domain
    {
        #region Constants
        public const string Tags = "Domains";

        private const string BaseAdmin = "/admin/domains";
        private const string Base = "/domains";
        public const string Create = $"{BaseAdmin}";
        public const string Update = $"{BaseAdmin}/{{id}}";
        public const string Delete = $"{BaseAdmin}/{{id}}";
        public const string GetDomains = $"{Base}";
        public const string GetDomainById = $"{Base}/{{id}}";

        #endregion
    }

    public static class Affiliation
    {
        #region Constants
        public const string Tags = "Affiliations";

        private const string BaseAdmin = "/admin/affiliations";
        private const string Base = "/affiliations";
        public const string Create = $"{BaseAdmin}";
        public const string Update = $"{BaseAdmin}/{{id}}";
        public const string Delete = $"{BaseAdmin}/{{id}}";
        public const string GetAffiliations = $"{Base}";
        public const string GetAffiliationById = $"{Base}/{{id}}";

        #endregion
    }

    public static class UserAffiliation
    {
        #region Constants
        public const string Tags = "User Affiliations";

        private const string BaseAdmin = "/admin/user-affiliations";
        private const string Base = "/user-affiliations";
        public const string Create = $"{BaseAdmin}";
        public const string Update = $"{BaseAdmin}/{{id}}";
        public const string Delete = $"{BaseAdmin}/{{id}}";
        public const string GetUserAffiliations = $"{BaseAdmin}";
        public const string GetUserAffiliationById = $"{Base}/{{id}}";
        public const string GetUserAffiliationByUserIdAndAffiliationId = $"{Base}/users/{{userId}}/affiliations/{{affiliationId}}";

        #endregion
    }
}
