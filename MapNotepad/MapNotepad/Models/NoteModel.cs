namespace MapNotepad.Models
{
    public class NoteModel : BaseModel
    {
        public int NoteId { get; set; }
        public int PinId { get; set; }
        public string NoteTitle { get; set; }
        public string NoteDescription { get; set; }
    }
}
