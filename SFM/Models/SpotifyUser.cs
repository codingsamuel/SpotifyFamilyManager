using System;
using System.ComponentModel.DataAnnotations;

namespace SFM.Models
{
    public class SpotifyUser
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string DisplayName { get; set; }

        [Required]
        public string Email { get; set; }

        public string ApiUrl { get; set; }

        [Required]
        public string SpotifyId { get; set; }
        
        public string ImageUrl { get; set; }

        [Required]
        public string Product { get; set; }
        
        public string Type { get; set; }

        public string Uri { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }
    }
}