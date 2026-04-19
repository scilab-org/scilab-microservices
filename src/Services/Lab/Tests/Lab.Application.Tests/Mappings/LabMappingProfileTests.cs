using AutoMapper;
using Lab.Application.Mappings;

namespace Lab.Application.Tests.Mappings;

public sealed class LabMappingProfileTests
{
    [Fact]
    public void MappingProfile_ShouldCreateMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<LabMappingProfile>());
        var mapper = config.CreateMapper();
        mapper.Should().NotBeNull();
    }
}
