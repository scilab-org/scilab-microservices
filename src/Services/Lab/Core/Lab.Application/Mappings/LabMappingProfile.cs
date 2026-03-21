#region using

using AutoMapper;
using Lab.Application.Dtos.Journals;
using Lab.Application.Dtos.PaperBanks;
using Lab.Application.Dtos.Papers;
using Lab.Application.Dtos.Sections;
using Lab.Application.Dtos.Tags;
using Lab.Application.Dtos.Template;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;

#endregion

namespace Lab.Application.Mappings;

public sealed class LabMappingProfile : Profile
{
    #region Ctors

    public LabMappingProfile()
    {
        CreatePaperMappings();
        CreatePaperBankMappings();
        CreateTagMappings();
        CreateTemplateMappings();
        CreateSectionMappings();
        CreateJournalMappings();
    }

    #endregion

    #region Paper Mappings

    private void CreatePaperMappings()
    {
        CreateMap<PaperEntity, PaperDto>();

        CreateMap<PaperEntity, GetPaperBankByIdResult>()
            .ForMember(dest => dest.PaperBank, opt => opt.MapFrom(src => src));
    }

    #endregion

    #region Paper Bank Mappings

    private void CreatePaperBankMappings()
    {
        CreateMap<PaperBankEntity, PaperBankDto>();
    }

    #endregion

    #region Tag Mappings

    private void CreateTagMappings()
    {
        CreateMap<TagEntity, TagDto>();

        CreateMap<TagEntity, GetTagByIdResult>()
            .ForMember(dest => dest.Tag, opt => opt.MapFrom(src => src));
    }

    #endregion

    #region Template Mappings

    private void CreateTemplateMappings()
    {
        CreateMap<TemplateEntity, TemplateDto>();
    }

    #endregion

    #region Section Mappings

    private void CreateSectionMappings()
    {
        CreateMap<SectionEntity, SectionDto>();
    }

    #endregion

    #region Journal Mappings

    private void CreateJournalMappings()
    {
        CreateMap<JournalEntity, JournalDto>();

        CreateMap<JournalEntity, GetJournalByIdResult>()
            .ForMember(dest => dest.Journal, opt => opt.MapFrom(src => src));
    }

    #endregion
}