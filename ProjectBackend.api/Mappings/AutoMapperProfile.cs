using AutoMapper;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.DTO;

namespace ProjectBackend.api.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ProductsDomain, ProductDto>().ReverseMap();
            CreateMap<CreateProductDto, ProductsDomain>();
            CreateMap<UpdateProductDto, ProductsDomain>();
        }
    }
}
