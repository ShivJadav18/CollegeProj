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



}