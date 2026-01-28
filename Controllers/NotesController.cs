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
    [Authorize] // todas las rutas requieren JWT
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
            {
                return NotFound();
            }

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

            return CreatedAtAction(
                nameof(GetNote),
                new { id = note.Id },
                new NoteResponseDto
                {
                    Id = note.Id,
                    Title = note.Title,
                    Content = dto.Content
                }
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, NoteUpdateDto dto)
        {
            var userId = GetUserId();

            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (note == null)
            {
                return NotFound();
            }

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
            {
                return NotFound();
            }

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/share")]
        public async Task<IActionResult> ShareNote(int id)
        {
            var userId = GetUserId();

            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (note == null)
            {
                return NotFound("La nota no existe o no te pertenece.");
            }

            var sharedNote = new SharedNote
            {
                NoteId = note.Id,
                SharedByUserId = userId
            };

            _context.SharedNotes.Add(sharedNote);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Nota compartida correctamente" });
        }

        [HttpGet("shared")]
        public async Task<ActionResult<IEnumerable<SharedNoteResponseDto>>> GetSharedNotes()
        {
            var sharedNotes = await _context.SharedNotes
                .Include(sn => sn.Note)
                .Include(sn => sn.SharedByUser)
                .Select(sn => new SharedNoteResponseDto
                {
                    NoteId = sn.NoteId,
                    Title = sn.Note.Title,
                    SharedBy = sn.SharedByUser.Username,
                    SharedAt = sn.SharedAt
                })
                .ToListAsync();

            return Ok(sharedNotes);
        }

        [HttpPost("{noteId}/share/{userId}")]
        public async Task<IActionResult> ShareNoteWithUser(int noteId, int userId)
        {
            var currentUserId = GetUserId();

            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == noteId && n.UserId == currentUserId);

            if (note == null)
            {
                return NotFound("La nota no existe o no te pertenece.");
            }

            var targetUserExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!targetUserExists)
            {
                return NotFound("El usuario destino no existe.");
            }

            var alreadyShared = await _context.SharedNotes.AnyAsync(sn =>
                sn.NoteId == noteId &&
                sn.SharedWithUserId == userId);

            if (alreadyShared)
            {
                return BadRequest("La nota ya fue compartida con este usuario.");
            }

            var sharedNote = new SharedNote
            {
                NoteId = noteId,
                SharedByUserId = currentUserId,
                SharedWithUserId = userId,
                CanRead = true
            };

            _context.SharedNotes.Add(sharedNote);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Nota compartida correctamente" });
        }

        [HttpGet("shared/by-me")]
        public async Task<IActionResult> GetNotesSharedByMe()
        {
            var userId = GetUserId();

            var notes = await _context.SharedNotes
                .Where(sn => sn.SharedByUserId == userId)
                .Include(sn => sn.Note)
                .Include(sn => sn.SharedWithUser)
                .Select(sn => new
                {
                    sn.NoteId,
                    sn.Note.Title,
                    SharedWith = sn.SharedWithUser.Username,
                    sn.SharedAt,
                    sn.CanRead
                })
                .ToListAsync();

            return Ok(notes);
        }

        [HttpGet("shared/with-me")]
        public async Task<IActionResult> GetNotesSharedWithMe()
        {
            var userId = GetUserId();

            var notes = await _context.SharedNotes
                .Where(sn => sn.SharedWithUserId == userId && sn.CanRead)
                .Include(sn => sn.Note)
                .Include(sn => sn.SharedByUser)
                .Select(sn => new
                {
                    sn.NoteId,
                    sn.Note.Title,
                    SharedBy = sn.SharedByUser.Username,
                    sn.SharedAt
                })
                .ToListAsync();

            return Ok(notes);
        }
    }
}
