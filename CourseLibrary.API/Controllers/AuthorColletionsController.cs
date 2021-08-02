using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CourseLibrary.API.Controllers
{
    [ApiController]
    [Route("api/authorcollections")]
    public class AuthorColletionsController : Controller
    {
        private readonly ICourseLibraryRepository _repository;
        private readonly IMapper _mapper;

        public AuthorColletionsController(ICourseLibraryRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("({ids})", Name = "GetAuthorCollection")]
        public ActionResult<IEnumerable<AuthorDto>> GetAuthorCollecion([FromRoute][ModelBinder(typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if (ids is null)
                return BadRequest();

            var authorEntities = _repository.GetAuthors(ids);

            if (authorEntities.Count() != ids.Count())
                return NotFound();

            return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authorEntities));
        }

        [HttpPost]
        public ActionResult<IEnumerable<AuthorDto>> CreateAuthorCollection(IEnumerable<AuthorForCreationDto> authorCollection)
        {
            var authorEntities = _mapper.Map<IEnumerable<Author>>(authorCollection);
            authorEntities.ToList().ForEach(authorEntity => _repository.AddAuthor(authorEntity));
            _repository.Save();
            var autorCollectionToReturn = _mapper.Map<IEnumerable<AuthorDto>>(authorEntities);
            var idsAsString = string.Join(',', authorEntities.Select(a => a.Id));
            return CreatedAtRoute("GetAuthorCollection", new { ids = idsAsString }, autorCollectionToReturn);
        }
    }
}
