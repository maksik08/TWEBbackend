using AutoMapper;
using ProjectBackend.Domain.Entities;
using ProjectBackend.BusinessLogic.Dto;

namespace ProjectBackend.BusinessLogic.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ProductsDomain, ProductDto>()
                .ForMember(dest => dest.Category,
                    opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
                .ForMember(dest => dest.Supplier,
                    opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.Name : null))
                .ForMember(dest => dest.AvailabilityState,
                    opt => opt.MapFrom(src => src.Availability))
                .ForMember(dest => dest.Availability,
                    opt => opt.MapFrom(src =>
                        src.IsPreorder ? "preorder"
                        : src.StockQuantity <= 0 ? "out-of-stock"
                        : src.Availability == ProductAvailability.Limited ? "limited"
                        : src.Availability == ProductAvailability.Preorder ? "preorder"
                        : src.Availability == ProductAvailability.OutOfStock ? "out-of-stock"
                        : "in-stock"));
            CreateMap<ProductSpecification, ProductSpecificationDto>().ReverseMap();
            CreateMap<ProductDto, ProductsDomain>()
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.Supplier, opt => opt.Ignore())
                .ForMember(dest => dest.Availability,
                    opt => opt.MapFrom(src => src.AvailabilityState))
                .ForMember(dest => dest.RowVersion, opt => opt.Ignore());
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
            CreateMap<UserDomain, InstallerLookupDto>();
            CreateMap<CreateUserDto, UserDomain>()
                .ForMember(dest => dest.Password, opt => opt.Ignore());
            CreateMap<UpdateUserDto, UserDomain>()
                .ForMember(dest => dest.Password, opt => opt.Ignore());

            CreateMap<OrderItemDomain, OrderItemDto>()
                .ForMember(dest => dest.LineTotal, opt => opt.MapFrom(src => src.Quantity * src.UnitPrice));
            CreateMap<OrderDomain, OrderDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.Username : null));

            CreateMap<ServiceRequestCommentDomain, ServiceRequestCommentDto>()
                .ForMember(dest => dest.AuthorUsername,
                    opt => opt.MapFrom(src => src.AuthorUser != null ? src.AuthorUser.Username : string.Empty));
            CreateMap<WorkPhotoDomain, WorkPhotoDto>();
            CreateMap<ServiceRequestDomain, ServiceRequestDto>()
                .ForMember(dest => dest.CustomerUsername,
                    opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Username : string.Empty))
                .ForMember(dest => dest.InstallerUsername,
                    opt => opt.MapFrom(src => src.Installer != null ? src.Installer.Username : null))
                .ForMember(dest => dest.ManagerUsername,
                    opt => opt.MapFrom(src => src.Manager != null ? src.Manager.Username : null));
            CreateMap<NotificationDomain, NotificationDto>();
            CreateMap<PaymentTransactionDomain, PaymentTransactionDto>()
                .ForMember(dest => dest.Username,
                    opt => opt.MapFrom(src => src.User != null ? src.User.Username : null));

            CreateMap<ProductReviewDomain, ProductReviewDto>()
                .ForMember(dest => dest.AuthorUsername,
                    opt => opt.MapFrom(src => src.User != null ? src.User.Username : string.Empty));
        }
    }
}
