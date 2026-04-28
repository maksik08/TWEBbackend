using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectBackend.api.Mappings;

namespace ProjectBackend.Tests.TestInfrastructure
{
    internal static class TestMapperFactory
    {
        private static readonly Lazy<IMapper> Mapper = new(() =>
        {
            var configuration = new MapperConfiguration(
                cfg => cfg.AddProfile<AutoMapperProfile>(),
                NullLoggerFactory.Instance);
            return configuration.CreateMapper();
        });

        public static IMapper Create()
        {
            return Mapper.Value;
        }
    }
}
