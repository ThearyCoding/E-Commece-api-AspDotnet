using Newtonsoft.Json;using System.ComponentModel.DataAnnotations;using System.ComponentModel.DataAnnotations.Schema;namespace E_Commerce_Final.Models{    public class Item    {        [Key]        public Guid ItemId { get; set; }        public Guid CategoryId { get; set; }        [Required]        [MaxLength(50)]        public string ItemName { get; set; } = string.Empty;        public string Description { get; set; } = string.Empty;        public double Price { get; set; }        public int QtyOnHand { get; set; }        public bool IsActive { get; set; }

        
        public List<ItemImage> Images { get; set; } = new();
        [NotMapped]
        [JsonProperty("files")] 
        public List<IFormFile> Files { get; set; } = new();    }}