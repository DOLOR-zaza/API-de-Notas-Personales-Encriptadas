namespace API_BACKEND1.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        // Ej: "REGISTER", "LOGIN", "CREATE_NOTE", "SHARE_NOTE"
        public string Action { get; set; } = string.Empty;

        // Opcional: para ligar la acción a una nota
        public int? NoteId { get; set; }
        public Note? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
