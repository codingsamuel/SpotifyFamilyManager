using System.ComponentModel.DataAnnotations;

namespace SFM.Models
{
    public class UserAddress
    {
        [Key] public long Id { get; set; }

        public long SpotifyUserId { get; set; }

        public virtual SpotifyUser SpotifyUser { get; set; }

        public string Street { get; set; }

        public string Number { get; set; }

        public string PostCode { get; set; }

        public string City { get; set; }

        public string State { get; set; }
    }
}