using AutoMapper;
using Management.Application.Dtos.Datasets;
using Management.Application.Dtos.Projects;
using Management.Application.Models.Results;
using Management.Domain.Entities;

namespace Management.Application.Mappings;

public class ManagementMappingProfile : Profile
{
    #region Ctors

    public ManagementMappingProfile()
    {
        CreateProjectMappings();
        CreateDatasetMappings();
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
    

    #endregion
}