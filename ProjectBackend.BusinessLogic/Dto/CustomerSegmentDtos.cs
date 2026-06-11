namespace ProjectBackend.BusinessLogic.Dto
{
    public class CustomerSegmentDto { public int Id { get; set; } public string Name { get; set; } = string.Empty; public string? Description { get; set; } }
    public class CreateCustomerSegmentDto { public string Name { get; set; } = string.Empty; public string? Description { get; set; } }
    public class AssignSegmentDto { public int CustomerId { get; set; } }
}
