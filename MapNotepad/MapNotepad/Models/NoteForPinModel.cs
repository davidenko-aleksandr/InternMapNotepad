using SQLite;

namespace MapNotepad.Models
{
    [Table("Note")]
    public class NoteForPinModel : IBaseModel
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        public int PinId { get; set; }

        public int UserId { get; set; }

        public string NoteTitle { get; set; }

        public string NoteDescription { get; set; }

        public string Image { get; set; }
    }
}
