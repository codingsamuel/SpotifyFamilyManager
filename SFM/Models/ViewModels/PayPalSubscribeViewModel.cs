namespace SFM.Models.ViewModels
{
    public class PayPalSubscribeViewModel
    {
        public PayPalSubscribeViewModel(string link, string token)
        {
            Link = link;
            Token = token;
        }

        public string Link { get; set; }

        public string Token { get; set; }
    }
}