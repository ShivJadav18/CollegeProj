using Microsoft.AspNetCore.Mvc;
using ElectroSphereProj.ViewModel;
using ElectroSphereProj.Data;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using ElectroSphereProj.Helpers;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ElectroSphereProj.Controllers;

public class AuthController : Controller{

    private readonly ElectronicDataBaseContext _context;
    

    public AuthController(ElectronicDataBaseContext context){
       
        _context = context;
    }
    
    public IActionResult Login()
    {
        var token = Request.Cookies["jwtCookie"];
        // if (token != null)
        // {
        //     return RedirectToAction("Dashboardpage", "Dashboard");
        // }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(Userlogin user)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }
        Customer userobj = _context.Customers.FirstOrDefault(u => u.Email == user.Email && u.Isdeleted == false );

        if(userobj == null){
            TempData["error"] = "Email/Password is Wrong Or This user's status is in-active or this user is deleted.";
            return View();
        }
        // !BCrypt.Net.BCrypt.Verify(user.Password, userobj.Password)
        if (userobj.Password != user.Password )
        {
            TempData["error"] = "Email/Password is Wrong Or This user's status is in-active or this user is deleted.";
            return View();
        }
        

        var claims = new[] {
                new Claim("UserName",userobj.Firstname),
                new Claim("Userid",userobj.CustomerId.ToString())
            };

        var token = GenerateJSONWebToken(claims);
        // var jwtToken = _userauth.GetAuthenticate(user);
        // if (jwtToken == "")
        // {
        //     TempData["error"] = "Email/Password is Wrong Or This user's status is in-active or this user is deleted.";
        //     return View();
        // }
        if (user.rememberme)
        {
            SetJWTCookie(token, 7, "jwtCookie");
        }
        else
        {
            SetJWTCookie(token, 1, "jwtCookie");
        }
        TempData["success"] = "You are Successfully Logged In!";
        // return RedirectToAction("Dashboardpage", "Dashboard");
        return View();
    }

    private void SetJWTCookie(string token, int days, string name)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(days),
        };
        Response.Cookies.Append(name, token, cookieOptions);
    }
    private string GenerateJSONWebToken(Claim[] claims)
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