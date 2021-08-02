using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;
using CourseLibrary.API.Helpers;

namespace CourseLibrary.API.Profiles
{
    public class AuthorProfile : Profile
    {
        public AuthorProfile()
        {
            CreateMap<Author, AuthorDto>()
                .ForMember(d => d.Age, options => options.MapFrom(s => s.DateOfBirth.GetCurrentAge()))
                .ForMember(d => d.Name, options => options.MapFrom(s => $"{s.FirstName} {s.LastName}"));

            CreateMap<AuthorForCreationDto, Author>();
        }
    }
}
