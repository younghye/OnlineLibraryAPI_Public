using OlineLibraryAPI.Models.Person_;

namespace OlineLibraryAPI.Models
{
    public class Auth
    {
        public required string Token { get; set; }
        public required virtual Employee Employee { get; set; }
        //public string RefreshToken { get; set; }
    }
}
