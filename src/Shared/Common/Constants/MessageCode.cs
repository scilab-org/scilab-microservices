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

    public const string PaperPublicationDateInvalid = "PAPER_PUBLICATION_DATE_INVALID";

    public const string PaperTitleIsRequired = "PAPER_TITLE_IS_REQUIRED";

    public const string PaperIsNotExists = "PAPER_IS_NOT_EXISTS";

    public const string PaperIdIsRequired = "PAPER_ID_IS_REQUIRED";

    #endregion

    #region Tag Message Codes

    public const string TagNameIsRequired = "TAG_NAME_IS_REQUIRED";

    public const string TagIsNotExists = "TAG_IS_NOT_EXISTS";

    public const string TagIdIsRequired = "TAG_ID_IS_REQUIRED";

    public const string TagNameLengthExceeded = "TAG_NAME_LENGTH_EXCEEDED_50_CHARACTERS";

    public const string TagNameMustStartWithHash = "TAG_NAME_MUST_START_WITH_HASH";

    #endregion
    
    #region Member Management

    public const string UserIdsAreRequired = "USER_IDS_ARE_REQUIRED";

    public const string AllMembersAlreadyExist = "ALL_MEMBERS_ALREADY_EXIST";

    public const string MemberProjectIdIsRequired = "MEMBER_PROJECT_ID_IS_REQUIRED";

    public const string MemberIdsAreRequired = "MEMBER_IDS_ARE_REQUIRED";

    public const string MembersNotFound = "MEMBERS_NOT_FOUND";

    public const string GroupNameIsRequired = "GROUP_NAME_IS_REQUIRED";

    #endregion
}