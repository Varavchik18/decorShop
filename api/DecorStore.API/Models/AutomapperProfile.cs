using AutoMapper;
using DecorStore.BL.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Section, SectionDto>();
        CreateMap<Category, CategoryDto>();
        CreateMap<Subcategory, SubcategoryDto>();
    }
}