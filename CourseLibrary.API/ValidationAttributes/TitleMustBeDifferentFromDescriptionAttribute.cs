using System.ComponentModel.DataAnnotations;

namespace CourseLibrary.API.ValidationAttributes
{
    public class TitleMustBeDifferentFromDescriptionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var titleProperty = value.GetType().GetProperty("Title");
            var descriptionProperty = value.GetType().GetProperty("Description");

            var title = titleProperty.GetValue(value)?.ToString();
            var description = descriptionProperty.GetValue(value)?.ToString();

            if (title == description)
                return new ValidationResult(GetErrorMessage(), new[] { validationContext.ObjectType.Name });

            return ValidationResult.Success;
        }

        private string GetErrorMessage()
        {
            return string.IsNullOrEmpty(ErrorMessage) ? "Title must be different from description (Default)(From attribute)" : ErrorMessage;
        }
    }
}
