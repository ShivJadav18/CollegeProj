using System.IdentityModel.Tokens.Jwt;
using ElectroSphereProj.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ElectroSphereProj.ViewModel;
namespace ElectroSphereProj.Controllers;

public class CartController : Controller
{
    private readonly ElectronicDataBaseContext _context;

    public CartController(ElectronicDataBaseContext context)
    {
        _context = context;
    }

    public IActionResult CartView()
    {
        string token = Request.Cookies["jwtCookie"];
        int CustomerId = Convert.ToInt32(GetClaimValueHelper(token, "Userid"));

        Carttable carttable = _context.Carttables.FirstOrDefault(c => c.Customerid == CustomerId);
        CartViewModel cartViewModel = new CartViewModel{};
        List<cartItem> cartItems = new List<cartItem>{};
        cartViewModel.totalItems = 0;
        cartViewModel.Totalamount = 0;
        cartViewModel.cartItems = cartItems;

        if(carttable == null){
            return View(cartViewModel);
        }

        List<Cartdetailtable> cartdetailtables = _context.Cartdetailtables.Include(c=>c.Item).Where(c => c.Cartid == carttable.Cartid).ToList();

        foreach(Cartdetailtable cartdetailtable in cartdetailtables){
            cartItem cartItem = new cartItem{};
            cartItem.itemImg = cartdetailtable.Item.Imageurl;
            cartItem.itemId = cartdetailtable.Itemid;
            cartItem.itemName = cartdetailtable.Item.Name;
            cartItem.itemQun = (int)cartdetailtable.Itemqun;
            cartItem.itemPrice = (decimal) (cartdetailtable.Item.Rate * cartdetailtable.Itemqun);
            cartViewModel.Totalamount = cartItem.itemPrice + cartViewModel.Totalamount;
            cartItems.Add(cartItem);
        }
        cartViewModel.cartItems = cartItems;
        cartViewModel.totalItems = cartdetailtables.Count;

        return View(cartViewModel);
    }

    private string GetClaimValueHelper(string token, string claimType)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var jwtToken = tokenHandler.ReadJwtToken(token);

        var claim = jwtToken.Claims.FirstOrDefault(c => c.Type == claimType);

        return claim.Value;
    }
}