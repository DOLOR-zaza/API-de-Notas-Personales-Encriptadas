namespace API_BACKEND1.Dtos
{
    public class SharedNoteResponseDto
    {
        public int NoteId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string SharedBy { get; set; } = string.Empty;
        public DateTime SharedAt { get; set; }
    }
}
