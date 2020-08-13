using System.ComponentModel.DataAnnotations;

namespace SFM.Models
{
    public class SpotifyUser
    {
        [Key]
        public long Id { get; set; }
    }
}