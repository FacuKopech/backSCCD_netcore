using System.IdentityModel.Tokens.Jwt;

namespace SCCD.Helpers
{
    public static class JwtHelper
    {
        public static string GetClaimValueFromToken(string token, string claimType)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            return jwtToken.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
        }
    }
}
