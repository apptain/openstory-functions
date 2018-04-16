using System;
using System.Linq;
using Newtonsoft.Json;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

using static System.Environment;
using User = Apptain.Functions.Domain.User;

namespace Apptain.Functions.Providers
{
  public static class JwtProvider
  {
    private static string JwtKey;
    static JwtProvider()
    {
      JwtKey = GetEnvironmentVariable("JwtKey", EnvironmentVariableTarget.Process);
    }
  
    /// <summary>
    /// Creates jwt with ClaimIdentity encrypted, serialized and stored in UserData
    /// </summary>
    /// <param name="user"></param>
    /// <returns>jwt string</returns>
    public static string GenerateToken(User user)
    {
      var symmetricKey = Convert.FromBase64String(JwtKey);
      var tokenHandler = new JwtSecurityTokenHandler();

      var now = DateTime.UtcNow;

      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Audience = user.Id.ToString(),
        Subject = new ClaimsIdentity(new[]
        {
          new Claim(ClaimTypes.UserData, JsonConvert.SerializeObject(user))
        }),
        Expires = DateTime.MaxValue,
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
      };

      var stoken = tokenHandler.CreateToken(tokenDescriptor);
      var token = tokenHandler.WriteToken(stoken);

      return token;
    }

    /// <summary>
    /// Reads user from jwt
    /// </summary>
    /// <param name="token"></param>
    /// <returns>ClaimsPrincipal</returns>
    public static ClaimsPrincipal GetPrincipal(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

        if (jwtToken == null)
          return null;

        var symmetricKey = Convert.FromBase64String(JwtKey);

        var validationParameters = new TokenValidationParameters()
        {
          RequireExpirationTime = false,
          ValidateIssuer = false,
          ValidateAudience = false,
          ValidateLifetime = false,
          IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
        };

        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);

        return principal;
    }

    /// <summary>
    /// Wraps GetPricipal to return User entity
    /// </summary>
    /// <param name="claimsPrincipal"></param>
    /// <returns>User domain entity</returns>
    public static User ClaimsUser(string token)
    { 
      ClaimsPrincipal claimsPrincipal = GetPrincipal(token);
      string userData = claimsPrincipal.Claims
            .First(x => x.Type == ClaimTypes.UserData).Value;

      return JsonConvert.DeserializeObject<User>(userData);
    }
  }
}
