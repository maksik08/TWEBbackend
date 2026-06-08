using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.BusinessLogic.Exceptions;
using ProjectBackend.Domain.Entities;

namespace ProjectBackend.BusinessLogic.Services
{
    /// <summary>
    /// Server-side port of the frontend installation-price calculator (calculate.ts).
    /// The equipment-installation portion is recomputed from the order's real unit prices,
    /// so the client cannot understate the services total.
    /// </summary>
    public static class OrderServicesCalculator
    {
        private static readonly IReadOnlyDictionary<string, decimal> ObjectBaseMap = new Dictionary<string, decimal>
        {
            ["flat"] = 100m,
            ["house"] = 200m,
            ["small_office"] = 350m,
            ["medium_office"] = 600m,
        };

        private static readonly IReadOnlyDictionary<string, decimal> InstallationTypeMap = new Dictionary<string, decimal>
        {
            ["wireless"] = 80m,
            ["fiber"] = 180m,
            ["copper"] = 120m,
        };

        private const decimal WorkUnitPrice = 50m;

        public static decimal Calculate(CreateOrderServicesDto services, IEnumerable<OrderItemDomain> items)
        {
            if (!ObjectBaseMap.TryGetValue(services.ObjectType, out var objectBase))
            {
                throw new ValidationException($"Unknown object type '{services.ObjectType}'.");
            }

            if (!InstallationTypeMap.TryGetValue(services.InstallationType, out var installationType))
            {
                throw new ValidationException($"Unknown installation type '{services.InstallationType}'.");
            }

            var worksPrice = services.Works.Count * WorkUnitPrice;
            var staffCount = Math.Max(0, services.StaffCount);
            var staffRate = Math.Max(0m, services.StaffRate);
            var installationPrice = Math.Max(0m, services.InstallationCost);
            var deliveryPrice = Math.Max(0m, services.DeliveryCost);
            var staffPrice = staffCount * staffRate;

            var equipmentPrice = items.Sum(item =>
            {
                var quantity = Math.Max(0, item.Quantity);
                var unitPrice = Math.Max(0m, item.UnitPrice);
                var installationUnit = Math.Max(12m, Math.Round(unitPrice * 0.08m, MidpointRounding.AwayFromZero));
                return installationUnit * quantity;
            });

            return objectBase
                + installationType
                + worksPrice
                + equipmentPrice
                + staffPrice
                + installationPrice
                + deliveryPrice;
        }
    }
}
