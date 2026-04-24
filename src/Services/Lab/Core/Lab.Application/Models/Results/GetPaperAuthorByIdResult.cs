using Lab.Application.Dtos.PaperAuthors;

namespace Lab.Application.Models.Results;

public sealed class GetPaperAuthorByIdResult
{
    public PaperAuthorDto PaperAuthor { get; init; }

    public GetPaperAuthorByIdResult(PaperAuthorDto paperAuthor)
    {
        PaperAuthor = paperAuthor;
    }
}
