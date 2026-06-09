namespace ProjectBackend.BusinessLogic.Dto
{
    public class CouponListRequestDto : ListQueryRequestDto
    {
        public string? Search { get; set; }

        public bool? IsActive { get; set; }
    }
}
