using System;

namespace CourseLibrary.API.Helpers
{
    public static class DateTimeOffsetExtensions
    {
        public static int GetCurrentAge(this DateTimeOffset dateTimeOffset)
        {
            var age = DateTime.Now.Year - dateTimeOffset.Year;
            if (DateTime.Now < dateTimeOffset.AddYears(age))
                age--;

            return age;
        }
    }
}
