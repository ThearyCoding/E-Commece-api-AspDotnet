using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace E_Commerce_Final.Models
{
    public class ItemImage
    {
        [Key]
        public Guid ItemImageId { get; set; }

        public string Image { get; set; } = string.Empty;

        [ForeignKey("Item")]
        public Guid ItemId { get; set; }

        [JsonIgnore]
        public Item Item { get; set; } = null!;
    }
}
