namespace Common.Constants;

public sealed class MessageCode
{
    #region Fields, Properties and Indexers

    public const string BadRequest = "BAD_REQUEST";

    public const string Unauthorized = "UNAUTHORIZED";

    public const string NotFound = "NOT_FOUND";

    public const string AccessDenied = "ACCESS_DENIED";

    public const string UnknownError = "UNKNOWN_ERROR";

    #endregion

    #region Project Management

    public const string ProjectNameIsRequired = "PROJECT_NAME_IS_REQUIRED";

    public const string ProjectIdIsRequired = "PROJECT_ID_IS_REQUIRED";

    public const string ProjectIsNotExists = "PROJECT_IS_NOT_EXISTS";

    public const string StartDateMustBeBeforeEndDate = "START_DATE_MUST_BE_BEFORE_END_DATE";

    public const string AllPapersAlreadyExist = "ALL_PAPERS_ALREADY_EXIST";

    public const string SubProjectNotFound = "SUB_PROJECT_NOT_FOUND";

    public const string ProjectHasPaper = "PROJECT_HAS_PAPER";

    public const string ProjectCodeAlreadyExists = "PROJECT_CODE_ALREADY_EXISTS";

    public const string ConferenceJournalIdIsRequired = "CONFERENCE_JOURNAL_ID_IS_REQUIRED";

    public const string ConferenceJournalNotFoundInProject = "CONFERENCE_JOURNAL_NOT_FOUND_IN_PROJECT";

    #endregion

    #region Dataset Management

    public const string DatasetNameIsRequired = "DATASET_NAME_IS_REQUIRED";

    public const string DatasetIdIsRequired = "DATASET_ID_IS_REQUIRED";

    public const string DatasetIsNotExists = "DATASET_IS_NOT_EXISTS";

    #endregion

    #region User Management

    public const string UsernameIsRequired = "USERNAME_IS_REQUIRED";

    public const string EmailIsRequired = "EMAIL_IS_REQUIRED";

    public const string InvalidEmail = "INVALID_EMAIL";

    public const string InitialPasswordIsRequired = "INITIAL_PASSWORD_IS_REQUIRED";

    public const string PasswordMinimumLength = "PASSWORD_MINIMUM_LENGTH";

    public const string UserAlreadyExists = "USER_ALREADY_EXISTS";

    public const string UserNotFound = "USER_NOT_FOUND";

    public const string GroupNotFound = "GROUP_NOT_FOUND";

    public const string FailedToCreateUser = "FAILED_TO_CREATE_USER";

    public const string FailedToResetPassword = "FAILED_TO_RESET_PASSWORD";

    public const string FailedToAssignGroup = "FAILED_TO_ASSIGN_GROUP";

    public const string FailedToGetAccessToken = "FAILED_TO_GET_ACCESS_TOKEN";

    public const string FailedToRetrieveUser = "FAILED_TO_RETRIEVE_USER";

    public const string UserCreationCompensationFailed = "USER_CREATION_COMPENSATION_FAILED";

    public const string UserIdIsRequired = "USER_ID_IS_REQUIRED";

    public const string FailedToUpdateUser = "FAILED_TO_UPDATE_USER";

    public const string FailedToDeactivateUser = "FAILED_TO_DEACTIVATE_USER";

    public const string FailedToActivateUser = "FAILED_TO_ACTIVATE_USER";

    public const string FailedToGetUsers = "FAILED_TO_GET_USERS";

    public const string FailedToGetGroups = "FAILED_TO_GET_GROUPS";

    public const string FailedToGetRoles = "FAILED_TO_GET_ROLES";

    public const string FailedToGetGroupRoles = "FAILED_TO_GET_GROUP_ROLES";

    public const string FailedToAddRoleToGroup = "FAILED_TO_ADD_ROLE_TO_GROUP";

    public const string FailedToRemoveRoleFromGroup = "FAILED_TO_REMOVE_ROLE_FROM_GROUP";

    public const string RoleNotFound = "ROLE_NOT_FOUND";

    public const string GroupIdIsRequired = "GROUP_ID_IS_REQUIRED";

    public const string RoleNamesAreRequired = "ROLE_NAMES_ARE_REQUIRED";

    #endregion

    #region Paper Message Codes

    public const string InvalidStatusTransition = "INVALID_STATUS_TRANSITION";

    public const string DuplicateStatusTransition = "DUPLICATE_STATUS_TRANSITION";

    public const string PaperPublicationDateInvalid = "PAPER_PUBLICATION_DATE_INVALID";

    public const string PaperTitleIsRequired = "PAPER_TITLE_IS_REQUIRED";

    public const string PaperIsNotExists = "PAPER_IS_NOT_EXISTS";

    public const string PaperIdIsRequired = "PAPER_ID_IS_REQUIRED";

    public const string PaperFileIsRequired = "PAPER_FILE_IS_REQUIRED";

    public const string PaperNotFoundInProject = "PAPER_NOT_FOUND_IN_PROJECT";

    public const string SectionRoleIsRequired = "SECTION_ROLE_IS_REQUIRED";

    public const string PaperContextIsRequired = "PAPER_CONTEXT_IS_REQUIRED";

    public const string PaperJournalIsRequired = "PAPER_JOURNAL_IS_REQUIRED";

    public const string PaperBankIdsIsRequired = "PAPER_BANK_IDS_IS_REQUIRED";

    public const string PaperCombineIsNotExists = "PAPER_COMBINE_IS_NOT_EXISTS";

    public const string PaperCombineVersionIdIsRequired = "PAPER_COMBINE_VERSION_ID_IS_REQUIRED";

    public const string PaperCombineContentIsRequired = "PAPER_COMBINE_CONTENT_IS_REQUIRED";

    public const string PaperTemplateIsRequired = "PAPER_TEMPLATE_IS_REQUIRED";

    public const string PaperResearchGapIsRequired = "PAPER_RESEARCH_GAP_IS_REQUIRED";

    public const string PaperGapTypeIsRequired = "PAPER_GAP_TYPE_IS_REQUIRED";

    public const string PaperMainContributionIsRequired = "PAPER_MAIN_CONTRIBUTION_IS_REQUIRED";

    public const string PaperResearchAimIsRequired = "PAPER_RESEARCH_AIM_IS_REQUIRED";

    public const string JournalStartAtIsRequired  = "JOURNAL_START_AT_IS_REQUIRED";

    public const string JournalEndAtIsRequired  = "JOURNAL_END_AT_IS_REQUIRED";

    public const string PaperVersionIdIsRequired = "PAPER_VERSION_ID_IS_REQUIRED";

    public const string PaperVersionNotFound = "PAPER_VERSION_NOT_FOUND";

    public const string PaperVersionNotBelongToPaper = "PAPER_VERSION_NOT_BELONG_TO_PAPER";

    public const string PdfFileIsRequired = "PDF_FILE_IS_REQUIRED";

    public const string PdfFileNotFound = "PDF_FILE_NOT_FOUND";

    public const string PdfFileNotBelongToPaper = "PDF_FILE_NOT_BELONG_TO_PAPER";

    public const string PdfFileNameIsRequired = "PDF_FILE_NAME_IS_REQUIRED";

    public const string PdfFileUrlIsRequired = "PDF_FILE_URL_IS_REQUIRED";

    public const string PdfFileIdIsRequired = "PDF_FILE_ID_IS_REQUIRED";

    #endregion

    #region Tag Message Codes

    public const string TagNameIsRequired = "TAG_NAME_IS_REQUIRED";

    public const string TagIsNotExists = "TAG_IS_NOT_EXISTS";

    public const string TagIdIsRequired = "TAG_ID_IS_REQUIRED";

    public const string TagNameAlreadyExists = "TAG_NAME_ALREADY_EXISTS";

    #endregion

    #region PaperTag Message Codes

    public const string PaperTagIsNotExists = "PAPER_TAG_IS_NOT_EXISTS";

    public const string PaperTagIdIsRequired = "AT_LEAST_ONE_TAG_ID_IS_REQUIRED";

    #endregion

    #region Member Management

    public const string UserIdsAreRequired = "USER_IDS_ARE_REQUIRED";

    public const string MemberAlreadyExists = "MEMBER_ALREADY_EXISTS";

    public const string ProjectAlreadyHasManager = "PROJECT_ALREADY_HAS_MANAGER";

    public const string AllMembersAlreadyExist = "ALL_MEMBERS_ALREADY_EXIST";

    public const string MemberProjectIdIsRequired = "MEMBER_PROJECT_ID_IS_REQUIRED";

    public const string MemberIdsAreRequired = "MEMBER_IDS_ARE_REQUIRED";
    public const string MemberIdIsRequired = "MEMBER_ID_IS_REQUIRED";

    public const string MemberNotFound = "MEMBER_NOT_FOUND";

    public const string MembersNotFound = "MEMBERS_NOT_FOUND";

    public const string GroupNameIsRequired = "GROUP_NAME_IS_REQUIRED";

    #endregion

    #region Section Management

    public const string SectionIdIsRequired = "SECTION_ID_IS_REQUIRED";

    public const string UserIsNotInPaper = "USER_IS_NOT_IN_PAPER";

    public const string SectionFileIsRequired = "SECTION_FILE_IS_REQUIRED";

    public const string SectionIsNotExists = "SECTION_IS_NOT_EXISTS";

    public const string SectionIsAlreadyMainSection = "SECTION_IS_ALREADY_MAIN_SECTION";

    public const string SectionAlreadyHasVersion =  "SECTION_ALREADY_HAS_VERSION";

    public const string SectionContentIsRequired = "SECTION_CONTENT_IS_REQUIRED";

    public const string PaperDescriptionIsRequired = "PAPER_DESCRIPTION_IS_REQUIRED";

    public const string SectionStatusMustBeInProgress = "SECTION_STATUS_MUST_BE_IN_PROGRESS";

    public const string SectionStatusMustBeInReview = "SECTION_STATUS_MUST_BE_IN_REVIEW";

    #endregion

    #region Comment Management

    public const string CommentContentIsRequired = "CONTENT_IS_REQUIRED";
    public const string CommentNotFound = "COMMENT_NOT_FOUND";

    #endregion

    #region Journal Management

    public const string JournalNameIsRequired = "JOURNAL_NAME_IS_REQUIRED";

    public const string JournalIsNotExists = "JOURNAL_IS_NOT_EXISTS";

    public const string JournalIdIsRequired = "JOURNAL_ID_IS_REQUIRED";

    public const string JournalNameAlreadyExists = "JOURNAL_NAME_ALREADY_EXISTS";

    public const string JournalTexFileInvalidExtension = "JOURNAL_TEX_FILE_INVALID_EXTENSION";

    public const string JournalPdfFileInvalidExtension = "JOURNAL_PDF_FILE_INVALID_EXTENSION";

    public const string JournalProjectIdIsRequired = "JOURNAL_PROJECT_ID_IS_REQUIRED";

    public const string JournalStartDateMustBeforeEndDate = "JOURNAL_START_DATE_MUST_BE_BEFORE_END_DATE";

    public const string JournalRankingIsRequired = "JOURNAL_RANKING_IS_REQUIRED";

    public const string JournalUrlIsRequired = "JOURNAL_URL_IS_REQUIRED";

    #endregion

    #region Task Management
    public const string TaskNameIsRequired = "TASK_NAME_IS_REQUIRED";
    public const string TaskIdIsRequired = "TASK_ID_IS_REQUIRED";
    public const string TaskIsNotExists = "TASK_IS_NOT_EXISTS";
    #endregion

    #region PaperContributor Management

    public const string PaperContributorIdIsRequired = "PAPER_CONTRIBUTOR_ID_IS_REQUIRED";

    public const string PaperContributorNotFound = "PAPER_CONTRIBUTOR_NOT_FOUND";

    public const string ContributorIsNotExists = "CONTRIBUTOR_IS_NOT_EXISTS";

    #endregion

    #region Template Management

    public const string TemplateIsRequired = "TEMPLATE_IS_REQUIRED";

    public const string TemplateCodeIsRequired = "TEMPLATE_CODE_IS_REQUIRED";

    public const string TemplateSectionsAreRequired = "TEMPLATE_SECTIONS_ARE_REQUIRED";

    public const string SectionIsRequired = "SECTION_IS_REQUIRED";

    public const string SectionTitleIsRequired = "SECTION_TITLE_IS_REQUIRED";

    public const string SectionRuleIsRequired = "SECTION_RULE_IS_REQUIRED";

    public const string SectionDisplayOrderIsRequired = "SECTION_DISPLAY_ORDER_IS_REQUIRED";

    public const string TemplateIdIsRequired = "TEMPLATE_ID_IS_REQUIRED";

    public const string TemplateIsNotExists = "TEMPLATE_IS_NOT_EXISTS";

    public const string TemplateCodeIsAlreadyExists = "TEMPLATE_CODE_IS_ALREADY_EXISTS";

    #endregion
}