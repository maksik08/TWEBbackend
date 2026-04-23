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

            CreateMap<CategoryDomain, CategoryDto>().ReverseMap();
            CreateMap<CreateCategoryDto, CategoryDomain>();
            CreateMap<UpdateCategoryDto, CategoryDomain>();

            CreateMap<SupplierDomain, SupplierDto>().ReverseMap();
            CreateMap<CreateSupplierDto, SupplierDomain>();
            CreateMap<UpdateSupplierDto, SupplierDomain>();

            CreateMap<CustomerDomain, CustomerDto>().ReverseMap();
            CreateMap<CreateCustomerDto, CustomerDomain>();
            CreateMap<UpdateCustomerDto, CustomerDomain>();

            CreateMap<UserDomain, UserDto>();
        }
    }
}
