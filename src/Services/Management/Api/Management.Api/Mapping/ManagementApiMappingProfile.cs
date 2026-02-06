using AutoMapper;
using Management.Api.Models;
using Management.Application.Dtos.Datasets;

namespace Management.Api.Mapping;

public sealed class ManagementApiMappingProfile : Profile
{
    #region Ctors

    public ManagementApiMappingProfile()
    {
        CreateMap<CreateDatasetRequest, CreateDatasetDto>()
            .ForMember(dest => dest.ProjectId, opt => opt.Ignore());
        
        CreateMap<UpdateDatasetRequest, UpdateDatasetDto>();
    }

    #endregion
}