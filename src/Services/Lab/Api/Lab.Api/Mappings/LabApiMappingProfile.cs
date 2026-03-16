#region using

using AutoMapper;
using Lab.Api.Models.PaperBank;
using Lab.Application.Dtos.PaperBanks;

#endregion

namespace Lab.Api.Mappings;

public sealed class LabApiMappingProfile : Profile
{
    #region Ctors

    public LabApiMappingProfile()
    {
        CreatePaperBankMapping();
        UploadSectionMapping();
    }

    #endregion


    #region Paper Mappings

    private void CreatePaperBankMapping()
    {
        CreateMap<CreatePaperBankRequest, CreatePaperBankDto>();

        CreateMap<UpdatePaperBankRequest, UpdatePaperBankDto>();
    }

    #endregion

    #region Section Mappings

    private void UploadSectionMapping()
    {
        CreateMap<UploadSectionFileRequest, UploadSectionFileDto>();
    }

    #endregion

}