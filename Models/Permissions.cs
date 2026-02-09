namespace API_BACKEND1.Models
{
    public class Permission
    {
        public int Id { get; set; }

        // Ej: "READ", "WRITE"
        public string Code { get; set; } = string.Empty;

        // Ej: "Solo lectura", "Lectura y escritura"
        public string Description { get; set; } = string.Empty;

        // Navegación (opcional, ayuda en UML)
        public List<SharedNote> SharedNotes { get; set; } = new();
    }
}
