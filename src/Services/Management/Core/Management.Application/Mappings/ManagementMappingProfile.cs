using AutoMapper;
using Management.Application.Dtos.Affiliations;
using Management.Application.Dtos.Datasets;
using Management.Application.Dtos.Members;
using Management.Application.Dtos.Projects;
using Management.Application.Dtos.UserAffiliations;
using Management.Application.Models.Results;
using Management.Domain.Entities;

using System.Diagnostics.CodeAnalysis;
using Management.Application.Dtos.Domains;

namespace Management.Application.Mappings;

[ExcludeFromCodeCoverage]
public class ManagementMappingProfile : Profile
{
    #region Ctors

    public ManagementMappingProfile()
    {
        CreateProjectMappings();
        CreateDatasetMappings();
        CreateMemberMappings();
        CreateDomainMappings();
        CreateAffiliationMappings();
        CreateUserAffiliationMappings();
    }

    #endregion

    #region Methods

    private void CreateProjectMappings()
    {
        // ProjectEntity -> ProjectDto
        CreateMap<ProjectEntity, ProjectDto>();
        
        // ProjectEntity -> GetProjectByIdResult
        CreateMap<ProjectEntity, GetProjectByIdResult>()
            .ForMember(dest => dest.Project, opt => opt.MapFrom(src => src));
    }

    private void CreateDatasetMappings()
    {
        // DatasetEntity -> DatasetDto
        CreateMap<DatasetEntity, DatasetDto>();
        
    }
    
    private void CreateMemberMappings()
    {
        // DatasetEntity -> DatasetDto
        CreateMap<MemberEntity, MemberDto>();
        
    }
    
    private void CreateDomainMappings()
    {
        // Domain Models -> Dtos
        CreateMap<DomainEntity, DomainDto>();
    }

    private void CreateAffiliationMappings()
    {
        CreateMap<AffiliationEntity, AffiliationDto>();
    }

    private void CreateUserAffiliationMappings()
    {
        CreateMap<UserAffiliationEntity, UserAffiliationDto>()
            .ForMember(dest => dest.Affiliation, opt => opt.MapFrom(src => new AffiliationDto
            {
                Id = src.AffiliationId,
                Name = null,
                ShortName = null,
                RorId = null,
                RorUrl = null,
                CreatedOnUtc = default,
                CreatedBy = null,
                LastModifiedOnUtc = null,
                LastModifiedBy = null
            }));
    }
    #endregion
}
