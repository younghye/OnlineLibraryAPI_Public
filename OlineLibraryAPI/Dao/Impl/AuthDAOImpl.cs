using Microsoft.IdentityModel.Tokens;
using OlineLibraryAPI.Models.Person_;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OlineLibraryAPI.Dao.Impl;
public class AuthDAOImpl : IAuthDAO
{
    public string GenerateJWTToken(Employee employee)
    {
        string? key = AppSettingsJson.GetAppSettings().GetValue<string>("Jwt:Key") ?? throw new ArgumentNullException(nameof(employee));
        var keyBytes = Encoding.UTF8.GetBytes(key);

        var claims = new List<Claim> {
            new("userID", employee.EmployeeID.ToString()),
            new(ClaimTypes.Role, employee.Role.ToString())
        };

        var jwtInfo = new JwtSecurityToken(
                issuer: AppSettingsJson.GetAppSettings().GetValue<string>("Jwt:Issuer"),
                audience: AppSettingsJson.GetAppSettings().GetValue<string>("Jwt:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256)
                );
        var Token = new JwtSecurityTokenHandler().WriteToken(jwtInfo);
        return Token;
    }
}



