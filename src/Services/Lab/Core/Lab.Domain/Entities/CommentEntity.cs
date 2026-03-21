using Lab.Domain.Abstractions;

namespace Lab.Domain.Entities;

public sealed class CommentEntity: Entity<Guid>
{ 
    #region Fields, Properties and Indexers

    public Guid SectionId { get; set; }
    public string Content { get; set; } = null!;
    public string UserName { get; set; } = null!;

    #endregion

    #region Factories

    public static CommentEntity Create(Guid id, Guid sectionId, string content, string userName)
    {
        return new CommentEntity()
        {
            Id = id,
            SectionId = sectionId,
            Content = content,
            UserName = userName,
            CreatedOnUtc = DateTimeOffset.UtcNow,
            LastModifiedOnUtc = DateTimeOffset.UtcNow,
        };
    }

    #endregion
    
    #region Methods
    
        public void Update(string content)
        {
            Content = content;
            LastModifiedOnUtc = DateTimeOffset.UtcNow;
        }
    
    #endregion
}