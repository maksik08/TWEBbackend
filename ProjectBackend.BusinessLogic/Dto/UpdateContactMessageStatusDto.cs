using System.ComponentModel.DataAnnotations;
using ProjectBackend.Domain.Entities;

namespace ProjectBackend.BusinessLogic.Dto
{
    public class UpdateContactMessageStatusDto
    {
        [Required]
        [EnumDataType(typeof(ContactMessageStatus))]
        public ContactMessageStatus Status { get; set; }
    }
}
