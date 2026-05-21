using System.ComponentModel.DataAnnotations;

namespace ProjectBackend.BusinessLogic.Dto
{
    public class AssignInstallerDto
    {
        [Range(1, int.MaxValue)]
        public int InstallerId { get; set; }

        public DateTime? ScheduledVisitAt { get; set; }
    }
}
