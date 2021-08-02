using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace CourseLibrary.API.Controllers
{
    [ApiController]
    [Route("api/authors/{authorId}/courses")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseLibraryRepository _repository;
        private readonly IMapper _mapper;

        public CoursesController(ICourseLibraryRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public ActionResult<IEnumerable<CourseDto>> GetCoursesForAuthor(Guid authorId)
        {
            if (!_repository.AuthorExists(authorId))
                return NotFound();

            var couresesForAuthorFromRepo = _repository.GetCourses(authorId);
            return Ok(_mapper.Map<IEnumerable<CourseDto>>(couresesForAuthorFromRepo));
        }

        [HttpGet("{courseId}", Name = "GetCourseForAuthor")]
        public ActionResult<CourseDto> GetCourseForAuthor(Guid authorId, Guid courseId)
        {
            if (!_repository.AuthorExists(authorId))
                return NotFound();

            var coureseForAuthorFromRepo = _repository.GetCourse(authorId, courseId);
            if (coureseForAuthorFromRepo is null)
                return NotFound();

            return Ok(_mapper.Map<CourseDto>(coureseForAuthorFromRepo));
        }

        [HttpPost]
        public ActionResult<CourseDto> CreateCourceForAuthor(Guid authorId, CourseForCreationDto course)
        {
            if (!_repository.AuthorExists(authorId))
                return NotFound();

            var courseEntitiy = _mapper.Map<Course>(course);
            _repository.AddCourse(authorId, courseEntitiy);
            _repository.Save();
            var courseToReturn = _mapper.Map<CourseDto>(courseEntitiy);
            return CreatedAtRoute("GetCourseForAuthor", new { authorId = courseToReturn.AuthorId, courseId = courseToReturn.Id }, courseToReturn);
        }

        [HttpPut("{courseId}")]
        public IActionResult UpdateCourseForAuthor(Guid authorId, Guid courseId, CourseForUpdateDto course)
        {
            if (_repository.AuthorExists(authorId) == false)
                return NotFound();

            var courseEntity = _repository.GetCourse(authorId, courseId);
            if (courseEntity is null)
            {
                var courseToAdd = _mapper.Map<Course>(course);
                courseToAdd.Id = courseId;
                _repository.AddCourse(authorId, courseToAdd);
                _repository.Save();
                var courseToReturn = _mapper.Map<CourseDto>(courseToAdd);
                return CreatedAtRoute("GetCourseForAuthor", new { authorId, courseId = courseToAdd.Id }, courseToReturn);
            }

            _mapper.Map(course, courseEntity);
            _repository.Save();

            return NoContent();
        }

        [HttpPatch("{courseId}")]
        public ActionResult PartiallyUpdateCourseForAuthor(Guid authorId, Guid courseId, JsonPatchDocument<CourseForUpdateDto> patchDocument)
        {
            if (_repository.AuthorExists(authorId) == false)
                return NotFound();

            var courseEntity = _repository.GetCourse(authorId, courseId);
            if (courseEntity is null)
            {
                var courseDto = new CourseForUpdateDto();
                patchDocument.ApplyTo(courseDto, ModelState);

                if (TryValidateModel(courseDto) == false)
                    return ValidationProblem(ModelState);

                var courseToAdd = _mapper.Map<Course>(courseDto);
                courseToAdd.Id = courseId;

                _repository.AddCourse(authorId, courseToAdd);
                _repository.Save();

                var courseToReturn = _mapper.Map<CourseDto>(courseToAdd);

                return CreatedAtRoute("GetCourseForAuthor", new { authorId, courseId = courseId }, courseToReturn);
            }

            var courseToPatch = _mapper.Map<CourseForUpdateDto>(courseEntity);
            patchDocument.ApplyTo(courseToPatch, ModelState);

            if (TryValidateModel(courseToPatch) == false)
                return ValidationProblem(ModelState);

            _mapper.Map(courseToPatch, courseEntity);
            _repository.Save();

            return NoContent();
        }

        [HttpDelete("{courseId}")]
        public IActionResult DeleteCourseForAuthor(Guid authorId, Guid courseId)
        {
            var course = _repository.GetCourse(authorId, courseId);
            if (course is null)
                return NotFound();
            _repository.DeleteCourse(course);
            _repository.Save();

            return NoContent();
        }

        public override ActionResult ValidationProblem([ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            var optiions = HttpContext.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (ActionResult)optiions.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
    }
}
