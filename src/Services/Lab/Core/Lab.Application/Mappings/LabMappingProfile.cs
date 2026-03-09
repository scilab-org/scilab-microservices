#region using

using AutoMapper;
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
        CreateTagMappings();
        CreateTemplateMappings();
        CreateSectionMappings();
    }

    #endregion

    #region Paper Mappings

    private void CreatePaperMappings()
    {
        CreateMap<PaperEntity, PaperDto>();

        CreateMap<PaperEntity, GetPaperByIdResult>()
            .ForMember(dest => dest.Paper, opt => opt.MapFrom(src => src));
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
}