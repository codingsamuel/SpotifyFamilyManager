using System.ComponentModel.DataAnnotations;

namespace SFM.Models
{
    public class Config
    {
        [Key]
        public string Key { get; set; }

        [Required]
        public string Value { get; set; }
    }
}