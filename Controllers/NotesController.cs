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
    [Authorize]
    public class NotesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEncryptionService _encryption;

        // Si ya sembraste (seed) permisos así: READ=1, WRITE=2, esto funciona siempre.
        // Si no estás seguro, abajo también busco por Code ("READ"/"WRITE").
        private const int READ_PERMISSION_ID_FALLBACK = 1;

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

        // =========================
        // 1) MIS NOTAS (CRUD)
        // =========================

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

        [HttpGet("{id:int}")]
        public async Task<ActionResult<NoteResponseDto>> GetNote(int id)
        {
            var userId = GetUserId();

            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (note == null)
                return NotFound(new { message = "Nota no encontrada." });

            return Ok(new NoteResponseDto
            {
                Id = note.Id,
                Title = note.Title,
                Content = _encryption.Decrypt(note.EncryptedContent)
            });
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

            // Ojo: Devolvemos el content "plano" como confirmación, no el cifrado.
            return CreatedAtAction(nameof(GetNote), new { id = note.Id }, new NoteResponseDto
            {
                Id = note.Id,
                Title = note.Title,
                Content = dto.Content
            });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, NoteUpdateDto dto)
        {
            var userId = GetUserId();

            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (note == null)
                return NotFound(new { message = "Nota no encontrada." });

            note.Title = dto.Title;
            note.EncryptedContent = _encryption.Encrypt(dto.Content);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();

            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (note == null)
                return NotFound(new { message = "Nota no encontrada." });

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // =========================
        // 2) COMPARTIR NOTAS
        // =========================
        // POST /api/Notes/{noteId}/share/{userId}?permission=READ|WRITE
        // - Comparte una nota propia con otro usuario específico.
        // - Por default: READ
        // =========================

        [HttpPost("{noteId:int}/share/{userId:int}")]
        public async Task<IActionResult> ShareNoteWithUser(int noteId, int userId, [FromQuery] string? permission = "READ")
        {
            var currentUserId = GetUserId();

            // 1) Validar que la nota exista y sea del usuario actual
            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == noteId && n.UserId == currentUserId);

            if (note == null)
                return NotFound(new { message = "La nota no existe o no te pertenece." });

            // 2) Validar usuario destino
            var targetUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (targetUser == null)
                return NotFound(new { message = "El usuario destino no existe." });

            if (userId == currentUserId)
                return BadRequest(new { message = "No puedes compartirte una nota a ti mismo." });

            // 3) Evitar duplicados
            var alreadyShared = await _context.SharedNotes.AnyAsync(sn =>
                sn.NoteId == noteId &&
                sn.SharedByUserId == currentUserId &&
                sn.SharedWithUserId == userId);

            if (alreadyShared)
                return Conflict(new { message = "Esta nota ya fue compartida con ese usuario." });

            // 4) Resolver PermissionId (por Code)
            var permissionId = await ResolvePermissionIdAsync(permission ?? "READ");

            var shared = new SharedNote
            {
                NoteId = noteId,
                SharedByUserId = currentUserId,
                SharedWithUserId = userId,
                PermissionId = permissionId,
                SharedAt = DateTime.UtcNow
            };

            _context.SharedNotes.Add(shared);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Nota compartida correctamente",
                noteId,
                sharedWithUserId = userId,
                permission = (permission ?? "READ").ToUpperInvariant()
            });
        }

        // =========================
        // 3) LISTADOS
        // =========================

        // GET /api/Notes/shared/by-me
        // Todo lo que YO compartí
        [HttpGet("shared/by-me")]
        public async Task<IActionResult> GetSharedByMe()
        {
            var userId = GetUserId();

            var list = await _context.SharedNotes
                .Include(sn => sn.Note)
                .Include(sn => sn.SharedWithUser)
                .Include(sn => sn.Permission)
                .Where(sn => sn.SharedByUserId == userId)
                .Select(sn => new
                {
                    noteId = sn.NoteId,
                    title = sn.Note.Title,
                    sharedWith = sn.SharedWithUser.Username,
                    permission = sn.Permission != null ? sn.Permission.Code : "UNKNOWN",
                    sharedAt = sn.SharedAt
                })
                .ToListAsync();

            return Ok(list);
        }

        // GET /api/Notes/shared/with-me
        // Todo lo que me compartieron a MÍ (solo si tengo permiso READ o WRITE)
        [HttpGet("shared/with-me")]
        public async Task<IActionResult> GetSharedWithMe()
        {
            var userId = GetUserId();

            var list = await _context.SharedNotes
                .Include(sn => sn.Note)
                .Include(sn => sn.SharedByUser)
                .Include(sn => sn.Permission)
                .Where(sn => sn.SharedWithUserId == userId)
                .Select(sn => new
                {
                    noteId = sn.NoteId,
                    title = sn.Note.Title,
                    sharedBy = sn.SharedByUser.Username,
                    permission = sn.Permission != null ? sn.Permission.Code : "UNKNOWN",
                    sharedAt = sn.SharedAt
                })
                .ToListAsync();

            return Ok(list);
        }

        // =========================
        // 4) LEER UNA NOTA COMPARTIDA (DESENCRIPTADA)
        // =========================
        // GET /api/Notes/shared/with-me/{noteId}
        // - Me permite leer el contenido si me la compartieron (READ o WRITE)
        // =========================

        [HttpGet("shared/with-me/{noteId:int}")]
        public async Task<IActionResult> GetSharedNoteContent(int noteId)
        {
            var userId = GetUserId();

            var share = await _context.SharedNotes
                .Include(sn => sn.Note)
                .Include(sn => sn.SharedByUser)
                .Include(sn => sn.Permission)
                .FirstOrDefaultAsync(sn => sn.SharedWithUserId == userId && sn.NoteId == noteId);

            if (share == null)
                return NotFound(new { message = "No tienes acceso a esa nota compartida." });

            // Permitir READ o WRITE
            var code = share.Permission?.Code?.ToUpperInvariant();
            if (code != "READ" && code != "WRITE")
                return Forbid();

            return Ok(new
            {
                id = share.Note.Id,
                title = share.Note.Title,
                content = _encryption.Decrypt(share.Note.EncryptedContent),
                sharedBy = share.SharedByUser.Username,
                permission = code,
                sharedAt = share.SharedAt
            });
        }

        // =========================
        // Helpers
        // =========================

        private async Task<int> ResolvePermissionIdAsync(string permissionCode)
        {
            var code = permissionCode.Trim().ToUpperInvariant();

            // Si tienes tabla Permissions, esta es la forma correcta:
            var perm = await _context.Set<Permission>()
                .FirstOrDefaultAsync(p => p.Code.ToUpper() == code);

            if (perm != null)
                return perm.Id;

            // Fallback (si no existe la tabla o no está sembrada)
            if (code == "READ")
                return READ_PERMISSION_ID_FALLBACK;

            // Si pidieron WRITE pero no existe, avisamos.
            throw new InvalidOperationException("No existe el permiso solicitado. Asegúrate de sembrar Permissions (READ/WRITE).");
        }
    }
}
