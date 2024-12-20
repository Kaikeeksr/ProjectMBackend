using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DotNetEnv;

namespace ProjectMBackend.AuthModel
{
    public class Auth
    {
        internal string GenerateJwt(Models.User user)
        {
            Env.Load();

            string secretKey = Environment.GetEnvironmentVariable("JWT_KEY")
                ?? throw new InvalidOperationException("JWT_KEY não encontrada nas variáveis de ambiente");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),      
                new Claim("username", user.Username),                       
                new Claim("name", $"{user.FirstName} {user.LastName}")      
            };

            
            var token = new JwtSecurityToken(
                issuer: "ProjectM", 
                audience: "Users", 
                claims: claims,
                expires: DateTime.Now.AddHours(1), 
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
