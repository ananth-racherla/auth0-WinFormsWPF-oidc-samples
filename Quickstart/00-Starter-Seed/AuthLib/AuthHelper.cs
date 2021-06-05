using Auth0.OidcClient;
using IdentityModel.OidcClient.Browser;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace AuthLib
{
    public class AuthHelper
    {
        public AuthHelper(IAuth0Client client)
        {
            Client = client;
        }


        public IAuth0Client Client { get; }

        private string _accessToken;
        private JwtSecurityToken _jwtSecurityToken;
        private string _refreshToken;
        private string _idToken;

        public async Task<string> GetAccessToken()
        {
            if (_jwtSecurityToken.ValidTo > DateTime.UtcNow)
            {
                return _accessToken;
            }

            if (string.IsNullOrEmpty(_refreshToken))
            {
                throw new Exception("Failed to fetch access token");
            }

            var refreshTokenResult = await Client.RefreshTokenAsync(_refreshToken);

            if (refreshTokenResult.IsError)
            {
                throw new Exception(refreshTokenResult.Error);
            }

            _accessToken = refreshTokenResult.AccessToken;
            _idToken = refreshTokenResult.IdentityToken;
            _refreshToken = refreshTokenResult.RefreshToken;

            return string.Empty;
        }

        public string GetIdToken()
        {
            return _idToken;
        }

        public string GetRefreshToken()
        {
            return _refreshToken;
        }

        public async Task LoginAsync(object extraParameters)
        {
            var result = await Client.LoginAsync(extraParameters);
            if (result.IsError)
            {
                throw new Exception(result.Error);
            }

            _accessToken = result.AccessToken;
            _jwtSecurityToken = new JwtSecurityToken(_accessToken);
            _refreshToken = result.RefreshToken;
            _idToken = result.IdentityToken;
        }

        public async Task<bool> LogoutAsync()
        {
            var result = await Client.LogoutAsync();
            return result == BrowserResultType.Success;
        }
    }
}
