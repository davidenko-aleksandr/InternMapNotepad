using SQLite;

namespace MapNotepad.Models
{
    public abstract class BaseModel
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
    }
}
