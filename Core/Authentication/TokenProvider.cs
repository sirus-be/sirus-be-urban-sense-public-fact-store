namespace Core.Authentication
{
    public class TokenProvider
    {
        private readonly TokenData TokenData = new();

        public void Set(string accessToken, string refreshToken)
        {
            TokenData.AccessToken = accessToken;
            TokenData.RefreshToken = refreshToken;
        }

        public TokenData Get()
        {
            return TokenData;
        }

        public string GetAccessToken()
        {
            return TokenData.AccessToken;
        }

        public void Remove()
        {
            TokenData.AccessToken = null;
            TokenData.RefreshToken = null;
        }
    }

    public class TokenData
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
