namespace API_BACKEND1.Dtos
{
    public class NoteCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty; // texto plano -> se encripta
    }

    public class NoteUpdateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    public class NoteResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty; // desencriptado al regresar
    }
}
