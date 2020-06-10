using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using AngularMongoASP.Models;
using AngularMongoASP.Services;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace AngularMongoASP.Controllers
{
    [Produces("application/json")]
    [Consumes("application/json", "multipart/form-data")]
    [ApiController]
    [Route("[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly BookService _bookService;
        private readonly FileService _fileService;

        public BooksController(BookService bookService, FileService fileService)
        {
            _bookService = bookService;
            _fileService = fileService;
        }

        [HttpGet]
        public ActionResult<List<Book>> Get() => _bookService.Get();

        [HttpGet("{id:length(24)}", Name = "GetBook")]
        public ActionResult<Book> Get(string id)
        {
            var book = _bookService.Get(id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }
        public async Task<Book> Create([FromForm(Name = "icon")]IFormFile file, [FromForm(Name = "body")] string body)
        {
            Book book = JsonConvert.DeserializeObject<Book>(body);
            return await _bookService.Create(book, file);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Book bookIn)
        {
            var book = _bookService.Get(id);

            if (book == null)
            {
                return NotFound();
            }

            _bookService.Update(id, bookIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var book = _bookService.Get(id);
            _fileService.DeleteFile(book.IconId);
            if (book == null)
            {
                return NotFound();
            }

            _bookService.Remove(book.Id);

            return NoContent();
        }
    }
}