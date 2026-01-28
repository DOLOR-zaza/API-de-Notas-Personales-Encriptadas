namespace API_BACKEND1.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        // Aquí va el contenido encriptado
        public string EncryptedContent { get; set; } = string.Empty;

        // Relación con usuario
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
