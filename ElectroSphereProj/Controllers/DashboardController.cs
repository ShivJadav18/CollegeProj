using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ElectroSphereProj.Data;
using ElectroSphereProj.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Iana;

namespace ElectroSphereProj.Controllers;

public class DashboardController : Controller
{

    private ElectronicDataBaseContext _context;

    public DashboardController(ElectronicDataBaseContext context)
    {
        _context = context;
    }
    // [Authorize]
    public IActionResult Dashboardpage()
    {
        var token = Request.Cookies["jwtCookie"];
        if (token == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        int CustomerId = Convert.ToInt32(GetClaimValueHelper(token, "Userid"));

        List<Item> items = _context.Items.Include(i => i.Category).Where(i => i.Isdeleted == false && i.Isavailable == true).ToList();
        List<ProductListViewModel> productListViewModels = new List<ProductListViewModel> { };
        foreach (Item item in items)
        {
            ProductListViewModel productListViewModel = new ProductListViewModel { };
            productListViewModel.producId = item.ItemId;
            productListViewModel.productDescription = item.Description;
            productListViewModel.productImgUrl = item.Imageurl;
            productListViewModel.productName = item.Name;
            productListViewModel.productPrice = (decimal)item.Rate;
            productListViewModel.companyName = item.Category.Categoryname;
            productListViewModels.Add(productListViewModel);
        }

        DashBoardView dashBoardView = new DashBoardView
        {
            productListView = productListViewModels
        };

        Carttable carttable = _context.Carttables.FirstOrDefault(c => c.Customerid == CustomerId);

        if(carttable == null){
            dashBoardView.cartCount = 0;
        }else{
            dashBoardView.cartCount = _context.Cartdetailtables.Where(c => c.Cartid == carttable.Cartid && c.Customerid == CustomerId).Count();
        }


        return View(dashBoardView);
    }

    public IActionResult ProductDetailView(int id)
    {
        Item item = _context.Items.Include(i => i.Category).FirstOrDefault(i => i.ItemId == id);

        return View(item);
    }

    public async Task<IActionResult> GetItemsByType(int typeId = 1)
    {
        // Query items matching the typeId
        // Include the 'Type' navigation property to access 'Typename' later
        // Add filtering for active/available items if needed (recommended)
        var items = await _context.Items
                                  .Include(item => item.Type)
                                  .Include(i => i.Category)// Eager load the related Typetable
                                  .Where(item => item.Typeid == typeId &&
                                                 item.Isdeleted != true && // Example: Only show non-deleted items
                                                 item.Isavailable == true) // Example: Only show available items
                                  .ToListAsync(); // Execute the query asynchronously
        Console.WriteLine(items + "items");
        var productListViewModels = new List<ProductListViewModel> { };
        foreach (Item item in items)
        {
            ProductListViewModel productListViewModel = new ProductListViewModel { };
            productListViewModel.producId = item.ItemId;
            productListViewModel.productDescription = item.Description;
            productListViewModel.productImgUrl = item.Imageurl;
            productListViewModel.productName = item.Name;
            productListViewModel.productPrice = (decimal)item.Rate;
            productListViewModel.companyName = item.Category.Categoryname;
            productListViewModels.Add(productListViewModel);
        }
        // var productViewModels = items.Select(item => new ProductListViewModel
        // {
        //     producId = item.ItemId,
        //     productName = item.Name ?? "N/A", // Provide default if Name is unexpectedly null
        //     // Assuming Typename maps to CompanyName. Requires Type to be loaded.
        //     companyName = item.Type?.Typename ?? "Unknown Company",
        //     productDescription = item.Description ?? string.Empty, // Use empty string if Description is null
        //     productPrice = item.Rate ?? 0m, // Use 0 if Rate is null, adjust default as needed
        //     productImgUrl = item.Imageurl ?? "/images/placeholder.png" // Use a placeholder if Imageurl is null
        // });


        return PartialView("_Products", productListViewModels);
    }

    public IActionResult AddToCart(int itemid)
    {
        try
        {
            string token = Request.Cookies["jwtCookie"];
            int CustomerId = Convert.ToInt32(GetClaimValueHelper(token, "Userid"));

            Carttable carttable = _context.Carttables.FirstOrDefault(c => c.Customerid == CustomerId);

            if (carttable == null)
            {
                carttable = new Carttable
                {
                    Customerid = CustomerId
                };
                _context.Add(carttable);
                _context.SaveChanges();
            }

            Cartdetailtable cartdetailtable = _context.Cartdetailtables.FirstOrDefault(c => c.Cartid == carttable.Cartid && c.Customerid == CustomerId && c.Itemid == itemid);
            if(cartdetailtable != null){
                cartdetailtable.Itemqun = cartdetailtable.Itemqun + 1;
                _context.SaveChanges();
            }else{
                cartdetailtable = new Cartdetailtable{};
                cartdetailtable.Cartid = carttable.Cartid;
                cartdetailtable.Customerid = carttable.Customerid;
                cartdetailtable.Itemid = itemid;

                _context.Add(cartdetailtable);
                _context.SaveChanges();
            }

            return Json(new { success = true });
        }
        catch (Exception e)
        {
            return Json(new { success = false, message = e.Message });
        }
    }

    private string GetClaimValueHelper(string token, string claimType)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var jwtToken = tokenHandler.ReadJwtToken(token);

        var claim = jwtToken.Claims.FirstOrDefault(c => c.Type == claimType);

        return claim.Value;
    }
}