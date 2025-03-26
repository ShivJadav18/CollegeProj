using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ElectroSphereProj.Helpers;

public class JwtHelper{

    public string GenerateJSONWebToken(Claim[] claims)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MyNameis_Shiv_Jadav_018_Tatvasoft_007_James_Bond"));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "Pizzashop App",
            audience: "dotnetclient",
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials,
            claims: claims
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    

}