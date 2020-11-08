using SQLite;

namespace MapNotepad.Models
{
    [Table("PinGoogleMap")]
    public class PinGoogleMapModel : IBaseModel
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        public int UserId { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Label { get; set; }

        public string Description { get; set; }

        public bool IsFavorite { get; set; }

        public int CountOfNote { get; set; }
        public string LableForCountOfNote { get; set; }
    }
}
