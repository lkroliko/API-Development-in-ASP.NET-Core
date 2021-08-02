namespace CourseLibrary.API.Models
{
    public class CourseForCreationDto : CourseForManipulationDto //: IValidatableObject
    {
        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if (Title == Description)
        //        yield return new ValidationResult("Title must be different from description (From IValidatableObject)", new[] { nameof(CourseForCreationDto) });
        //}
    }
}
