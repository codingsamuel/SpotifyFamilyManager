using System.ComponentModel.DataAnnotations;

namespace SFM.Models
{
    public class Config
    {
        public const string DEBUG = "DEBUG";
        public const string PAYPAL_CLIENT_ID = "PAYPAL_CLIENT_ID";
        public const string PAYPAL_SECRET = "PAYPAL_SECRET";
        public const string PAYPAL_TEST_CLIENT_ID = "PAYPAL_TEST_CLIENT_ID";
        public const string PAYPAL_TEST_SECRET = "PAYPAL_TEST_SECRET";
        public const string SPOTIFY_PRICE = "SPOTIFY_PRICE";
        public const string SPOTIFY_MEMBERS = "SPOTIFY_MEMBERS";
        public const string SPOTIFY_CLIENT_ID = "SPOTIFY_CLIENT_ID";
        public const string SPOTIFY_CLIENT_SECRET = "SPOTIFY_CLIENT_SECRET";
        public const string BASE_URL = "BASE_URL";

        [Key] public long Id { get; set; }

        [Required] [MaxLength(100)] public string Key { get; set; }

        [Required] [MaxLength(255)] public string Value { get; set; }
    }
}