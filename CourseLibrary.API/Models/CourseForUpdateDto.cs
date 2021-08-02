using System.ComponentModel.DataAnnotations;

namespace CourseLibrary.API.Models
{

    public class CourseForUpdateDto : CourseForManipulationDto
    {
        [Required]
        [MaxLength(1500)]
        public override string  Description { get => base.Description; set => base.Description = value; }
    }
}
