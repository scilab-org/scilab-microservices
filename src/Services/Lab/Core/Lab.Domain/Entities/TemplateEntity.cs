using Lab.Domain.Abstractions;
using Lab.Domain.Enums;
using Lab.Domain.Models;

namespace Lab.Domain.Entities;

public sealed class TemplateEntity : Entity<Guid>
{
    #region Fields, Properties and Indexers

    public string? Code { get; set; }
    public string? Description { get; set; }
    public Guid ConferenceJournalId { get; set; }
    public List<Section>? Sections { get; set; }

    #endregion

    #region Factories

    public static TemplateEntity Create(string? code, string? description, Guid conferenceJournalId,
        List<Section>? sections, string? createdBy = null)
    {
        return new TemplateEntity()
        {
            Id = Guid.NewGuid(),
            Code = code,
            Description = description,
            ConferenceJournalId = conferenceJournalId,
            Sections = sections,
            CreatedBy = createdBy,
            CreatedOnUtc = DateTimeOffset.UtcNow,
            LastModifiedOnUtc = DateTimeOffset.UtcNow,
        };
    }

    #endregion

    #region Methods

    public void Update(string? code = null,
        string? description = null,
        Guid? confereneceJournalId = null,
        List<Section>? sections = null,
        string? lastModifiedBy = null)
    {
        Code = code ?? Code;
        Description = description ?? Description;
        ConferenceJournalId = confereneceJournalId ?? ConferenceJournalId;
        Sections = sections ?? Sections;
        LastModifiedBy = lastModifiedBy ?? LastModifiedBy;
        LastModifiedOnUtc = DateTimeOffset.UtcNow;
    }

    #endregion
}