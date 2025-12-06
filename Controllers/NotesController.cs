using System.Security.Claims;
using API_BACKEND1.Data;
using API_BACKEND1.Dtos;
using API_BACKEND1.Models;
using API_BACKEND1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_BACKEND1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // 🔐 todas las rutas requieren JWT
    public class NotesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEncryptionService _encryption;

        public NotesController(AppDbContext context, IEncryptionService encryption)
        {
            _context = context;
            _encryption = encryption;
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(claim!.Value);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NoteResponseDto>>> GetNotes()
        {
            var userId = GetUserId();

            var notes = await _context.Notes
                .Where(n => n.UserId == userId)
                .ToListAsync();

            var result = notes.Select(n => new NoteResponseDto
            {
                Id = n.Id,
                Title = n.Title,
                Content = _encryption.Decrypt(n.EncryptedContent)
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NoteResponseDto>> GetNote(int id)
        {
            var userId = GetUserId();

            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (note == null)
                return NotFound();

            return new NoteResponseDto
            {
                Id = note.Id,
                Title = note.Title,
                Content = _encryption.Decrypt(note.EncryptedContent)
            };
        }

        [HttpPost]
        public async Task<ActionResult<NoteResponseDto>> Create(NoteCreateDto dto)
        {
            var userId = GetUserId();

            var note = new Note
            {
                Title = dto.Title,
                EncryptedContent = _encryption.Encrypt(dto.Content),
                UserId = userId
            };

            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNote), new { id = note.Id }, new NoteResponseDto
            {
                Id = note.Id,
                Title = note.Title,
                Content = dto.Content
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, NoteUpdateDto dto)
        {
            var userId = GetUserId();

            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (note == null)
                return NotFound();

            note.Title = dto.Title;
            note.EncryptedContent = _encryption.Encrypt(dto.Content);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();

            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (note == null)
                return NotFound();

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
