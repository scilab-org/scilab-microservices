namespace Common.Constants;

public sealed class MessageCode
{
    #region Fields, Properties and Indexers

    public const string BadRequest = "BAD_REQUEST";

    public const string Unauthorized = "UNAUTHORIZED";

    public const string NotFound = "NOT_FOUND";

    public const string AccessDenied = "ACCESS_DENIED";

    public const string UnknownError = "UNKNOWN_ERROR";

    public const string ProjectNameIsRequired = "PROJECT_NAME_IS_REQUIRED";

    public const string ProjectIdIsRequired = "PROJECT_ID_IS_REQUIRED";

    public const string ProjectIsNotExists = "PROJECT_IS_NOT_EXISTS";

    public const string DatasetNameIsRequired = "DATASET_NAME_IS_REQUIRED";

    public const string DatasetIdIsRequired = "DATASET_ID_IS_REQUIRED";

    public const string DatasetIsNotExists = "DATASET_IS_NOT_EXISTS";

    public const string StartDateMustBeBeforeEndDate = "START_DATE_MUST_BE_BEFORE_END_DATE";

    #endregion

    #region Paper Message Codes

    public const string PaperPublicationDateInvalid = "PAPER_PUBLICATION_DATE_INVALID";

    public const string PaperTitleIsRequired = "PAPER_TITLE_IS_REQUIRED";

    public const string PaperIsNotExists = "PAPER_IS_NOT_EXISTS";

    public const string PaperIdIsRequired = "PAPER_ID_IS_REQUIRED";

    #endregion
}