using SQLite;
using Xamarin.Forms;

namespace MapNotepad.Models
{
    [Table("PinGoogleMap")]
    public class PinGoogleMap : IBaseModel
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        public int User_Id { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Label { get; set; }

        public string Description { get; set; }

        public bool IsFavorite { get; set; }
        public string Image { get; set; }

    }
}
