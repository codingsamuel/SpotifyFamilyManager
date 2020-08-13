using System;
using System.ComponentModel.DataAnnotations;

namespace SFM.Models
{
    public class Subscription
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long SpotifyUserId { get; set; }

        public virtual SpotifyUser SpotifyUser { get; set; }

        [Required]
        public int PaymentInterval { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public DateTime LastPayment { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public bool Active { get; set; }
    }
}