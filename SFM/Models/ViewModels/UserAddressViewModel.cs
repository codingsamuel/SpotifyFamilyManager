namespace SFM.Models.ViewModels
{
    public class UserAddressViewModel
    {
        public long SpotifyUserId { get; set; }

        public virtual SpotifyUser SpotifyUser { get; set; }

        public string Street { get; set; }

        public string Number { get; set; }

        public string PostCode { get; set; }

        public string City { get; set; }

        public string State { get; set; }
    }
}