#region using

using AutoMapper;
using Lab.Api.Models.Papers;
using Lab.Application.Dtos.Papers;

#endregion

namespace Lab.Api.Mappings;

public sealed class LabApiMappingProfile : Profile
{
    #region Ctors

    public LabApiMappingProfile()
    {
        CreatePaperMapping();
	}

    #endregion


    #region Paper Mappings

    private void CreatePaperMapping()
    {
        CreateMap<CreatePaperRequest, CreatePaperDto>();

        CreateMap<UpdatePaperRequest, UpdatePaperDto>();
    }

    #endregion

}