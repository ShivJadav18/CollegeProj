using ElectroSphereProj.Data;
using ElectroSphereProj.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        List<Item> items = _context.Items.Include(i => i.Category).Where(i => i.Isdeleted == false && i.Isavailable == true).ToList();
        List<ProductListViewModel> productListViewModels = new List<ProductListViewModel>{};
        foreach(Item item in items){
            ProductListViewModel productListViewModel = new ProductListViewModel{};
            productListViewModel.producId = item.ItemId;
            productListViewModel.productDescription = item.Description;
            productListViewModel.productImgUrl = item.Imageurl;
            productListViewModel.productName = item.Name;
            productListViewModel.productPrice =(decimal) item.Rate;
            productListViewModel.companyName = item.Category.Categoryname;
            productListViewModels.Add(productListViewModel);
        }

        return View(productListViewModels);
    }

    public IActionResult ProductDetailView(int id){
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
            Console.WriteLine(items+"items");
            var productListViewModels = new List<ProductListViewModel>{};
            foreach(Item item in items){
            ProductListViewModel productListViewModel = new ProductListViewModel{};
            productListViewModel.producId = item.ItemId;
            productListViewModel.productDescription = item.Description;
            productListViewModel.productImgUrl = item.Imageurl;
            productListViewModel.productName = item.Name;
            productListViewModel.productPrice =(decimal) item.Rate;
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


            return PartialView("_Products",productListViewModels);
        }
}