using API_BACKEND1.Models;

namespace API_BACKEND1.Models
{
public class SharedNote
{
    public int Id { get; set; }

    public int NoteId { get; set; }
    public Note Note { get; set; }

    // QUIÉN la comparte
    public int SharedByUserId { get; set; }
    public User SharedByUser { get; set; }

    //  A QUIÉN se le comparte
    public int SharedWithUserId { get; set; }
    public User SharedWithUser { get; set; }

    public DateTime SharedAt { get; set; } = DateTime.UtcNow;

    //  Permisos
    public bool CanRead { get; set; } = true;
}

}
